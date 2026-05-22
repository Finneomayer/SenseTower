using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class ResetRequestCommand : IRequest<bool>
    {
        public string LoginOrEmail { get; set; } = null!;
    }
}
