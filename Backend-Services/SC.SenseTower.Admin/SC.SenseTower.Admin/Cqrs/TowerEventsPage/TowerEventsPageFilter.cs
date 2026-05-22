using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.TowerEventsPage
{
    public class TowerEventsPageFilter
    {
        public TowerEventState? State { get; set; }

        public string? Title { get; set; }

        public DateTimeOffset? From { get; set; }

        public DateTimeOffset? UpTo { get; set; }

        public Guid? SpaceId { get; set; }

        public IEnumerable<LookupItemDto<Guid>> AvailableSpaces { get; set; } = Array.Empty<LookupItemDto<Guid>>();
    }
}
