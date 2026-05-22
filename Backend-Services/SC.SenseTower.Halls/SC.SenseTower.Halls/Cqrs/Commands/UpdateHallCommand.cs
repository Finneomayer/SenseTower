using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class UpdateHallCommand : IRequest<bool>
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
        /// Идентификатор пространства, к которому привязан холл.
        /// </summary>
        public Guid? SpaceId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
