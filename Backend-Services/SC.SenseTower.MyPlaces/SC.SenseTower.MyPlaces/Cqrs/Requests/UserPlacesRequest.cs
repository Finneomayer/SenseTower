using MediatR;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class UserPlacesRequest : IRequest<PlaceDto[]?>
    {
        public Guid UserId { get; set; }

        public string? AccessToken { get; set; }
    }
}
