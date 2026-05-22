using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommandHandler : BaseHandler, IRequestHandler<SpaceDeleteCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public SpaceDeleteCommandHandler(
            ILogger<SpaceDeleteCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(SpaceDeleteCommand request, CancellationToken cancellationToken)
        {
            await galleriesService.DeleteSpace(request.SpaceId, cancellationToken);
            return Unit.Value;
        }
    }
}
