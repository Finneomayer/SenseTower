using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.GalleryExists
{
    public class GalleryExistsRequestHandler : BaseHandler, IRequestHandler<GalleryExistsRequest, bool>
    {
        private readonly GalleriesService galleriesService;

        public GalleryExistsRequestHandler(
            ILogger<GalleryExistsRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<bool> Handle(GalleryExistsRequest request, CancellationToken cancellationToken)
        {
            var result = await galleriesService.Exists(request.Id, cancellationToken);
            return result;
        }
    }
}
