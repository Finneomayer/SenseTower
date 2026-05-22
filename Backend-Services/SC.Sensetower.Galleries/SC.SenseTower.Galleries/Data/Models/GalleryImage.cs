namespace SC.SenseTower.Galleries.Data.Models
{
    public class GalleryImage
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!;

        public ImageInfo Image { get; set; } = new();

        public decimal PictureWidthInMeters { get; set; }

        public decimal PassepartoutWidthInMeters { get; set; }
    }
}
