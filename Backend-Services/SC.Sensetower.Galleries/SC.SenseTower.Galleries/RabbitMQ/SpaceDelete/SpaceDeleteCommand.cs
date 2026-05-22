using MediatR;

namespace SC.SenseTower.Galleries.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }
    }
}
