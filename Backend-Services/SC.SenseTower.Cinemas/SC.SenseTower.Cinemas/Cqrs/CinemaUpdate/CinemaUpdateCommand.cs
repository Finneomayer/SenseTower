using MediatR;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaUpdate
{
    public class CinemaUpdateCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название кинотеатра.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid SpaceId { get; set; }

        /// <summary>
        /// Токен доступа пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
