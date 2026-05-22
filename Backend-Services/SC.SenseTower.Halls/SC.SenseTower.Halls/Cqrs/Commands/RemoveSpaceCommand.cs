using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class RemoveSpaceCommand : IRequest<Unit>
    {
        public Guid HallId { get; set; }

        public Guid SpaceId { get; set; }
    }
}
