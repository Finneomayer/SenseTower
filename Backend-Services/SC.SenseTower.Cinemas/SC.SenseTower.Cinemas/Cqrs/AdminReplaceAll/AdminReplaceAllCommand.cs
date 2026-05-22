using MediatR;

namespace SC.SenseTower.Cinemas.Cqrs.AdminReplaceAll
{
    public class AdminReplaceAllCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid CinemaId { get; set; }

        /// <summary>
        /// Массив идентификаторов пользователей администраторов кинотеатра.
        /// </summary>
        public Guid[] UserIds { get; set; } = Array.Empty<Guid>();

        /// <summary>
        /// Токен текущего пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
