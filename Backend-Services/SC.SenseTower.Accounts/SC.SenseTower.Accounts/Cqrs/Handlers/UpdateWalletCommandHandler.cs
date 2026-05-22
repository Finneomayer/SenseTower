using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class UpdateWalletCommandHandler : BaseHandler, IRequestHandler<UpdateWalletCommand, bool>
    {
        private readonly WalletsService walletsService;

        public UpdateWalletCommandHandler(
            ILogger<UpdateWalletCommandHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<bool> Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
        {
            return await walletsService.UpdateWallet(request.WalletId, request.Name, request.IsActive, cancellationToken).ConfigureAwait(false);
        }
    }
}
