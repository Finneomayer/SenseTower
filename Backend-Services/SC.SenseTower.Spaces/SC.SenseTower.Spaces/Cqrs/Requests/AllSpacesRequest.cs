using MediatR;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Cqrs.Requests
{
    public class AllSpacesRequest : IRequest<IEnumerable<SpaceItemDto>>
    {
    }
}
