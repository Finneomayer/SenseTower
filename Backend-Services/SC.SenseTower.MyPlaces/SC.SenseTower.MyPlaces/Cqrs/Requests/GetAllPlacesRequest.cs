using MediatR;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class GetAllPlacesRequest : IRequest<PlaceDto[]>
    {
        public string? AccessToken { get; set; }
    }
}
