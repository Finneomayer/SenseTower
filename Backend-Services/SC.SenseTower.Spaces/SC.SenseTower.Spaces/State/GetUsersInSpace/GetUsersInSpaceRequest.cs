using MediatR;

namespace SC.SenseTower.Spaces.State.GetUsersInSpace
{
    public class GetUsersInSpaceRequest : IRequest<Guid[]>
    {
        public Guid SpaceId { get; set; }
    }
}
