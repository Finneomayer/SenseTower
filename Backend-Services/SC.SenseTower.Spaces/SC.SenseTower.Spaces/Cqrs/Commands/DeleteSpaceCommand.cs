using MediatR;

namespace SC.SenseTower.Spaces.Cqrs.Commands
{
    public class DeleteSpaceCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
