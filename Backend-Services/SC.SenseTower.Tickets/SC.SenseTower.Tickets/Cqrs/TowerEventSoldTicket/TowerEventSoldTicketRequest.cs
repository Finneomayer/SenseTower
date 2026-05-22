using MediatR;
using SC.SenseTower.Tickets.Dto.Tickets;

namespace SC.SenseTower.Tickets.Cqrs.TowerEventSoldTicket
{
    public class TowerEventSoldTicketRequest : IRequest<IEnumerable<SoldTicketInfoDto>>
    {
        public Guid EventId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
