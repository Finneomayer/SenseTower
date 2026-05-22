using MediatR;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class PlaceBySpaceRequest : IRequest<PlaceDto?>
    {
        public Guid SpaceId { get; set; }
    }
}
