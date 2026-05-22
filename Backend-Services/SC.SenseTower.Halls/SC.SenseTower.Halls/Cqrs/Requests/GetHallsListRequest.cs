using MediatR;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Cqrs.Requests
{
    public class GetHallsListRequest : IRequest<HallListItemDto[]>
    {
        public Guid? UserId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
