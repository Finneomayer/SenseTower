using MediatR;
using SC.SenseTower.Accounts.Dto.Wallets;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class UserWalletRequest : IRequest<WalletDto>
    {
        public string WalletId { get; set; } = null!;

        public Guid? OwnerId { get; set; }
    }
}
