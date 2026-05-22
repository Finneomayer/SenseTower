using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryDelete
{
    public class GalleryDeleteCommandHandler : BaseHandler, IRequestHandler<GalleryDeleteCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public GalleryDeleteCommandHandler(
            ILogger<GalleryDeleteCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(GalleryDeleteCommand request, CancellationToken cancellationToken)
        {
            await galleriesService.Delete(request.AccessToken, request.GalleryId, cancellationToken);
            return Unit.Value;
        }
    }
}
