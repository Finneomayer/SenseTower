using MediatR;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class PlaceRequest : IRequest<PlaceDto?>
    {
        public Guid PlaceId { get; set; }

        public string? AccessToken { get; set; }
    }
}
