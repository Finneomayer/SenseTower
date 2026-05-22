using MediatR;
using SC.SenseTower.Admin.Dto.Users;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.UsersPage
{
    public class UsersPageRequest : BaseListRequest, IRequest<PagedDataDto<UserGridItemDto>>
    {
        public UsersPageFilter Filters { get; set; } = new();
    }
}
