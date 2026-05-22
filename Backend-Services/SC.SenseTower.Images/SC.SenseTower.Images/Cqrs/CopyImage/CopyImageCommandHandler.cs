using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Services;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Cqrs.CopyImage
{
    public class CopyImageCommandHandler : BaseHandler, IRequestHandler<CopyImageCommand, ImageInfoDto>
    {
        private readonly ImageStorageService imageStorageService;
        private readonly ImageFilesService imageFilesService;
        private readonly ImageSettings settings;

        public CopyImageCommandHandler(
            ILogger<CopyImageCommandHandler> logger,
            IMapper mapper,
            ImageStorageService imageStorageService,
            ImageFilesService imageFilesService,
            IOptions<ImageSettings> options) : base(logger, mapper)
        {
            this.imageStorageService = imageStorageService;
            this.imageFilesService = imageFilesService;
            settings = options.Value;
        }

        public async Task<ImageInfoDto> Handle(CopyImageCommand request, CancellationToken cancellationToken)
        {
            var image = await imageFilesService.Get(request.Id, cancellationToken);
            image.Id = Guid.NewGuid();

            await imageFilesService.Create(image.Id, null, image.Name, image.FileName, cancellationToken);

            var ext = Path.GetExtension(image.FileName);
            var content = await imageStorageService.Get($"{request.Id}_full{ext}", cancellationToken);
            await imageStorageService.Save(content, $"{image.Id}_full{ext}", cancellationToken);
            content = await imageStorageService.Get($"{request.Id}_pre{ext}", cancellationToken);
            await imageStorageService.Save(content, $"{image.Id}_pre{ext}", cancellationToken);

            var data = await imageFilesService.Get(image.Id, cancellationToken);
            var result = Mapper.Map<ImageFile, ImageInfoDto>(data, o =>
            {
                o.AfterMap((s, d) =>
                {
                    d.FileUrl = settings.RootUrl + d.FileUrl;
                    d.PreviewUrl = settings.RootUrl + d.PreviewUrl;
                });
            });
            return result;
        }
    }
}
