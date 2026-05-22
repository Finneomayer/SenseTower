using MediatR;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventList
{
    public class TowerEventListRequest : TowerEventListFilter, IRequest<TowerEventListItemDto[]>
    {
    }
}
