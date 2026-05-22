using MediatR;

namespace SC.SenseTower.Spaces.State.RegisterUserInSpace
{
    public class RegisterUserInSpaceCommand : IRequest<Unit>
    {
        public Guid SpaceId { get; set; }

        public Guid UserId { get; set; }
    }
}
