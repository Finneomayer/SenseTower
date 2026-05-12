using System;

namespace Assets.Mechanics.Mafia
{
    public class KickMafiaPlayerRequestDto
    {
        public string TableId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
