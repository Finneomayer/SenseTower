using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Models.Galleries
{
    public class GalleryImagesViewModel
    {
        public Guid GalleryId { get; set; }

        public IEnumerable<GalleryImageDto> Images { get; set; } = Array.Empty<GalleryImageDto>();

        public IEnumerable<ImageInfoDto> AvailableImages { get; set; } = Array.Empty<ImageInfoDto>();
    }
}
