using MediatR;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEvent
{
    public class TowerEventRequest : IRequest<TowerEventDto?>
    {
        public Guid EventId { get; set; }
    }
}
