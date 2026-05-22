using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryImages
{
    public class GalleryImagesRequestHandler : BaseHandler, IRequestHandler<GalleryImagesRequest, IEnumerable<GalleryImageDto>>
    {
        private readonly GalleriesService galleriesService;

        public GalleryImagesRequestHandler(
            ILogger<GalleryImagesRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<IEnumerable<GalleryImageDto>> Handle(GalleryImagesRequest request, CancellationToken cancellationToken)
        {
            var result = await galleriesService.GetGalleryImages(request.AccessToken, request.GalleryId, cancellationToken);
            return result;
        }
    }
}
