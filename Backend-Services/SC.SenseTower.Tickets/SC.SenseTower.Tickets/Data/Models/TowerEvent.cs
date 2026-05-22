using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Tickets.Data.Models
{
    public class TowerEvent
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public Space? Space { get; set; }

        public TowerEventState State { get; set; }
    }
}
