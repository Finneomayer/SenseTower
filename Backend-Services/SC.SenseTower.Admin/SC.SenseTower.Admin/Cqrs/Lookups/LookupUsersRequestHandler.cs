using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.Lookups
{
    public class LookupUsersRequestHandler : BaseHandler, IRequestHandler<LookupUsersRequest, LookupItemDto<Guid>[]>
    {
        private readonly IdentityService identityService;

        public LookupUsersRequestHandler(
            ILogger<LookupUsersRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<LookupItemDto<Guid>[]> Handle(LookupUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await identityService.GetByRole(request.Role, request.Eq, cancellationToken);
            return Mapper.Map<LookupItemDto<Guid>[]>(users);
        }
    }
}
