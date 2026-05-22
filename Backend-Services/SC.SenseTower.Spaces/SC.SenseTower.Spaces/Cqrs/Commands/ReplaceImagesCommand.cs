using MediatR;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Cqrs.Commands
{
    public class ReplaceImagesCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid SpaceId { get; set; }

        /// <summary>
        /// Коллекция изображений в помещении.
        /// </summary>
        public PlaceImageUpdateDto[] Images { get; set; } = Array.Empty<PlaceImageUpdateDto>();

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Идентификатор текущего пользователя (заполняется сервером).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Роль текущего пользователя (заполняется сервером).
        /// </summary>
        public string Role { get; set; } = null!;
    }
}
