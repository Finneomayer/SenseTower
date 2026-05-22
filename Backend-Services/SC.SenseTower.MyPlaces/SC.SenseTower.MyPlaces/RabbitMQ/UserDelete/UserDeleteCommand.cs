using MediatR;

namespace SC.SenseTower.MyPlaces.RabbitMQ.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
