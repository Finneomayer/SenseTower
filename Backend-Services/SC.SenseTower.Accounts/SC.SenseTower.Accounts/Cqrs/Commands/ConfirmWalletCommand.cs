using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class ConfirmWalletCommand : IRequest<bool>
    {
        public string? WalletId { get; set; }
    }
}
