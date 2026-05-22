using MediatR;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Spaces.Cqrs.Requests
{
    public class LookupRequest : IRequest<IEnumerable<LookupItemDto<Guid>>>
    {
        public SpaceType? SpaceType { get; set; }
    }
}
