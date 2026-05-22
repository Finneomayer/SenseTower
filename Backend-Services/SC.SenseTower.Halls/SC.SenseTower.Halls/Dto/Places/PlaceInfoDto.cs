using SC.SenseTower.Common.Enums;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Dto.Places
{
    public class PlaceInfoDto
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
        public ImageDto DoorImage { get; set; } = new();

        /// <summary>
        /// Список изображений в помещении.
        /// </summary>
        public Dictionary<int, ImageDto>? Images { get; set; }

        /// <summary>
        /// Пространство, к которому привязано помещение.
        /// </summary>
        public LocalSpaceDto? LocalSpace { get; set; }
    }
}
