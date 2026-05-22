using MediatR;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.Lookups
{
    public class LookupRolesRequest : IRequest<LookupItemDto<Guid>[]>
    {
    }
}
