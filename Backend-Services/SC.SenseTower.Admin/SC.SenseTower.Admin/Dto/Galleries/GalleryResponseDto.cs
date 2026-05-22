using SC.SenseTower.Admin.Dto.Spaces;

namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class GalleryResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public InfoTableResponseDto GalleryInfoTable { get; set; } = new();

        public SpaceResponseDto Space { get; set; } = new();

        // Это есть только на беке, в юнити это ПОКА не нужно
        //public TowerPicture[] Pictures { get; set; }

        public Dictionary<int, GalleryImageResponseDto> PicturesLocation { get; set; } = new();
    }
}
