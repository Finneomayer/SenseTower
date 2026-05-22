namespace SC.SenseTower.TowerEvents.Dto.TowerEvents
{
    public class TicketDto
    {
        /// <summary>
        /// Идентификатор билета.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идкентификатор пользователя, владельца билета.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
