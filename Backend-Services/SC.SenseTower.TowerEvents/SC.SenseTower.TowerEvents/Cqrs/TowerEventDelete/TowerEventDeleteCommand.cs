using MediatR;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventDelete
{
    public class TowerEventDeleteCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }
    }
}
