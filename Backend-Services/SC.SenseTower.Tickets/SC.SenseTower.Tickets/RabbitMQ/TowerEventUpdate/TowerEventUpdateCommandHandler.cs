using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Data.Models;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.RabbitMQ.TowerEventUpdate
{
    public class TowerEventUpdateCommandHandler : BaseHandler, IRequestHandler<TowerEventUpdateCommand, Unit>
    {
        private readonly TicketsService ticketsService;

        public TowerEventUpdateCommandHandler(
            ILogger<TowerEventUpdateCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<Unit> Handle(TowerEventUpdateCommand request, CancellationToken cancellationToken)
        {
            var towerEvent = Mapper.Map<TowerEvent>(request.TowerEvent);
            await ticketsService.UpdateTowerEvent(towerEvent, cancellationToken);
            return Unit.Value;
        }
    }
}
