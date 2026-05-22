using MediatR;

namespace SC.SenseTower.Tickets.Cqrs.BuyTicket
{
    public class BuyTicketCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }
    }
}
