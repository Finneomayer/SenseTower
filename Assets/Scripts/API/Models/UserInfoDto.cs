using System;

namespace Assets.Scripts.Models
{
    public class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? AvatarNumber { get; set; }
        public int AvatarWatchNumber { get; set; }
        public int OculusAvatarWatchNumber { get; set; }
        public bool IsGuest { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}