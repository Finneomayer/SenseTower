using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class AddWalletCommandHandler : BaseHandler, IRequestHandler<AddWalletCommand, string>
    {
        private readonly WalletsService walletsService;

        public AddWalletCommandHandler(
            ILogger<AddWalletCommandHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<string> Handle(AddWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = Mapper.Map<Wallet>(request);
            var result = await walletsService.AddWallet(wallet, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
