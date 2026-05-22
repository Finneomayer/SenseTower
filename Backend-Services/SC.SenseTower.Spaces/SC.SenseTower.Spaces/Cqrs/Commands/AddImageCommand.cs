using MediatR;

namespace SC.SenseTower.Spaces.Cqrs.Commands
{
    public class AddImageCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid SpaceId { get; set; }

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Идентификатор текущего пользователя (заполняется сервером).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
