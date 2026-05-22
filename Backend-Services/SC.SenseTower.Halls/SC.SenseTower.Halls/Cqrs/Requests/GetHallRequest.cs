using MediatR;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Cqrs.Requests
{
    public class GetHallRequest : IRequest<HallDto?>
    {
        public Guid HallId { get; set; }

        public Guid? UserId { get; set; }

        public string? AccessToken { get; set; }
    }
}
