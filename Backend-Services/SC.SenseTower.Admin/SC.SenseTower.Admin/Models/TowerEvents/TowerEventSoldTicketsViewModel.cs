using SC.SenseTower.Admin.Dto.TowerEvents;

namespace SC.SenseTower.Admin.Models.TowerEvents
{
    public class TowerEventSoldTicketsViewModel
    {
        public IEnumerable<TowerEventTicketDto> Tickets { get; set; } = Array.Empty<TowerEventTicketDto>();
    }
}
