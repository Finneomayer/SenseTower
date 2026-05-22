using MediatR;

namespace SC.SenseTower.MyPlaces.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }
    }
}
