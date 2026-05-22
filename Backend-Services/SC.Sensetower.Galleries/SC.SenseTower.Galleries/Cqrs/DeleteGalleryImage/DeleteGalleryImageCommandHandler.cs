using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.DeleteGalleryImage
{
    public class DeleteGalleryImageCommandHandler : BaseHandler, IRequestHandler<DeleteGalleryImageCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public DeleteGalleryImageCommandHandler(
            ILogger<DeleteGalleryImageCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(DeleteGalleryImageCommand request, CancellationToken cancellationToken)
        {
            await galleriesService.DeletePicture(request.GalleryId, request.Position, cancellationToken);
            return Unit.Value;
        }
    }
}
