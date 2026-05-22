namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryImageDto
    {
        public int Position { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Author { get; set; } = null!;

        public Guid ImageId { get; set; }

        public string ImageName { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public decimal PictureWidthInMeters { get; set; }

        public decimal PassepartoutWidthInMeters { get; set; }
    }
}
