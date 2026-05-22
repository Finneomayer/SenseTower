using MediatR;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaDelete
{
    public class CinemaDeleteCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid Id { get; set; }
    }
}
