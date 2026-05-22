using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.MyPlaces.Cqrs.Commands
{
    public class CreatePlaceCommand : IRequest<Guid>
    {
        /// <summary>
        /// Идентификатор помещения (необязательно).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название помещения.
        /// </summary>
        public string PlaceName { get; set; } = null!;

        /// <summary>
        /// Порядковый номер помещения.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// Идентификатор владельца помещения.
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Тип доступа в помещение.
        /// </summary>
        public AccessType? PublicAccessType { get; set; }

        /// <summary>
        /// Идентификатор изображения на двери.
        /// </summary>
        public Guid? DoorImageId { get; set; }

        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid? SpaceId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
