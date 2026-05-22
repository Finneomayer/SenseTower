using MediatR;
using SC.SenseTower.Cinemas.Dto.Cinemas;

namespace SC.SenseTower.Cinemas.Cqrs.Cinema
{
    public class CinemaRequest : IRequest<CinemaDto>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid CinemaId { get; set; }
    }
}
