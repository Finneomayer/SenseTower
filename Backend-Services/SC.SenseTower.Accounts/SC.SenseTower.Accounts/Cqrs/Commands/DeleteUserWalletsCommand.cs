using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class DeleteUserWalletsCommand : IRequest<bool>
    {
        public Guid? OwnerId { get; set; }
    }
}
