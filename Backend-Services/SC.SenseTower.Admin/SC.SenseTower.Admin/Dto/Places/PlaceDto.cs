using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Dto.Places
{
    public class PlaceDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Number { get; set; }

        public Guid? OwnerId { get; set; }

        public AccessType PublicAccessType { get; set; }

        public Guid? DoorImageId { get; set; }

        public string? DoorImageUrl { get; set; }

        public Guid? SpaceId { get; set; }

        public Dictionary<int, ImageInfoDto>? Images { get; set; }

        public IEnumerable<LookupItemDto<Guid>> AvailableUsers { get; set; } = Enumerable.Empty<LookupItemDto<Guid>>();

        public IEnumerable<ImageInfoDto> AvailableImages { get; set; } = Enumerable.Empty<ImageInfoDto>();

        public IEnumerable<LookupItemDto<Guid>> AvailableSpaces { get; set; } = Enumerable.Empty<LookupItemDto<Guid>>();
    }
}
