using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class ClearUserPlacesCommand : IRequest<bool>
    {
        public Guid[] PlaceIds { get; set; } = Array.Empty<Guid>();
    }
}
