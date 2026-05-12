using System;

namespace Assets.Scripts.Space
{
    public class UserInSpaceInfoDto
    {
        public Guid SpaceId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string SpaceName { get; set; }
    }
}
