namespace SC.SenseTower.Galleries.Dto.Galleries
{
    public class GalleryImageSaveDto
    {
        /// <summary>
        /// Позиция изображения в галерее.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Название изображения.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание изображения.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Автор изображения.
        /// </summary>
        public string Author { get; set; } = null!;

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Ширина изображения в метрах для Unity.
        /// </summary>
        public decimal PictureWidthInMeters { get; set; }

        /// <summary>
        /// Ширина паспарту в метрах для Unity.
        /// </summary>
        public decimal PassepartoutWidthInMeters { get; set; }
    }
}
