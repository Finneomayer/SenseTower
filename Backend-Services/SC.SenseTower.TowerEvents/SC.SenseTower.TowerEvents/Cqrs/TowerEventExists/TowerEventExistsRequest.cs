using MediatR;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventExists
{
    public class TowerEventExistsRequest : IRequest<bool>
    {
        public Guid EventId { get; set; }
    }
}
