using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class LogonCommandHandler : BaseHandler, IRequestHandler<LogonCommand, LogonResultDto?>
    {
        private readonly IdentityService identityService;
        private readonly AccountsService accountsService;

        public LogonCommandHandler(
            ILogger<LogonCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.accountsService = accountsService;
        }

        public async Task<LogonResultDto?> Handle(LogonCommand request, CancellationToken cancellationToken)
        {
            var response = await identityService.Logon(request.Login, request.Password, cancellationToken);
            if (response == null)
                throw new ScException("Ошибка входа в приложение.");
            if (response.IsError)
                throw new ScException($"Ошибка входа в приложение: {response.Error}.");
            if (string.IsNullOrEmpty(response.AccessToken))
                throw new ScException("Ошибка получения токена.");

            var userInfo = await identityService.GetIdentityInfo(response.AccessToken, cancellationToken);
            if (userInfo == null)
                throw new ScException("Ошибка получения информации о пользователе.");

            var account = await accountsService.Get(userInfo.UserId, cancellationToken);
            var result = Mapper.Map<LogonResultDto>(userInfo);
            result.AccessToken = response.AccessToken;
            result.RefeshToken = response.RefreshToken;
            result.AvatarNumber = account.AvatarNumber;

            return result;
        }
    }
}
