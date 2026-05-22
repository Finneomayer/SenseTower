using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Cqrs.PlaceUpdate
{
    public class PlaceUpdateCommand : ExternalRequestDto, IRequest<Unit>
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Number { get; set; }

        public Guid? OwnerId { get; set; }

        public AccessType PublicAccessType { get; set; }

        public Guid? DoorImageId { get; set; }

        public Guid? SpaceId { get; set; }
    }
}
