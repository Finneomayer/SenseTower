using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly AccountsService accountsService;
        private readonly InvitesService invitesService;
        private readonly WalletsService walletsService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            AccountsService accountsService,
            InvitesService invitesService,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.accountsService = accountsService;
            this.invitesService = invitesService;
            this.walletsService = walletsService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await accountsService.Delete(request.UserId, cancellationToken);
                await invitesService.DeleteByUser(request.UserId, cancellationToken);
                await walletsService.DeleteByUser(request.UserId, cancellationToken);
            }
            catch (Exception ex)
            {
                //todo: сделать передачу предупреждений на фронт
                Logger.LogError(ex.Message);
            }
            return Unit.Value;
        }
    }
}
