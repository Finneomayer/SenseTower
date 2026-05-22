using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class DeleteWalletCommandHandler : BaseHandler, IRequestHandler<DeleteWalletCommand, bool>
    {
        private readonly WalletsService walletsService;

        public DeleteWalletCommandHandler(
            ILogger<DeleteWalletCommandHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<bool> Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
        {
            return await walletsService.DeleteWallet(request.WalletId, cancellationToken).ConfigureAwait(false);
        }
    }
}
