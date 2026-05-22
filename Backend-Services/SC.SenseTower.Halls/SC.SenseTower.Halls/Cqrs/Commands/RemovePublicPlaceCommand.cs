using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class RemovePublicPlaceCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор холла.
        /// </summary>
        public Guid HallId { get; set; }

        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid SpaceId { get; set; }
    }
}
