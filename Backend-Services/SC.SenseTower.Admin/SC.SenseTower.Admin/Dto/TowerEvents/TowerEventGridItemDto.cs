using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Dto.TowerEvents
{
    public class TowerEventGridItemDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string? SpaceName { get; set; }

        public TowerEventState State { get; set; }

        public int TotalTickets { get; set; }

        public int Sold { get; set; }
    }
}
