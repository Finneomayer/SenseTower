using SC.SenseTower.Halls.Dto.Places;

namespace SC.SenseTower.Halls.Dto.Halls
{
    public class HallListItemDto
    {
        /// <summary>
        /// Идентификатор холла.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название холла.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Пространства в холле.
        /// </summary>
        public LocalSpaceDto[] Spaces { get; set; } = Array.Empty<LocalSpaceDto>();

        /// <summary>
        /// Пользовательские помещения в холле.
        /// </summary>
        public UserPlaceDto[] UserPlaces { get; set; } = Array.Empty<UserPlaceDto>();

        /// <summary>
        /// Публичные помещения в холле.
        /// </summary>
        public SpaceDto[] PublicPlaces { get; set; } = Array.Empty<SpaceDto>();

        /// <summary>
        /// Пространство, к которому привязан холл.
        /// </summary>
        public LocalSpaceDto? Space { get; set; }
    }
}
