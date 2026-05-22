using SC.SenseTower.Galleries.Dto.Spaces;

namespace SC.SenseTower.Galleries.Dto.Galleries
{
    public class GalleryItemDto
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
        /// Информационное табло.
        /// </summary>
        public InfoTableDto InfoTable { get; set; } = new();

        /// <summary>
        /// Пространство, к которому привязана галерея.
        /// </summary>
        public SpaceDto Space { get; set; } = new();

        /// <summary>
        /// Число изображений в галерее.
        /// </summary>
        public int PicturesCounter { get; set; }
    }
}
