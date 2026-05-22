using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryImageResponseDto
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Author { get; set; } = null!;

        public ImageInfoDto? Image { get; set; }

        public decimal PictureWidthInMeters { get; set; }

        public decimal PassepartoutWidthInMeters { get; set; }
    }
}
