using MediatR;
using SC.SenseTower.Cinemas.Dto.Cinemas;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaList
{
    public class CinemaListRequest : IRequest<IEnumerable<CinemaDto>>
    {
    }
}
