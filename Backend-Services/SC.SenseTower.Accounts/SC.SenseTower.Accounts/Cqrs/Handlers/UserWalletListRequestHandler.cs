using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.Wallets;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class UserWalletListRequestHandler : BaseHandler, IRequestHandler<UserWalletListRequest, WalletItemDto[]>
    {
        private readonly WalletsService walletsService;

        public UserWalletListRequestHandler(
            ILogger<UserWalletListRequestHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<WalletItemDto[]> Handle(UserWalletListRequest request, CancellationToken cancellationToken)
        {
            return await walletsService.GetUserWallets(request.OwnerId, cancellationToken).ConfigureAwait(false);
        }
    }
}
