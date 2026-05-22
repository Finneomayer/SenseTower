using SC.SenseTower.Tickets.Dto.TowerEvents;

namespace SC.SenseTower.Tickets.Dto.Tickets
{
    public class TicketDto
    {
        /// <summary>
        /// Идентификатор билета.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, владельца билета.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Событие, для которого выпущен билет.
        /// </summary>
        public TowerEventDto TowerEvent { get; set; } = new();
    }
}
