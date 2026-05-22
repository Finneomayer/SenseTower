using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Data.Models;
using SC.SenseTower.Tickets.RabbitMQ;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.Cqrs.TicketsCreate
{
    public class TicketsCreateCommandHandler : BaseHandler, IRequestHandler<TicketsCreateCommand, Unit>
    {
        private readonly TicketsService ticketsService;
        private readonly TowerEventsService towerEventsService;
        private readonly RabbitMQService rabbitMQService;

        public TicketsCreateCommandHandler(
            ILogger<TicketsCreateCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService,
            TowerEventsService towerEventsService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
            this.towerEventsService = towerEventsService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(TicketsCreateCommand request, CancellationToken cancellationToken)
        {
            var ticket = Mapper.Map<Ticket>(request);
            //var towerEventDto = await towerEventsService.Get(request.AccessToken, request.EventId, cancellationToken);
            //if (towerEventDto != null)
            //{
            //    ticket.TowerEvent = Mapper.Map<TowerEvent>(towerEventDto);
            //}
            await ticketsService.Create(ticket, request.Quantity, cancellationToken);
            var total = await ticketsService.CountTotal(request.EventId, cancellationToken);
            await rabbitMQService.SendTotalTicketsMessage(request.EventId, total, cancellationToken);
            return Unit.Value;
        }
    }
}
