using MediatR;

namespace SC.SenseTower.Spaces.Cqrs.Commands
{
    public class SetDoorImageCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }

        public Guid? ImageId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
