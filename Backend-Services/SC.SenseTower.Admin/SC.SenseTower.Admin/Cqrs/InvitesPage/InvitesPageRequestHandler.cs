using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Dto.Invites;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.InvitesPage
{
    public class InvitesPageRequestHandler : BaseHandler, IRequestHandler<InvitesPageRequest, PagedDataDto<InviteGridItemDto>>
    {
        private readonly InvitesService invitesService;
        private readonly IdentityService identityService;

        public InvitesPageRequestHandler(
            ILogger<InvitesPageRequestHandler> logger,
            IMapper mapper,
            InvitesService invitesService,
            IdentityService identityService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
            this.identityService = identityService;
        }

        public async Task<PagedDataDto<InviteGridItemDto>> Handle(InvitesPageRequest request, CancellationToken cancellationToken)
        {
            var start = (request.CurrentPage - 1) * request.PageSize;
            var filter = await request.Filters.Filter(identityService, cancellationToken);
            var sorting = request.Sorting.Length == 0
                ? new QuerySorting[] { new QuerySorting { Ascending = true, PropertyName = nameof(Invite.Id), SortOrder = 0 } }
                : request.Sorting;
            var data = await invitesService.GetInvites(sorting, filter, start, request.PageSize, cancellationToken);
            var userIds = data
                .Where(r => r.IssuerId != null)
                .Select(r => r.IssuerId)
                .Distinct();
            userIds = userIds.Union(data
                .Where(r => r.UsingInfo != null && r.UsingInfo.UserId != null)
                .Select(r => r.UsingInfo.UserId)
                .Distinct());
            var users = await identityService.GetUserLookups(userIds, cancellationToken);
            var items = data
                .Select(r => Mapper.Map<Invite, InviteGridItemDto>(r, o =>
                {
                    o.AfterMap((s, d) =>
                    {
                        if (s.IssuerId != null)
                            d.IssuerName = users.FirstOrDefault(u => u.Id == s.IssuerId)?.Name;
                        if (s.UsingInfo?.UserId != null)
                            d.UserName = users.FirstOrDefault(u => u.Id == s.UsingInfo.UserId)?.Name;
                    });
                }))
                .ToArray();
            var result = new PagedDataDto<InviteGridItemDto>
            {
                Items = items,
                TotalCount = await invitesService.Count(filter, cancellationToken)
            };
            return result;
        }
    }
}
