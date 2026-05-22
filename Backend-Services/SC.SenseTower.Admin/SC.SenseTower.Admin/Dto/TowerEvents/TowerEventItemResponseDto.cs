using SC.SenseTower.Admin.Dto.Spaces;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Dto.TowerEvents
{
    public class TowerEventItemResponseDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public SpaceResponseDto? Space { get; set; }

        public TowerEventState State { get; set; }

        public TicketInfoResponseDto[] SoldTickets { get; set; } = Array.Empty<TicketInfoResponseDto>();

        public int TotalTickets { get; set; }
    }
}
