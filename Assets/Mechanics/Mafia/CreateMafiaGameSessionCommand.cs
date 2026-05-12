using System;
using System.Collections.Generic;

namespace Assets.Mechanics.Mafia
{
    public class CreateMafiaGameSessionCommand
    {
        public string TableId { get; set; }

        public bool IsAutoGenerateRoles { get; set; }

        public List<Guid> Players { get; set; }

        public List<MafiaPlayerRole> UserRolesPreset { get; set; }

        public Guid GameMasterUserId { get; set; }
    }
}
