using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Tickets.RabbitMQ;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.Cqrs.BuyTicket
{
    public class BuyTicketCommandHandler : BaseHandler, IRequestHandler<BuyTicketCommand, Unit>
    {
        private readonly TicketsService ticketsService;
        private readonly RabbitMQService rabbitMQService;

        public BuyTicketCommandHandler(
            ILogger<BuyTicketCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(BuyTicketCommand request, CancellationToken cancellationToken)
        {
            var result = await ticketsService.Buy(request.EventId, request.UserId, cancellationToken);
            if (result == null)
                throw new ScException("Нет свободных билетов");
            await rabbitMQService.SendTicketBoughtMessage(request.EventId, result.Value, request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}
