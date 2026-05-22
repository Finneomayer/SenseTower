using MediatR;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class LookupPlacesRequest : IRequest<IEnumerable<LookupItemDto<Guid>>?>
    {
        public Guid[] PlaceIds { get; set; } = Array.Empty<Guid>();
    }
}
