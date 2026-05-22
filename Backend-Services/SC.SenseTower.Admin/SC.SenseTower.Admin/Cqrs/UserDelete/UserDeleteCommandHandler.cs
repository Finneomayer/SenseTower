using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using System.Net;

namespace SC.SenseTower.Admin.Cqrs.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly AccountsHttpService accountsHttpService;
        private readonly IS4Service is4Service;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            AccountsHttpService accountsHttpService,
            IS4Service is4Service,
            IHttpContextAccessor httpContextAccessor) : base(logger, mapper)
        {
            this.accountsHttpService = accountsHttpService;
            this.is4Service = is4Service;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            var cnt = 0;
            do
            {
                try
                {
                    var result = await accountsHttpService.Delete(request.UserId, request.AccessToken, cancellationToken);
                    if (!result)
                        throw new ScException("Пользователь не удалён.");
                    return Unit.Value;
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode != HttpStatusCode.Unauthorized)
                        throw;
                    cnt++;
                    if (cnt < 2)
                    {
                        var refreshResult = await is4Service.Refresh(request.RefreshToken, cancellationToken);
                        if (refreshResult == null)
                            throw new ScException("Не удалось обновить токен пользователя.");
                        var response = httpContextAccessor.HttpContext.Response;
                        response.Cookies.Append("SC.SenseTower.Admin.Token", refreshResult.AccessToken);
                        response.Cookies.Append("SC.SenseTower.Admin.Refresh", refreshResult.RefreshToken);
                        request.AccessToken = refreshResult.AccessToken;
                    }
                }
            } while (cnt < 2);

            throw new ScException("Недостаточно прав для удаления пользователя.");
        }
    }
}
