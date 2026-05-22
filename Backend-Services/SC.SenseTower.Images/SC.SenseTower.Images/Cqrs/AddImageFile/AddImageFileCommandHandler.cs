using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Images.Services;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Cqrs.AddImageFile
{
    public class AddImageFileCommandHandler : BaseHandler, IRequestHandler<AddImageFileCommand, Guid>
    {
        private readonly ImageStorageService imageStorageService;
        private readonly ImageFilesService imageFilesService;
        private readonly ImageProcessingService imageProcessingService;
        private readonly ImageSettings imageSettings;

        public AddImageFileCommandHandler(
            ILogger<AddImageFileCommandHandler> logger,
            IMapper mapper,
            ImageStorageService imageStorageService,
            ImageFilesService imageFilesService,
            ImageProcessingService imageProcessingService,
            IOptions<ImageSettings> options) : base(logger, mapper)
        {
            this.imageStorageService = imageStorageService;
            this.imageFilesService = imageFilesService;
            this.imageProcessingService = imageProcessingService;
            imageSettings = options.Value;
        }

        public async Task<Guid> Handle(AddImageFileCommand request, CancellationToken cancellationToken)
        {
            var fileName = request.File.FileName;
            var ext = Path.GetExtension(fileName);
            var fileId = Guid.NewGuid();
            await imageFilesService.Create(fileId, request.UserId, request.Name, fileName, cancellationToken);
            try
            {
                using var inStream = request.File.OpenReadStream();
                if (inStream.Length > imageSettings.Original.Weight)
                    throw new ScException("Размер файла изображения превышает максимально допустимый");

                using var st = new MemoryStream((int)inStream.Length);
                await inStream.CopyToAsync(st, cancellationToken);
                st.Position = 0;

                var imageSize = await imageProcessingService.GetImageSize(st, cancellationToken);
                if (imageSize.X > imageSettings.Original.Size || imageSize.Y > imageSettings.Original.Size)
                    throw new ScException("Размер изображения превышает максимально допустимый");

                var width = 0;
                var height = 0;
                if (imageSize.X > imageSize.Y)
                {
                    width = imageSettings.Preview.Size;
                    height = imageSize.Y * width / imageSize.X;
                }
                else
                {
                    height = imageSettings.Preview.Size;
                    width = imageSize.X * height / imageSize.Y;
                }
                var preview = await imageProcessingService.Resize(st, width, height, imageSettings.Preview.Resampler, cancellationToken);
                await imageStorageService.Save(st, $"{fileId}_full{ext}", cancellationToken);
                try
                {
                    await imageStorageService.Save(preview, $"{fileId}_pre{ext}", cancellationToken);
                }
                catch
                {
                    await imageStorageService.Delete($"{fileId}_full{ext}", cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                await imageFilesService.Delete(fileId, cancellationToken);
                throw new ScException(ex.Message);
            }

            return fileId;
        }
    }
}
