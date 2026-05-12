using System;

namespace Assets.Scripts.Event
{
    public sealed class Ticket
    {
        public TowerEvent Event { get; set; }
        public Guid? UserId { get; set; }
    }
}