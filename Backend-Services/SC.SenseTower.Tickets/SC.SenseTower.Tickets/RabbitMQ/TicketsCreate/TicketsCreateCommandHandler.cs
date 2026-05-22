using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Data.Models;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.RabbitMQ.TicketsCreate
{
    public class TicketsCreateCommandHandler : BaseHandler, IRequestHandler<TicketsCreateCommand, Unit>
    {
        private readonly TicketsService ticketsService;
        private readonly RabbitMQService rabbitMQService;

        public TicketsCreateCommandHandler(
            ILogger<TicketsCreateCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(TicketsCreateCommand request, CancellationToken cancellationToken)
        {
            var ticket = Mapper.Map<Ticket>(request);
            await ticketsService.Create(ticket, request.Quantity, cancellationToken);
            var total = await ticketsService.CountTotal(request.TowerEvent.Id, cancellationToken);
            await rabbitMQService.SendTotalTicketsMessage(request.TowerEvent.Id, total, cancellationToken);
            return Unit.Value;
        }
    }
}
