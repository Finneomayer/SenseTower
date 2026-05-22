using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class LookupUsersRequestHandler : BaseHandler, IRequestHandler<LookupUsersRequest, LookupItemDto<Guid>[]?>
    {
        private readonly IdentityService identityService;

        public LookupUsersRequestHandler(
            ILogger<LookupUsersRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<LookupItemDto<Guid>[]?> Handle(LookupUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await identityService.LookupUsers(request.UserIds, request.RoleName, request.AccessToken, cancellationToken);
            if (result == null)
                throw new ScException("Ошибка получения списка пользователей.");
            return result;
        }
    }
}
