using MediatR;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class AllocatedSpaceIdsRequest : IRequest<IEnumerable<Guid>>
    {
    }
}
