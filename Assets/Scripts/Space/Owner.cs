using System;

namespace Assets.Scripts.Space
{
    [Serializable]
    public class Owner
    {
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? AvatarNumber { get; set; } = default;
        public int OwnedSpacesNumber { get; set; } = 0;
    }
}