using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Services;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Cqrs.ImagesByIds
{
    public class ImagesByIdsRequestHandler : BaseHandler, IRequestHandler<ImagesByIdsRequest, IEnumerable<ImageInfoDto>>
    {
        private readonly ImageFilesService imageFilesService;
        private readonly ImageSettings settings;

        public ImagesByIdsRequestHandler(
            ILogger<ImagesByIdsRequestHandler> logger,
            IMapper mapper,
            ImageFilesService imageFilesService,
            IOptions<ImageSettings> options) : base(logger, mapper)
        {
            this.imageFilesService = imageFilesService;
            settings = options.Value;
        }

        public async Task<IEnumerable<ImageInfoDto>> Handle(ImagesByIdsRequest request, CancellationToken cancellationToken)
        {
            var imageFiles = await imageFilesService.GetByIds(request.ImageIds, cancellationToken);
            var result = Mapper.Map<IEnumerable<ImageFile>, ImageInfoDto[]>(imageFiles, o =>
            {
                o.AfterMap((s, d) =>
                {
                    foreach (var item in d)
                    {
                        item.FileUrl = settings.RootUrl + item.FileUrl;
                        item.PreviewUrl = settings.RootUrl + item.PreviewUrl;
                    }
                });
            });
            return result;
        }
    }
}
