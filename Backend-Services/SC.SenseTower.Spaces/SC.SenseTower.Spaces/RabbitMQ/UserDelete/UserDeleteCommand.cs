using MediatR;

namespace SC.SenseTower.Spaces.RabbitMQ.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
