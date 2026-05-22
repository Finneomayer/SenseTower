using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto.Users;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.UsersPage
{
    public class UsersPageRequestHandler : BaseHandler, IRequestHandler<UsersPageRequest, PagedDataDto<UserGridItemDto>>
    {
        private readonly IdentityService identityService;

        public UsersPageRequestHandler(
            ILogger<UsersPageRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<PagedDataDto<UserGridItemDto>> Handle(UsersPageRequest request, CancellationToken cancellationToken)
        {
            var start = (request.CurrentPage - 1) * request.PageSize;
            var filter = await request.Filters.Filter(identityService, cancellationToken);
            var data = await identityService.GetUsers(request.Sorting, filter, start, request.PageSize, cancellationToken);
            var roleIds = data.SelectMany(r => r.Roles).Distinct().ToArray();
            var roles = await identityService.GetRoleLookups(roleIds, cancellationToken);
            var items = data
                .Select(r => Mapper.Map<ApplicationUser, UserGridItemDto>(r, o =>
                {
                    o.AfterMap((s, d) =>
                    {
                        d.RoleName = string.Join(", ", roles.Where(x => s.Roles.Contains(x.Id)).Select(x => x.Name));
                    });
                }))
                .ToArray();
            return new PagedDataDto<UserGridItemDto>
            {
                Items = items,
                TotalCount = await identityService.Count(filter, cancellationToken)
            };
        }
    }
}
