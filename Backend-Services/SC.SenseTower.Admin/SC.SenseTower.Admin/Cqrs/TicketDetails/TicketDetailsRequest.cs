using MediatR;
using SC.SenseTower.Admin.Dto.Tickets;

namespace SC.SenseTower.Admin.Cqrs.TicketDetails
{
    public class TicketDetailsRequest : IRequest<TicketDetailsDto>
    {
        public string TicketId { get; set; } = null!;
    }
}
