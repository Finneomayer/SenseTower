using MediatR;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class PlacesByIdsRequest : IRequest<PlaceDto[]>
    {
        public Guid[] PlaceIds { get; set; } = Array.Empty<Guid>();

        public string? AccessToken { get; set; }
    }
}
