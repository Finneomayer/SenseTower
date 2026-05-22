using SC.SenseTower.Tickets.Dto.Spaces;

namespace SC.SenseTower.Tickets.Dto.TowerEvents
{
    public class TowerEventResponseDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public SpaceDto? Space { get; set; }
    }
}
