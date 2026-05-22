using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Dto.Places
{
    public class PlaceResponseDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Number { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerName { get; set; }

        public AccessType PublicAccessType { get; set; }

        public ImageInfoDto DoorImage { get; set; } = new();

        public Dictionary<int, ImageInfoDto>? Images { get; set; }

        public LocalSpaceResponseDto? LocalSpace { get; set; }
    }
}
