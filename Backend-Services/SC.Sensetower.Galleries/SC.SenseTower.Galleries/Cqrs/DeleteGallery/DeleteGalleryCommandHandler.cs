using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.RabbitMQ;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.DeleteGallery
{
    public class DeleteGalleryCommandHandler : BaseHandler, IRequestHandler<DeleteGalleryCommand, Unit>
    {
        private readonly GalleriesService galleriesService;
        private readonly RabbitMQService rabbitMQService;

        public DeleteGalleryCommandHandler(
            ILogger<DeleteGalleryCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(DeleteGalleryCommand request, CancellationToken cancellationToken)
        {
            var gallery = await galleriesService.Get(request.Id, cancellationToken);
            var imageIds = gallery.Pictures
                .Select(x => x.Image.Image.Id)
                .Distinct()
                .ToArray();
            await galleriesService.Delete(request.Id, cancellationToken);
            await rabbitMQService.SendImagesDeleteMessage(imageIds, cancellationToken);
            return Unit.Value;
        }
    }
}
