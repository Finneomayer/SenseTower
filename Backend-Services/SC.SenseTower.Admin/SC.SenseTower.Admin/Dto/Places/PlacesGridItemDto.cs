using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Dto.Places
{
    public class PlacesGridItemDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Number { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerName { get; set; }

        public AccessType PublicAccessType { get; set; }

        public string? DoorImageUrl { get; set; }

        public Guid? SpaceId { get; set; }

        public string? SpaceName { get; set; }

        public int ImageCount { get; set; }
    }
}
