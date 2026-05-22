namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryGridItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsVisible { get; set; }

        public string SceneName { get; set; } = null!;

        public int PicturesCount { get; set; }
    }
}
