using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.MyPlaces.Dto.Users
{
    public class UserPlaceDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int PlaceNumber { get; set; }

        public AccessType PublicAccessType { get; set; }

        public Guid? DoorImageId { get; set; }

        public string? DoorImageUrl { get; set; }

        public Guid? SpaceId { get; set; }

        public string? SpaceName { get; set; }
    }
}
