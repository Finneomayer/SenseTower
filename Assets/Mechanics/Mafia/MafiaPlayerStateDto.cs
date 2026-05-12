using System;
using System.Collections.Generic;

namespace Assets.Mechanics.Mafia
{
    [Serializable]
    public class MafiaPlayerStateDto
    {
        public Guid PlayerId;

        public MafiaPlayerRole Role;

        public MafiaPlayerAction[] MafiaPlayerActions;

        public bool IsAlive = true;

        public Dictionary<Effect, int> Effects { get; set; } = new();

        public bool IsDeadManLastWords { get; set; } = false;
    }
}
