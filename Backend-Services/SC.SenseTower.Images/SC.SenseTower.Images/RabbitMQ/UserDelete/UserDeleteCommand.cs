using MediatR;

namespace SC.SenseTower.Images.RabbitMQ.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
