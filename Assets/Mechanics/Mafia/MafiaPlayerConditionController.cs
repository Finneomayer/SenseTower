using System.Linq;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class MafiaPlayerConditionController
    {
        private Scripts.Shared.NetworkPlayer _networkPlayer;
        private string _localUserId;

        public void Init(Scripts.Shared.NetworkPlayer networkPlayer, string localUserId)
        {
            _networkPlayer = networkPlayer;
            _localUserId = localUserId;
        }

        public void SetGameState(GameState gameState)
        {
            if (_networkPlayer == null)
            {
                Debug.LogError("MafiaPlayerConditionController. _networkPlayer == null");
                return;
            }

            if (gameState == null)
            {
                _networkPlayer.SetVisibleToOthers(true);
                return;
            }
            
            if (!gameState.TryGetPlayerStateByUserId(_localUserId, out PlayerState playerState))
            {
                _networkPlayer.SetVisibleToOthers(true);
                return;
            }

            _networkPlayer.SetVisibleToOthers(playerState.IsAlive 
                || playerState.AvailableActions.Contains(MafiaPlayerAction.Talk));
        }
    }
}