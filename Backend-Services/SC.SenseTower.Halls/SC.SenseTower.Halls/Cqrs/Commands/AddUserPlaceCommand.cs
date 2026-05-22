using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class AddUserPlaceCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор холла.
        /// </summary>
        public Guid HallId { get; set; }

        /// <summary>
        /// Идентификатор помещения.
        /// </summary>
        public Guid PlaceId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
