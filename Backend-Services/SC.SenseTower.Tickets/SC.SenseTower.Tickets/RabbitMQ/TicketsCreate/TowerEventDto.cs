using SC.SenseTower.Common.Enums;
using SC.SenseTower.Tickets.Data.Models;
using SC.SenseTower.Tickets.Dto.Spaces;

namespace SC.SenseTower.Tickets.RabbitMQ.TicketsCreate
{
    public class TowerEventDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public ImageInfo? Image { get; set; }

        public SpaceDto? Space { get; set; }

        public TowerEventState State { get; set; } = TowerEventState.Planned;
    }
}
