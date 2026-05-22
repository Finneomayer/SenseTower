using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Dto.Galleries;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.Gallery
{
    public class GalleryRequestHandler : BaseHandler, IRequestHandler<GalleryRequest, GalleryDto?>
    {
        private readonly GalleriesService galleriesService;

        public GalleryRequestHandler(
            ILogger<GalleryRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<GalleryDto?> Handle(GalleryRequest request, CancellationToken cancellationToken)
        {
            var data = await galleriesService.Get(request.Id, cancellationToken);
            var result = Mapper.Map<GalleryDto>(data);
            return result;
        }
    }
}
