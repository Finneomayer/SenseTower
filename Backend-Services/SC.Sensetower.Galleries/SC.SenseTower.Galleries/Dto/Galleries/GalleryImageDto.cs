using SC.SenseTower.Galleries.Dto.Images;

namespace SC.SenseTower.Galleries.Dto.Galleries
{
    public class GalleryImageDto
    {
        /// <summary>
        /// Название изображения в галерее.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание изображения.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Автор изображения.
        /// </summary>
        public string Author { get; set; } = null!;

        /// <summary>
        /// Информация о файле изображения.
        /// </summary>
        public ImageInfoDto? Image { get; set; }

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
