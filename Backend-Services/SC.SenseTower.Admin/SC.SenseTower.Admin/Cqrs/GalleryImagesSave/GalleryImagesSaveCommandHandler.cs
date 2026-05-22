using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryImagesSave
{
    public class GalleryImagesSaveCommandHandler : BaseHandler, IRequestHandler<GalleryImagesSaveCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public GalleryImagesSaveCommandHandler(
            ILogger<GalleryImagesSaveCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(GalleryImagesSaveCommand request, CancellationToken cancellationToken)
        {
            await galleriesService.UpdateImages(request.AccessToken, request.GalleryId, request.Images, cancellationToken);
            return Unit.Value;
        }
    }
}
