using MediatR;
using SC.SenseTower.Tickets.Dto.Tickets;

namespace SC.SenseTower.Tickets.Cqrs.EventTicketList
{
    public class EventTicketListRequest : IRequest<IEnumerable<TicketDto>>
    {
        public Guid EventId { get; set; }
    }
}
