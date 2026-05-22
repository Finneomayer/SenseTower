using SC.SenseTower.Admin.Dto.Spaces;

namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryItemResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public InfoTableResponseDto InfoTable { get; set; } = new();

        public SpaceResponseDto Space { get; set; } = new();

        public int PicturesCounter { get; set; }
    }
}
