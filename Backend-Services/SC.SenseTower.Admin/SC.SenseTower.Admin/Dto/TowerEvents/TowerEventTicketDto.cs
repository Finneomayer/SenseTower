namespace SC.SenseTower.Admin.Dto.TowerEvents
{
    public class TowerEventTicketDto
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = null!;
    }
}
