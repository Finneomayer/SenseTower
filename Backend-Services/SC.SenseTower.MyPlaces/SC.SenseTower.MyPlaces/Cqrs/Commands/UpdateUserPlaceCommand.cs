using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.MyPlaces.Cqrs.Commands
{
    public class UpdateUserPlaceCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор пользователя (заполняется сервером).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор помещения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип доступа в помещение.
        /// </summary>
        public AccessType PublicAccessType { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
