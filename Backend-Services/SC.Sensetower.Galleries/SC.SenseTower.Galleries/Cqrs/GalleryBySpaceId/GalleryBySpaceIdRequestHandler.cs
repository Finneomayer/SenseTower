using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Dto.Galleries;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.GalleryBySpaceId
{
    public class GalleryBySpaceIdRequestHandler : BaseHandler, IRequestHandler<GalleryBySpaceIdRequest, GalleryDto?>
    {
        private readonly GalleriesService galleriesService;

        public GalleryBySpaceIdRequestHandler(
            ILogger<GalleryBySpaceIdRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<GalleryDto?> Handle(GalleryBySpaceIdRequest request, CancellationToken cancellationToken)
        {
            var data = await galleriesService.GetBySpace(request.SpaceId, cancellationToken);
            var result = Mapper.Map<GalleryDto>(data);
            return result;
        }
    }
}
