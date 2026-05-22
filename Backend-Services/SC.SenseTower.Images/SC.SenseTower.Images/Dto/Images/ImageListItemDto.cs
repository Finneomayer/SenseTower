namespace SC.SenseTower.Images.Dto.Images
{
    public class ImageListItemDto
    {
        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название изображения.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Ссылка на оригинальное изображение.
        /// </summary>
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// Ссылка на уменьшенную копию изображения.
        /// </summary>
        public string PreviewUrl { get; set; } = null!;
    }
}
