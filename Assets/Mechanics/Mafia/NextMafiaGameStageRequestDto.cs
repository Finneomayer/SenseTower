using System;
using System.Collections.Generic;

namespace Assets.Mechanics.Mafia
{
    public class NextMafiaGameStageRequestDto
    {
        public Dictionary<Guid, Guid> Votes { get; set; }
    }
}
