using SC.SenseTower.Halls.Dto.Places;

namespace SC.SenseTower.Halls.Dto.Halls
{
    public class HallDto
    {
        /// <summary>
        /// Идентификатор холла.
        /// </summary>
        public Guid Id { get; set; } = default;

        /// <summary>
        /// Название холла.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Пространства холла.
        /// </summary>
        public LocalSpaceDto[] Spaces { get; set; } = Array.Empty<LocalSpaceDto>();

        /// <summary>
        /// Пользовательские помещения в холле.
        /// </summary>
        public UserPlaceDto[] UserPlaces { get; set; } = Array.Empty<UserPlaceDto>();

        /// <summary>
        /// Публичные пространства в холле.
        /// </summary>
        public SpaceDto[] PublicPlaces { get; set; } = Array.Empty<SpaceDto>();

        /// <summary>
        /// Пространство, к которому привязан холл.
        /// </summary>
        public LocalSpaceDto? LocalSpace { get; set; }
    }
}
