using MediatR;

namespace SC.SenseTower.Halls.RabbitMQ.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
