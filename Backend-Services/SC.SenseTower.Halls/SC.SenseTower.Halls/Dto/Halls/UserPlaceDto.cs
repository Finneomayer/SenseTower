using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Halls.Dto.Halls
{
    public class UserPlaceDto
    {
        /// <summary>
        /// Идентификатор помещения.
        /// </summary>
        public Guid Id { get; set; } = default;

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
        /// Имя пользователя-владельца.
        /// </summary>
        public string? OwnerName { get; set; }

        /// <summary>
        /// Информация об изображении на двери помещения.
        /// </summary>
        public ImageDto? DoorImage { get; set; }

        /// <summary>
        /// Коллекция изображений в помещении.
        /// </summary>
        public Dictionary<int, ImageDto>? Images { get; set; }

        /// <summary>
        /// Пространство, к которому привязано помещение.
        /// </summary>
        public SpaceDto? LocalSpace { get; set; }

        /// <summary>
        /// Тип доступа в помещение.
        /// </summary>
        public AccessType PublicAccessType { get; set; }
    }
}
