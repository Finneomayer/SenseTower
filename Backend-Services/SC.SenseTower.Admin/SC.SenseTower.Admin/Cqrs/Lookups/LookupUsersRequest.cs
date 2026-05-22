using MediatR;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.Lookups
{
    public class LookupUsersRequest : IRequest<LookupItemDto<Guid>[]>
    {
        public string? Role { get; set; }

        public bool Eq { get; set; } = true;
    }
}
