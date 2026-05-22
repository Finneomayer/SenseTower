using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class ResetRequestCommandHandler : BaseHandler, IRequestHandler<ResetRequestCommand, bool>
    {
        private readonly IdentityService identityService;
        private readonly AccountsService accountsService;

        public ResetRequestCommandHandler(
            ILogger<ResetRequestCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.accountsService = accountsService;
        }

        public async Task<bool> Handle(ResetRequestCommand request, CancellationToken cancellationToken)
        {
            var sendResult = await identityService.SendResetPasswordMail(request.LoginOrEmail, cancellationToken);
            if (sendResult == null)
            {
                return false;
            }

            await accountsService.SaveResetPasswordToken(sendResult.UserId, sendResult.Token, cancellationToken);

            return true;
        }
    }
}
