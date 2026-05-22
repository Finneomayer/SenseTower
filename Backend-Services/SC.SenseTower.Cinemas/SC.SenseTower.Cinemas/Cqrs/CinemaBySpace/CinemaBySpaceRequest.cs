using MediatR;
using SC.SenseTower.Cinemas.Dto.Cinemas;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaBySpace
{
    public class CinemaBySpaceRequest : IRequest<CinemaDto?>
    {
        public Guid SpaceId { get; set; }
    }
}
