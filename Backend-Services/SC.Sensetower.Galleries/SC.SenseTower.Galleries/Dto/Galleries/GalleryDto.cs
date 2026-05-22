using SC.SenseTower.Galleries.Dto.Spaces;

namespace SC.SenseTower.Galleries.Dto.Galleries
{
    public class GalleryDto
    {
        /// <summary>
        /// Идентификатор галереи.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название галереи.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Информационное табло галереи.
        /// </summary>
        public InfoTableDto GalleryInfoTable { get; set; } = new();

        /// <summary>
        /// Пространство, к которому привязана галерея.
        /// </summary>
        public SpaceDto Space { get; set; } = new();

        // Это есть только на беке, в юнити это ПОКА не нужно
        //public TowerPicture[] Pictures { get; set; }

        /// <summary>
        /// Коллекция картин в галерее.
        /// </summary>
        public Dictionary<int, GalleryImageDto> PicturesLocation { get; set; } = new();
    }
}
