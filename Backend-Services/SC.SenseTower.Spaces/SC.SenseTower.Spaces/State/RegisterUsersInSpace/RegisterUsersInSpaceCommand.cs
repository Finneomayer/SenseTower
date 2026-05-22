using MediatR;

namespace SC.SenseTower.Spaces.State.RegisterUsersInSpace
{
    public class RegisterUsersInSpaceCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }

        public Guid[] UserIds { get; set; } = Array.Empty<Guid>();
    }
}
