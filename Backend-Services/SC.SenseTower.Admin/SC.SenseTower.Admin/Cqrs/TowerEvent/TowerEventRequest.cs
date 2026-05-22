using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.TowerEvents;

namespace SC.SenseTower.Admin.Cqrs.TowerEvent
{
    public class TowerEventRequest : ItemRequestDto, IRequest<TowerEventDto>
    {
    }
}
