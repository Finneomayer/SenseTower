using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate
{
    public class UserPlaceUpdateCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Number { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerName { get; set; }

        public AccessType PublicAccessType { get; set; }

        public ImageInfoDto DoorImage { get; set; } = new();

        public Dictionary<int, ImageInfoDto>? Images { get; set; }

        public SpaceInfoDto? LocalSpace { get; set; }
    }
}
