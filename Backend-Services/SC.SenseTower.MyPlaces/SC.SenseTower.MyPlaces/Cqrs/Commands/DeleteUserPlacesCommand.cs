using MediatR;

namespace SC.SenseTower.MyPlaces.Cqrs.Commands
{
    public class DeleteUserPlacesCommand : IRequest<bool>
    {
        public Guid? OwnerId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
