using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Models.Galleries
{
    public class AddGalleryImageViewModel
    {
        public Guid GalleryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Author { get; set; } = null!;

        public Guid ImageId { get; set; }

        public IEnumerable<ImageInfoDto> AvailableImages { get; set; } = Array.Empty<ImageInfoDto>();

        public decimal PictureWidthInMeters { get; set; } = 1;

        public decimal PassepartoutWidthInMeters { get; set; } = 0;
    }
}
