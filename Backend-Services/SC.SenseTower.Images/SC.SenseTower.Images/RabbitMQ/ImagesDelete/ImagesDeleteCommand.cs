using MediatR;

namespace SC.SenseTower.Images.RabbitMQ.ImagesDelete
{
    public class ImagesDeleteCommand : IRequest<Unit>
    {
        public Guid[] ImageIds { get; set; } = Array.Empty<Guid>();
    }
}
