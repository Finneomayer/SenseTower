using MediatR;

namespace SC.SenseTower.Halls.RabbitMQ.PlacesDelete
{
    public class PlacesDeleteCommand : IRequest<Unit>
    {
        public Guid[] PlaceIds { get; set; } = Array.Empty<Guid>();
    }
}
