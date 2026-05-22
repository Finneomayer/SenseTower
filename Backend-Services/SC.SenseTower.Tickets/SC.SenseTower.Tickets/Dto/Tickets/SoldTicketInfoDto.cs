namespace SC.SenseTower.Tickets.Dto.Tickets
{
    public class SoldTicketInfoDto
    {
        /// <summary>
        /// Идентификатор билета.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, владельца билета.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя пользователя, владельца билета.
        /// </summary>
        public string UserName { get; set; } = null!;
    }
}
