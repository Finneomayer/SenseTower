using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class RegisterUserCommandHandler : BaseHandler, IRequestHandler<RegisterUserCommand, LogonResultDto?>
    {
        private readonly IdentityService identityService;
        private readonly InvitesService invitesService;
        private readonly AccountsService accountsService;
        private readonly IMediator mediator;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            InvitesService invitesService,
            IMediator mediator,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.invitesService = invitesService;
            this.mediator = mediator;
            this.accountsService = accountsService;
        }

        public async Task<LogonResultDto?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var registerResult = await identityService.Register(request.Login, request.Email, request.Password, null, cancellationToken);
            if (registerResult.UserId == null)
                throw new ScException(registerResult.Message);

            LogonResultDto? result = null;
            try
            {
                await invitesService.MarkAsUsed(request.InviteId, registerResult.UserId.Value, cancellationToken);

                var invite = await invitesService.Get(request.InviteId, cancellationToken);
                if (invite == null)
                    throw new ScException("Ошибка получения инвайта пользователя.");
                var accountId = await accountsService.Create(registerResult.UserId.Value, invite.IssuerId, cancellationToken);
                if (accountId == null)
                    throw new ScException("Ошибка создания аккаунта.");

                var response = await identityService.Logon(request.Login, request.Password, cancellationToken);
                if (response == null || string.IsNullOrEmpty(response.AccessToken))
                    throw new ScException("Ошибка входа в приложение.");

                var userInfo = await identityService.GetIdentityInfo(response.AccessToken, cancellationToken);
                if (userInfo == null)
                    throw new ScException("Ошибка получения токена.");

                result = Mapper.Map<LogonResultDto>(userInfo);
                result.AccessToken = response.AccessToken;
                result.RefeshToken = response.RefreshToken;

                var wallets = request.Wallets?.Where(r => !string.IsNullOrWhiteSpace(r.WalletId)).ToArray();
                if (wallets != null && wallets.Length > 0 && userInfo != null)
                {
                    wallets.ForEach(async c =>
                    {
                        try
                        {
                            c.OwnerId = userInfo.UserId;
                            await mediator.Send(c, cancellationToken).ConfigureAwait(false);
                        }
                        catch
                        {
                        //todo: сделать возврат предупреждений на фронт
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                //todo: сделать возврат предупреждений на фронт
                Logger.LogError(ex.Message);
                result ??= new LogonResultDto();
            }

            return result;
        }
    }
}
