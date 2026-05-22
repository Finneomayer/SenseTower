using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class CreateHallCommand : IRequest<Guid>
    {
        /// <summary>
        /// Идентификатор холла (необязательно).
        /// </summary>
        public Guid Id { get; set; }

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
