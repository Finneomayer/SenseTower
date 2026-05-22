using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class DeleteHallCommand : IRequest<bool>
    {
        public Guid HallId { get; set; }

        public Guid? UserId { get; set; }
    }
}
