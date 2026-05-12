using System;
using System.Collections.Generic;

namespace Assets.Mechanics.Mafia
{
    public class MafiaGameStateDto
    {
        public string TableId { get; set; }

        public MafiaGameStage GameStage { get; set; }

        public MafiaGameStage NextGameStage { get; set; }

        public List<MafiaPlayerStateDto> PlayerStates { get; set; } = new();

        public int Day { get; set; }

        public int Turn { get; set; }

        public MafiaGameStatus GameStatus { get; set; } = MafiaGameStatus.InProgress;

        public Guid GameMasterId { get; set; }

        public List<Guid> VotingCandidates { get; set; } = new();
    }
}
