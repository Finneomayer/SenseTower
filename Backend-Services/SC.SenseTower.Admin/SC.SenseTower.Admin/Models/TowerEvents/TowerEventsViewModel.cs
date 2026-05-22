using SC.SenseTower.Admin.Cqrs.TowerEventsPage;
using SC.SenseTower.Admin.Dto.TowerEvents;

namespace SC.SenseTower.Admin.Models.TowerEvents
{
    public class TowerEventsViewModel : BaseListViewModel<TowerEventGridItemDto>
    {
        public TowerEventsPageFilter Filter { get; set; } = new();
    }
}
