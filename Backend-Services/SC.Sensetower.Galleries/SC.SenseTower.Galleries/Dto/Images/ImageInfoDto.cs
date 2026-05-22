namespace SC.SenseTower.Galleries.Dto.Images
{
    public class ImageInfoDto
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
