using SC.SenseTower.Common.Enums;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Dto.Places
{
    public class PlaceResponseDto
    {
        /// <summary>
        /// Идентификатор помещения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название помещения.
        /// </summary>
        public string PlaceName { get; set; } = null!;

        /// <summary>
        /// Номер помещения.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Идентификатор пользователя-владельца.
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Имя владельца.
        /// </summary>
        public string? OwnerName { get; set; }

        /// <summary>
        /// Тип доступа в помещение.
        /// </summary>
        public AccessType PublicAccessType { get; set; }

        /// <summary>
        /// Информация об изображении на двери.
        /// </summary>
        public ImageInfoDto DoorImage { get; set; } = new();

        /// <summary>
        /// Список изображений в помещении.
        /// </summary>
        public Dictionary<int, ImageInfoDto>? Images { get; set; }
    }
}
