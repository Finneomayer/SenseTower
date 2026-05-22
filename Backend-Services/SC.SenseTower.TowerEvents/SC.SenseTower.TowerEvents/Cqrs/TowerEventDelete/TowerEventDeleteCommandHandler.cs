using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.RabbitMQ;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventDelete
{
    public class TowerEventDeleteCommandHandler : BaseHandler, IRequestHandler<TowerEventDeleteCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;
        private readonly RabbitMQService rabbitMQService;

        public TowerEventDeleteCommandHandler(
            ILogger<TowerEventDeleteCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(TowerEventDeleteCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.Delete(request.EventId, cancellationToken);
            await rabbitMQService.SendTowerEventDeleteMessage(request.EventId, cancellationToken);
            return Unit.Value;
        }
    }
}
