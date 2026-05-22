using MediatR;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Cqrs.Requests
{
    public class HallByPlaceIdRequest : IRequest<HallDto?>
    {
        public Guid PlaceId { get; set; }
    }
}
