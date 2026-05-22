using MediatR;

namespace SC.SenseTower.Accounts.RabbitMQ.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
