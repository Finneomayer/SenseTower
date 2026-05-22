using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class CheckLoginRequestHandler : BaseHandler, IRequestHandler<CheckLoginRequest, bool>
    {
        private readonly IdentityService identityService;

        public CheckLoginRequestHandler(
            ILogger<CheckLoginRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<bool> Handle(CheckLoginRequest request, CancellationToken cancellationToken)
        {
            Logger.LogTrace($"Запрошена проверка логина \"{request.Login}\"");
            var result = await identityService.IsLoginFree(request.Login, cancellationToken).ConfigureAwait(false);
            Logger.LogTrace($"Проверка логина \"{request.Login}\" завершена с результатом {result}");
            return result;
        }
    }
}
