using MediatR;

namespace SC.SenseTower.TowerEvents.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }
    }
}
