using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class RemoveUserPlaceCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор холла.
        /// </summary>
        public Guid HallId { get; set; }

        /// <summary>
        /// Идентификатор помещения.
        /// </summary>
        public Guid PlaceId { get; set; }
    }
}
