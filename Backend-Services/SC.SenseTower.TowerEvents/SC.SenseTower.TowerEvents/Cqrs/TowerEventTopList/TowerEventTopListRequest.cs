using MediatR;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventTopList
{
    public class TowerEventTopListRequest : IRequest<IEnumerable<TowerEventListItemDto>>
    {
        public int? Limit { get; set; }
    }
}
