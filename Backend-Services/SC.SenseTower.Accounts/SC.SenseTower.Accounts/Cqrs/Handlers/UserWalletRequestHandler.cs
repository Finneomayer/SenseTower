using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.Wallets;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class UserWalletRequestHandler : BaseHandler, IRequestHandler<UserWalletRequest, WalletDto?>
    {
        private readonly WalletsService walletsService;

        public UserWalletRequestHandler(
            ILogger<UserWalletRequestHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<WalletDto?> Handle(UserWalletRequest request, CancellationToken cancellationToken)
        {
            var wallet = await walletsService.GetWallet(request.WalletId, cancellationToken).ConfigureAwait(false);
            return Mapper.Map<WalletDto>(wallet);
        }
    }
}
