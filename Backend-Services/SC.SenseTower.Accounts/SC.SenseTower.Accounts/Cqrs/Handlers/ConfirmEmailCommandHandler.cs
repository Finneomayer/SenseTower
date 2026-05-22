using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class ConfirmEmailCommandHandler : BaseHandler, IRequestHandler<ConfirmEmailCommand, OperationResultDto>
    {
        private readonly IdentityService identityService;
        private readonly InvitesService invitesService;
        private readonly AccountsService accountsService;
        private readonly int maxInvitesCount;

        public ConfirmEmailCommandHandler(
            ILogger<ConfirmEmailCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            InvitesService invitesService,
            IConfiguration configuration,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.invitesService = invitesService;
            this.accountsService = accountsService;
            maxInvitesCount = configuration.GetValue<int>("InvitesForConfirmedUser");
        }

        public async Task<OperationResultDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResultDto
            {
                Message = await identityService.ConfirmEmail(request.UserId, request.Code, cancellationToken)
            };
            if (string.IsNullOrEmpty(result.Message))
            {
                result.Succeeded = true;
                // приглашения пользователю выдаются только в том случае, если он сам был зарегистрирован по приглашению
                if (maxInvitesCount > 0 && invitesService.GetByUser(request.UserId, cancellationToken) != null)
                {
                    try
                    {
                        var account = await accountsService.Get(request.UserId, cancellationToken);
                        if (account.ReferrerId != null)
                        {
                            var referrerInvitesCount = await invitesService.CountByUser(account.ReferrerId.Value, cancellationToken);
                            var invitesCount = Math.Min(referrerInvitesCount - 1, maxInvitesCount);
                            if (invitesCount > 0)
                            {
                                var inviteCodes = await invitesService.CreateUserInvites(request.UserId, request.UserId, invitesCount, cancellationToken);
                                result.Message = $"Для вас созданы инвайты с кодами {string.Join(", ", inviteCodes)}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                        result.Message = $"Инвайты для вас не созданы: {ex.Message}. Обратитесь в службу техподдержки.";
                    }
                }
            }
            return result;
        }
    }
}
