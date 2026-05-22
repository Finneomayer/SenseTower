using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Cqrs.PlaceCreate
{
    public class PlaceCreateCommand : ExternalRequestDto, IRequest<Guid?>
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public Guid? OwnerId { get; set; }

        public AccessType PublicAccessType { get; set; }

        public Guid? DoorImageId { get; set; }

        public Guid? SpaceId { get; set; }
    }
}
