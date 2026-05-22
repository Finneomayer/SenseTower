using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class ConfirmWalletCommandHandler : BaseHandler, IRequestHandler<ConfirmWalletCommand, bool>
    {
        private readonly WalletsService walletsService;

        public ConfirmWalletCommandHandler(
            ILogger<ConfirmWalletCommandHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<bool> Handle(ConfirmWalletCommand request, CancellationToken cancellationToken)
        {
            return await walletsService.Confirm(request.WalletId, cancellationToken).ConfigureAwait(false);
        }
    }
}
