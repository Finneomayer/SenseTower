using System;

namespace Assets.Scripts.Models
{
    public class AccessResultDto
    {
        public Guid? UserId { get; set; }

        public Guid? SpaceId { get; set; }

        public bool CanBeHere { get; set; }

        public string? Message { get; set; }

        public string? DoorMessage { get; set; }

        public bool? CanBeInSenseTower { get; set; }

        public DateTimeOffset? ServerRegistrationTime { get; set; }
    }
}