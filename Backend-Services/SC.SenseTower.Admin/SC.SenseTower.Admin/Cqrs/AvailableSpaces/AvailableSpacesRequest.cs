using MediatR;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.AvailableSpaces
{
    public class AvailableSpacesRequest : IRequest<IEnumerable<LookupItemDto<Guid>>>
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public SpaceType? SpaceType { get; set; }
    }
}
