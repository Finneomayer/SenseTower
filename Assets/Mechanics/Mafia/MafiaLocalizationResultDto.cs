using System.Collections.Generic;

namespace Assets.Mechanics.Mafia
{
    public class MafiaLocalizationResultDto
    {
        public Dictionary<MafiaPlayerRole, string> RoleNames { get; set; }
        public Dictionary<MafiaPlayerRole, string> RoleDescriptions { get; set; }
        public Dictionary<MafiaGameStage, string> StageNames { get; set; }
    }

    public class MafiaTicketCheckResultDto
    {
        public bool Result;
        public string Reason;
    }
}
