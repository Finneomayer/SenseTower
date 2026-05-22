using MediatR;

namespace SC.SenseTower.Cinemas.Cqrs.AdminAdd
{
    public class AdminAddCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid CinemaId { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Токен доступа текущего пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
