using MediatR;

namespace SC.SenseTower.MyPlaces.Cqrs.Commands
{
    public class DeletePlaceCommand : IRequest<Unit>
    {
        public Guid PlaceId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
