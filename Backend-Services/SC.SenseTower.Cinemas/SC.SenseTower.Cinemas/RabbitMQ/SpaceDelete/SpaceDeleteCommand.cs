using MediatR;

namespace SC.SenseTower.Cinemas.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }
    }
}
