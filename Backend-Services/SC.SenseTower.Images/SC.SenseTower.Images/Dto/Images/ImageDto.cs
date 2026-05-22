namespace SC.SenseTower.Images.Dto.Images
{
    public class ImageDto
    {
        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название изображения.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Ссылка на оригинал изображения.
        /// </summary>
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// Ссылка на уменьшенную копию изображения.
        /// </summary>
        public string PreviewUrl { get; set; } = null!;
    }
}
