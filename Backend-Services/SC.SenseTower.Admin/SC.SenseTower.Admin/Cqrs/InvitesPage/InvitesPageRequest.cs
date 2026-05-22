using MediatR;
using SC.SenseTower.Admin.Dto.Invites;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.InvitesPage
{
    public class InvitesPageRequest : BaseListRequest, IRequest<PagedDataDto<InviteGridItemDto>>
    {
        public InvitesPageFilter Filters { get; set; } = new();
    }
}
