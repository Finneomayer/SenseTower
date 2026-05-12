using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Client;
using Assets.Scripts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class MafiaGameStatePlayerUI : MonoBehaviour
    {
        private GameState _gameState;
        private OnPlayerUI _onPlayerUI;
        private string _localUserId;
        private MafiaRemotePlayerVisual[] _remotePlayers;
        private CompositionRootNetworkScene _compositionRootNetworkScene;
        private MafiaNightView _nightView;

        private void Awake()
        {
            _compositionRootNetworkScene = FindObjectOfType<CompositionRootNetworkScene>();       
        }

        public void Init(IClientData clientData, MafiaNightView mafiaNight)
        {
            _localUserId = clientData.UserId.Value.ToString();
            _nightView = mafiaNight;
        }

        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
            SetGameStateAsync(gameState).Forget();
        }

        private async UniTask SetGameStateAsync(GameState gameState)
        {
            if (_onPlayerUI == null)
            {
                PlayerLogic player = await _compositionRootNetworkScene.GetLocalPlayerAsync();
                _onPlayerUI = player.gameObject.GetComponentInChildren<OnPlayerUI>();
            }

            SetMasksOnOtherPlayersVisible(false);

            if (gameState == null)
            {
                _onPlayerUI.FadeToTransparent().Forget();
                RefreshByUsersSound(null, null);
                //SetDeadLabelsOnOtherPlayers(null);
                return;
            }

            if (!gameState.TryGetPlayerStateByUserId(_localUserId, out PlayerState playerState))
            {
                _onPlayerUI.FadeToTransparent().Forget();
                return;
            }

            //SetDeadLabelsOnOtherPlayers(gameState);
            SetMasksOnOtherPlayersVisible(true, gameState);

            if (playerState.Role == MafiaPlayerRole.GameMaster)
            {               
                _onPlayerUI.FadeToTransparent().Forget();
                return;
            }

            bool isSleeping = !playerState.AvailableActions.Contains(MafiaPlayerAction.Watch);
            //if (gameState.GameStage == MafiaGameStage.MafiaTurn || gameState.GameStage == MafiaGameStage.MafiaMeetEachOther)
            //{
            //    SetMasksOnOtherPlayersVisible(!isSleeping, gameState);
            //}
            
            if (isSleeping)
            {
                
                //_onPlayerUI.FadeToBlackDefault($"Вы спите. Ваша роль {roleName}.\n{roleDescription}").Forget();
                
                _nightView.SetData(playerState, gameState);
                _onPlayerUI.FadeToBlackCustomCanvas(_nightView.NightCanvas).Forget();
            }
            else
            {
                _onPlayerUI.FadeToTransparent().Forget();
            }
        }

        [ContextMenu("OnEditorFadeBlack")]
        public async void FadeBlackEditor()
        {
            PlayerLogic player = await _compositionRootNetworkScene.GetLocalPlayerAsync();
            _onPlayerUI = player.gameObject.GetComponentInChildren<OnPlayerUI>();
            _onPlayerUI.FadeToBlackCustomCanvas(_nightView.NightCanvas).Forget();
        }

        [ContextMenu("OnEditorFadeTransparent")]
        public async void FadeTransparentEditor()
        {
            PlayerLogic player = await _compositionRootNetworkScene.GetLocalPlayerAsync();
            _onPlayerUI = player.gameObject.GetComponentInChildren<OnPlayerUI>();
            _onPlayerUI.FadeToTransparent().Forget();
        }

        public void RefreshByUsersSound(IEnumerable<string> localPlayerListeners, IEnumerable<string> localPlayerTalkers)
        {
            _remotePlayers = FindObjectsOfType<MafiaRemotePlayerVisual>();
            if (_gameState == null || _gameState.PlayerStates == null || _gameState.GameStatus != MafiaGameStatus.InProgress
                || localPlayerListeners == null || localPlayerTalkers == null)
            {
                foreach (var player in _remotePlayers)
                {
                    player.SetHeadphonesVisible(false);
                    player.SetMouthMaskVisible(false);
                }
                return;
            }
            
            foreach (var player in _remotePlayers)
            {
                string playerGuid = player.GetPlayerId();
                player.SetHeadphonesVisible(!localPlayerListeners.Contains(playerGuid));
                player.SetMouthMaskVisible(!localPlayerTalkers.Contains(playerGuid));
            }
        }

        private void SetMasksOnOtherPlayersVisible(bool visible, GameState gameState = null)
        {
            _remotePlayers = FindObjectsOfType<MafiaRemotePlayerVisual>();

            if (visible == false)
            {
                foreach (var player in _remotePlayers)
                {
                    player.SetMaskVisible(false);
                }
            }
            else
            {
                if (gameState != null)
                {
                    foreach (var player in _remotePlayers)
                    {
                        if (gameState.TryGetPlayerStateByUserId(player.GetPlayerId(), out PlayerState playerState))
                        {
                            player.SetMaskVisible(!playerState.AvailableActions.Contains(MafiaPlayerAction.Watch));
                        }
                    }
                }
                else
                {
                    foreach (var player in _remotePlayers)
                    {
                        player.SetMaskVisible(false);
                    }
                }
            }
           
        }
    }
}
