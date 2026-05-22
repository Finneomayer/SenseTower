using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.AvailableUsers
{
    public class AvailableUsersRequestHandler : BaseHandler, IRequestHandler<AvailableUsersRequest, IEnumerable<LookupItemDto<Guid>>>
    {
        private readonly AccountsHttpService accountsService;

        public AvailableUsersRequestHandler(
            ILogger<AvailableUsersRequestHandler> logger,
            IMapper mapper,
            AccountsHttpService accountsService) : base(logger, mapper)
        {
            this.accountsService = accountsService;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Handle(AvailableUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await accountsService.Lookup(request.AccessToken, request.RefreshToken, request.UserIds, request.RoleName, cancellationToken);
            return result;
        }
    }
}
