using MediatR;

namespace SC.SenseTower.Cinemas.Cqrs.AdminDelete
{
    public class AdminDeleteCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid CinemaId { get; set; }

        /// <summary>
        /// Идентификатор администратора кинотеатра.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
