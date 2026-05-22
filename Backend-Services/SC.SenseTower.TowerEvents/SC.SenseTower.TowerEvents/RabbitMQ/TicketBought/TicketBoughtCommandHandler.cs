using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.RabbitMQ.TicketBought
{
    public class TicketBoughtCommandHandler : BaseHandler, IRequestHandler<TicketBoughtCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public TicketBoughtCommandHandler(
            ILogger<TicketBoughtCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(TicketBoughtCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.SellTicket(request.EventId, request.TicketId, request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}
