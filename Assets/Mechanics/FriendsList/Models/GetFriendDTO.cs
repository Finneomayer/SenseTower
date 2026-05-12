using System;

namespace Assets.Mechanics.FriendsList.Models
{
    public class GetFriendDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int? AvatarNumber { get; set; }
        public bool Online { get; set; }
        public Guid SpaceId { get; set; }
        public string SpaceName { get; set; }

        public bool ConfirmationSended { get; set; }

        public bool IsEqual(GetFriendDTO other)
        {
            if (other == null)
            {
                return false;
            }

            if (UserId != other.UserId || UserName != other.UserName
                || Online != other.Online 
                || SpaceId != other.SpaceId || SpaceName != other.SpaceName
                || ConfirmationSended != other.ConfirmationSended)
            {
                return false;
            }

            if (AvatarNumber.HasValue != other.AvatarNumber.HasValue
                || (AvatarNumber.HasValue && other.AvatarNumber.HasValue && AvatarNumber.Value != other.AvatarNumber.Value))
            {
                return false;
            }

            return true;
        }
    }
}