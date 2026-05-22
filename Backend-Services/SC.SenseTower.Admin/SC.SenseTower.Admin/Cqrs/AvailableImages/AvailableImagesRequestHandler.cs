using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.AvailableImages
{
    public class AvailableImagesRequestHandler : BaseHandler, IRequestHandler<AvailableImagesRequest, IEnumerable<ImageInfoDto>>
    {
        private readonly ImagesService imagesService;

        public AvailableImagesRequestHandler(
            ILogger<AvailableImagesRequestHandler> logger,
            IMapper mapper,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.imagesService = imagesService;
        }

        public async Task<IEnumerable<ImageInfoDto>> Handle(AvailableImagesRequest request, CancellationToken cancellationToken)
        {
            var result = await imagesService.Lookup(request.AccessToken, null, cancellationToken);
            return result;
        }
    }
}
