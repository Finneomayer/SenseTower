using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class RegisterUserByTicketCommandHandler : BaseHandler, IRequestHandler<RegisterUserByTicketCommand, LogonResultDto?>
    {
        private readonly IdentityService identityService;
        private readonly TicketsService ticketsService;
        private readonly AccountsService accountsService;
        private readonly IMediator mediator;

        public RegisterUserByTicketCommandHandler(
            ILogger<RegisterUserByTicketCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            TicketsService ticketsService,
            IMediator mediator,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.ticketsService = ticketsService;
            this.mediator = mediator;
            this.accountsService = accountsService;
        }

        public async Task<LogonResultDto?> Handle(RegisterUserByTicketCommand request, CancellationToken cancellationToken)
        {
            var registerResult = await identityService.Register(
                request.Login,
                request.Email,
                request.Password,
                DateTime.UtcNow.AddDays(1),
                cancellationToken);
            if (registerResult.UserId == null)
                throw new ScException(registerResult.Message);

            LogonResultDto? result = null;
            try
            {
                await ticketsService.MarkAsUsed(request.TicketId, registerResult.UserId.Value, cancellationToken);

                var ticket = await ticketsService.Get(request.TicketId, cancellationToken);
                if (ticket == null)
                    throw new ScException("Ошибка получения билета пользователя.");
                var accountId = await accountsService.Create(registerResult.UserId.Value, ticket.IssuerId, cancellationToken);
                if (accountId == null)
                    throw new ScException("Ошибка создания аккаунта.");

                try
                {
                    var response = await identityService.Logon(request.Login, request.Password, cancellationToken);
                    if (response == null || string.IsNullOrEmpty(response.AccessToken))
                        throw new ScException("Ошибка входа в приложение.");

                    var userInfo = await identityService.GetIdentityInfo(response.AccessToken, cancellationToken);
                    if (userInfo == null)
                        throw new ScException("Ошибка получения токена.");

                    result = Mapper.Map<LogonResultDto>(userInfo);
                    result.AccessToken = response.AccessToken;
                    result.RefeshToken = response.RefreshToken;
                }
                catch
                {
                    await accountsService.Delete(accountId.Value, cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                //todo: сделать возврат предупреждений на фронт
                Logger.LogError(ex.Message);
                result ??= new LogonResultDto();
                var logonResponse = await identityService.Logon(request.Login, request.Password, cancellationToken);
                if (!string.IsNullOrEmpty(logonResponse?.AccessToken))
                {
                    await identityService.DeleteUser(logonResponse.AccessToken, registerResult.UserId.Value, cancellationToken);
                }
            }

            return result;
        }
    }
}
