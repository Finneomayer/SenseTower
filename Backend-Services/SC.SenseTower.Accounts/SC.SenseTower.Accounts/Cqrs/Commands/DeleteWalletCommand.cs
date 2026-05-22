using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class DeleteWalletCommand : IRequest<bool>
    {
        public string WalletId { get; set; } = null!;

        public Guid? OwnerId { get; set; }
    }
}
