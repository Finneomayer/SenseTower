using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class AddPublicPlaceCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор холла.
        /// </summary>
        public Guid HallId { get; set; }

        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid SpaceId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
