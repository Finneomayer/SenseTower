using MediatR;
using SC.SenseTower.Accounts.Dto.Wallets;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class UserWalletListRequest : IRequest<WalletItemDto[]>
    {
        public Guid OwnerId { get; set; }
    }
}
