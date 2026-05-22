using MediatR;

namespace SC.SenseTower.Halls.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }
    }
}
