using MediatR;
using SC.SenseTower.Spaces.Dto.Places;

namespace SC.SenseTower.Spaces.Cqrs.Requests
{
    public class PlacesRequest : IRequest<PlaceInfoDto[]?>
    {
        public Guid[] PlaceIds { get; set; } = Array.Empty<Guid>();

        public string? AccessToken { get; set; }
    }
}
