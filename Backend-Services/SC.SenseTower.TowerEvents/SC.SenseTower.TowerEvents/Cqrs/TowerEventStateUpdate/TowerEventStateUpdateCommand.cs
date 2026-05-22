using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventStateUpdate
{
    public class TowerEventStateUpdateCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }

        public TowerEventState State { get; set; }
    }
}
