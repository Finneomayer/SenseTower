using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class AddSpaceCommand : IRequest<Unit>
    {
        public Guid HallId { get; set; }

        public Guid SpaceId { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
