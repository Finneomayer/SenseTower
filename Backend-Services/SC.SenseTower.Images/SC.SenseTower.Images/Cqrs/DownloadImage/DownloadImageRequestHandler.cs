using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.DownloadImage
{
    public class DownloadImageRequestHandler : BaseHandler, IRequestHandler<DownloadImageRequest, DownloadImageDto>
    {
        private readonly ImageFilesService imageFilesService;
        private readonly ImageStorageService imageStorageService;

        public DownloadImageRequestHandler(
            ILogger<DownloadImageRequestHandler> logger,
            IMapper mapper,
            ImageFilesService imageFilesService,
            ImageStorageService imageStorageService) : base(logger, mapper)
        {
            this.imageFilesService = imageFilesService;
            this.imageStorageService = imageStorageService;
        }

        public async Task<DownloadImageDto> Handle(DownloadImageRequest request, CancellationToken cancellationToken)
        {
            var imageFile = await imageFilesService.Get(request.Id, cancellationToken);
            var ext = Path.GetExtension(imageFile?.FileName);
            var fileName = request.IsPreview ? $"{request.Id}_pre{ext}" : $"{request.Id}_full{ext}";
            var result = new DownloadImageDto
            {
                Content = await imageStorageService.Get(fileName, cancellationToken),
                Name = imageFile?.FileName ?? string.Empty
            };
            return result;
        }
    }
}
