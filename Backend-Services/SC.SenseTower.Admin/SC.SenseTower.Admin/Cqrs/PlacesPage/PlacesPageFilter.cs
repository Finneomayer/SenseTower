using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.PlacesPage
{
    public class PlacesPageFilter
    {
        public string? PlaceName { get; set; }

        public Guid? OwnerId { get; set; }

        public IEnumerable<LookupItemDto<Guid>> AvailableUsers { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        public Guid? SpaceId { get; set; }

        public IEnumerable<LookupItemDto<Guid>> AvailableSpaces { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        public AccessType? PublicAccessType { get; set; }
    }
}
