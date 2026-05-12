using Cysharp.Threading.Tasks;
using System;
using Assets.Scripts.Client;
using Assets.Scripts.Server;

namespace Assets.Mechanics.Mafia
{
    public interface IMafiaGameService
    {
        public UniTask<MafiaGameStateDto> StartNewGame(string tableId, string adminId, string[] playerIds, MafiaPlayerRole[] userRolesPreset);
        public UniTask<bool> CompleteGame(string tableId);
        public UniTask<MafiaGameStateDto> GetCurrentState(string tableId);
        public UniTask<MafiaGameStateDto> GoToNextStage(string tableId, NextMafiaGameStageRequestDto sendingData);
        public UniTask<MafiaGameStateDto> KickPlayer(string tableId, Guid playerId);
        public UniTask<MafiaPlayerRole[]> GetPlayerRolesPresetClient(int playerCount);
        public UniTask<MafiaLocalizationResultDto> GetLocalizationContentClient();
        public UniTask<MafiaTicketCheckResultDto> CheckMafiaTicketClient(string tableId);
        public void Init(IServerApiData serverApiData);
        public void InitAsClient(IClientData clientData);
    }
}
