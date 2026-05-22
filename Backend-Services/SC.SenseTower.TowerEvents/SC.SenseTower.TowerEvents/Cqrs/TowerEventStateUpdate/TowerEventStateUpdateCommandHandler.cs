using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.RabbitMQ;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventStateUpdate
{
    public class TowerEventStateUpdateCommandHandler : BaseHandler, IRequestHandler<TowerEventStateUpdateCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;
        private readonly RabbitMQService rabbitMQService;

        public TowerEventStateUpdateCommandHandler(
            ILogger<TowerEventStateUpdateCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(TowerEventStateUpdateCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.UpdateState(request.EventId, request.State, cancellationToken);
            var towerEvent = await towerEventsService.Get(request.EventId, cancellationToken);
            await rabbitMQService.SendTowerEventUpdateMessage(towerEvent, cancellationToken);
            return Unit.Value;
        }
    }
}
