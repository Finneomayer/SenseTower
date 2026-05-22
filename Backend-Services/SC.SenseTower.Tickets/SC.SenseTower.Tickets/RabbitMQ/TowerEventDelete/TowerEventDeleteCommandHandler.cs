using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.RabbitMQ.TowerEventDelete
{
    public class TowerEventDeleteCommandHandler : BaseHandler, IRequestHandler<TowerEventDeleteCommand, Unit>
    {
        private readonly TicketsService ticketsService;

        public TowerEventDeleteCommandHandler(
            ILogger<TowerEventDeleteCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<Unit> Handle(TowerEventDeleteCommand request, CancellationToken cancellationToken)
        {
            await ticketsService.DeleteTowerEvent(request.EventId, cancellationToken);
            return Unit.Value;
        }
    }
}
