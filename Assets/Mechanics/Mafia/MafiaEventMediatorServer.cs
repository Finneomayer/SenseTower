using System;

namespace Assets.Mechanics.Mafia
{
    public class MafiaEventMediatorServer
    {
        public event Action<ulong, MafiaPlayerRole[]> StartGameRequested;
        public event Action CompleteGameRequested;
        public event Action<NextMafiaGameStageRequestDto> NextGameStageRequested;
        public event Action<GameState> GameStateChangeRequested;
        public event Action<Guid> PlayerReconnectToGameRequested;
        public event Action<Guid> KickUserRequested;

        public void RequestStartGame(ulong clientId, MafiaPlayerRole[] mafiaPlayerRoles)
        {
            StartGameRequested?.Invoke(clientId, mafiaPlayerRoles);
        }

        public void RequestCompleteGame()
        {
            CompleteGameRequested?.Invoke();
        }

        public void RequestNextGameStage(NextMafiaGameStageRequestDto sendingData)
        {
            NextGameStageRequested?.Invoke(sendingData);
        }

        public void RequestChangeGameState(GameState newGameState)
        {
            GameStateChangeRequested?.Invoke(newGameState);
        }

        public void RequestPlayerReconnectToGame(Guid playerGuid)
        {
            PlayerReconnectToGameRequested?.Invoke(playerGuid);
        }

        public void RequestKickUser(Guid userGuid)
        {
            KickUserRequested?.Invoke(userGuid);
        }
    }
}
