using SC.SenseTower.Admin.Cqrs.PlacesPage;
using SC.SenseTower.Admin.Dto.Places;

namespace SC.SenseTower.Admin.Models.Places
{
    public class PlacesViewModel : BaseListViewModel<PlacesGridItemDto>
    {
        public PlacesPageFilter Filter { get; set; } = new();
    }
}
