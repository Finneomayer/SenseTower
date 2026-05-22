using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryImageRemove
{
    public class GalleryImageRemoveCommandHandler : BaseHandler, IRequestHandler<GalleryImageRemoveCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public GalleryImageRemoveCommandHandler(
            ILogger<GalleryImageRemoveCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(GalleryImageRemoveCommand request, CancellationToken cancellationToken)
        {
            await galleriesService.RemoveImage(request.AccessToken, request.GalleryId, request.Position, cancellationToken);
            return Unit.Value;
        }
    }
}
