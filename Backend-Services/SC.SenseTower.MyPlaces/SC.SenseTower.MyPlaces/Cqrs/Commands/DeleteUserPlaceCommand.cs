using MediatR;

namespace SC.SenseTower.MyPlaces.Cqrs.Commands
{
    public class DeleteUserPlaceCommand : IRequest<bool>
    {
        public Guid PlaceId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
