using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.Lookups
{
    public class LookupRolesRequestHandler : BaseHandler, IRequestHandler<LookupRolesRequest, LookupItemDto<Guid>[]>
    {
        private readonly IdentityService identityService;

        public LookupRolesRequestHandler(
            ILogger<LookupRolesRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<LookupItemDto<Guid>[]> Handle(LookupRolesRequest request, CancellationToken cancellationToken)
        {
            return await identityService.GetRoleLookups(null, cancellationToken);
        }
    }
}
