namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryUpdateRequestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsVisible { get; set; }

        public Guid? ImageId { get; set; }

        public Guid SpaceId { get; set; }
    }
}
