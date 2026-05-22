using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Dto.Places
{
    public class PlaceSaveDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int PlaceNumber { get; set; }

        public Guid? OwnerId { get; set; }

        public AccessType PublicAccessType { get; set; }

        public Guid? DoorImageId { get; set; }

        public Guid? SpaceId { get; set; }
    }
}
