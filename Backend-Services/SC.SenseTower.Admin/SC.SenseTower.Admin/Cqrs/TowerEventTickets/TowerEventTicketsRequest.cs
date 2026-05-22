using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.TowerEvents;

namespace SC.SenseTower.Admin.Cqrs.TowerEventTickets
{
    public class TowerEventTicketsRequest : ItemRequestDto, IRequest<IEnumerable<TowerEventTicketDto>>
    {
    }
}
