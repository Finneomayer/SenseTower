using MediatR;

namespace SC.SenseTower.Spaces.State.CheckIsUserImSpace
{
    public class CheckIsUserInSpaceRequest : IRequest<bool>
    {
        public Guid SpaceId { get; set; }

        public Guid UserId { get; set; }
    }
}
