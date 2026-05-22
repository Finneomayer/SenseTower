using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Models.Places
{
    public class PlaceImagesViewModel
    {
        public Guid PlaceId { get; set; }

        public Dictionary<int, ImageInfoDto> Images { get; set; } = new();

        public IEnumerable<ImageInfoDto> AvailableImages { get; set; } = Array.Empty<ImageInfoDto>();
    }
}
