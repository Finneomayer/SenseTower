namespace SC.SenseTower.Admin.Dto.TowerEvents
{
    public class TowerEventCreateRequestDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public Guid? ImageId { get; set; }

        public Guid? SpaceId { get; set; }

        public int? TicketQuantity { get; set; }
    }
}
