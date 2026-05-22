using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Cqrs.TowerEventUpdate
{
    public class TowerEventUpdateCommand : ExternalRequestDto, IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public Guid? ImageId { get; set; }

        public Guid? SpaceId { get; set; }

        public TowerEventState State { get; set; }
    }
}
