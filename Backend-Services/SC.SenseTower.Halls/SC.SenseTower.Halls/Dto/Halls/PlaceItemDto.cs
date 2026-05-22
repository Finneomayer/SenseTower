using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Halls.Dto.Halls
{
    public class PlaceItemDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int PlaceNumber { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerName { get; set; }

        public AccessType PublicAccessType { get; set; }

        public Guid? DoorImageId { get; set; }

        public string? DoorImageUrl { get; set; }

        public Guid? SpaceId { get; set; }

        public string? SpaceName { get; set; }
    }
}
