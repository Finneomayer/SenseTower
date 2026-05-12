using Assets.Mechanics.Mafia.UI;

namespace Mechanics.Mafia.Services
{
    public interface IMafiaGameControllerService
    {
        public void InitMafiaAdminPanel(MafiaAdminPanelViewPanel mafiaAdminPanel);
        public void StartGame();
        public void SetAdmin(ulong clientId);
        public void AddUser(ulong clientId);
        public void KickUser(ulong clientId);
        public void CompleteGame();
        public void NextGameStage();
    }
}