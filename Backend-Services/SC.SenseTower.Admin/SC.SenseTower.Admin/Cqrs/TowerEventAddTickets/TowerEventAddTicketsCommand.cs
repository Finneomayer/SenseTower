using MediatR;
using SC.SenseTower.Admin.Dto;

namespace SC.SenseTower.Admin.Cqrs.TowerEventAddTickets
{
    public class TowerEventAddTicketsCommand : ExternalRequestDto, IRequest<Unit>
    {
        public Guid EventId { get; set; }

        public int Quantity { get; set; }
    }
}
