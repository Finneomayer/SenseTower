namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryAddImageDto
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!;

        public Guid? ImageId { get; set; }

        public decimal PictureWidthInMeters { get; set; }

        public decimal PassepartoutWidthInMeters { get; set; }
    }
}
