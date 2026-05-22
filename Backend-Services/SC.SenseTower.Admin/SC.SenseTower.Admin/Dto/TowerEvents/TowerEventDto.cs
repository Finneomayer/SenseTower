using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Dto.TowerEvents
{
    public class TowerEventDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public Guid? ImageId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public Guid? SpaceId { get; set; }

        public TowerEventState State { get; set; }

        public TicketInfoResponseDto[] SoldTickets { get; set; } = Array.Empty<TicketInfoResponseDto>();

        public int TotalTickets { get; set; }

        public IEnumerable<LookupItemDto<Guid>> AvailableSpaces { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        public IEnumerable<ImageInfoDto> AvailableImages { get; set; } = Array.Empty<ImageInfoDto>();
    }
}
