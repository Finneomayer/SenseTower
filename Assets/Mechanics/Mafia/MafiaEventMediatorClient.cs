using System;

namespace Assets.Mechanics.Mafia
{
    public class MafiaEventMediatorClient
    {
        public event Action StartGameRequested;
        public event Action NextGameStageRequested;
        public event Action CompleteGameRequested;
        public event Action<int> SelectPlayerRequested;
        public event Action<GameState> NewClientGameStateReceived;
        public event Action PlayerStatesChanged;
        public event Action<int> SetTableMaxPlayerCountRequested;
        public event Action<int> PlayerRolePresetRequested;
        public event Action<MafiaPlayerRole[]> OnUpdatePreset;
        public event Action PlayersSoundChanged;
        public event Action RestoreGameForUserRequested;
        public event Action<Guid> KickUserRequested;
        public event Action<MafiaPlayersKickSerializedData> AutoKickDataRefreshRequested;

        public void RequestStartGame()
        {
            StartGameRequested?.Invoke();
        }

        public void RequestNextGameStage()
        {
            NextGameStageRequested?.Invoke();
        }

        public void RequestCompleteGame()
        {
            CompleteGameRequested?.Invoke();
        }

        public void RequestSelectPlayer(int seatNumber)
        {
            SelectPlayerRequested?.Invoke(seatNumber);
        }

        public void RaiseNewClientGameStateReceived(GameState newGameState)
        {
            NewClientGameStateReceived?.Invoke(newGameState);
        }

        public void RaisePlayerStatesChanged()
        {
            PlayerStatesChanged?.Invoke();
        }

        public void RequestSetTableMaxPlayerCount(int count)
        {
            SetTableMaxPlayerCountRequested?.Invoke(count);
        }

        public void RequestPlayerRolePreset(int count)
        {
            PlayerRolePresetRequested?.Invoke(count);
        }

        public void UpdateRolePreset(MafiaPlayerRole[] preset)
        {
            OnUpdatePreset?.Invoke(preset);
        }

        public void RequestPlayersSoundChanged()
        {
            PlayersSoundChanged?.Invoke();
        }

        public void RequestRestoreGameForUser()
        {
            RestoreGameForUserRequested?.Invoke();
        }

        public void RequestKickUser(Guid userGuid)
        {
            KickUserRequested?.Invoke(userGuid);
        }

        public void RequestRefreshAutoKickData(MafiaPlayersKickSerializedData data)
        {
            AutoKickDataRefreshRequested?.Invoke(data);
        }
    }
}
