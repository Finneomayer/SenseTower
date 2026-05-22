using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsVisible { get; set; }

        public Guid? ImageId { get; set; }

        public string? ImageUrl { get; set; }

        public Guid? SpaceId { get; set; }

        public IEnumerable<LookupItemDto<Guid>> AvailableSpaces { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        public IEnumerable<ImageInfoDto> AvailableImages { get; set; } = Array.Empty<ImageInfoDto>();
    }
}
