using System;

namespace Assets.Scripts.Models
{
    public sealed class GetTokenResponse
    {
        // ReSharper disable once InconsistentNaming
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public bool IsGuest { get; set; }
        public string AccessToken { get; set; }
        public string RefeshToken { get; set; }
        public int? AvatarNumber { get; set; }
        public int? AvatarWatchNumber { get; set; }
        public int? OculusAvatarWatchNumber { get; set; }

        public int OwnedSpacesNumber { get; set; }
    }
}