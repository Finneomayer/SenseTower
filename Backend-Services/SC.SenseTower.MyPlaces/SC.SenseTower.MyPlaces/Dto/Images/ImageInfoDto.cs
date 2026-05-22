namespace SC.SenseTower.MyPlaces.Dto.Images
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
        /// Сссылка на оригинал изображения.
        /// </summary>
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// Сссылка на превью изображения.
        /// </summary>
        public string PreviewUrl { get; set; } = null!;
    }
}
