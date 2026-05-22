using SC.SenseTower.Galleries.Dto.Images;

namespace SC.SenseTower.Galleries.Dto.Galleries
{
    public class InfoTableDto
    {
        /// <summary>
        /// Описание галереи.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Изображение на информационном табло.
        /// </summary>
        public ImageInfoDto Image { get; set; } = new();

        /// <summary>
        /// Признак показа информационного табло.
        /// </summary>
        public bool ShowInformation { get; set; }
    }
}
