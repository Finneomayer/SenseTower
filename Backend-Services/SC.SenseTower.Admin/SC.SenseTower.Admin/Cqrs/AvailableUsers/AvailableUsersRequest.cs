using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.AvailableUsers
{
    public class AvailableUsersRequest : ExternalRequestDto, IRequest<IEnumerable<LookupItemDto<Guid>>>
    {
        public Guid[]? UserIds { get; set; }

        public string? RoleName { get; set; }
    }
}
