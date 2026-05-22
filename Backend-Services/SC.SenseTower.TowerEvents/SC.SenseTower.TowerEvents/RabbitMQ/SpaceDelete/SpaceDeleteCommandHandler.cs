using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommandHandler : BaseHandler, IRequestHandler<SpaceDeleteCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public SpaceDeleteCommandHandler(
            ILogger<SpaceDeleteCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(SpaceDeleteCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.DeleteSpace(request.SpaceId, cancellationToken);
            return Unit.Value;
        }
    }
}
