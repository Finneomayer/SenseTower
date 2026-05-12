using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Assets.Scripts.Audio;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Server;
using Assets.Mechanics.Network.Scripts;
using static Assets.Scripts.Server.ServerVerification;
using Assets.Scripts.Player;

namespace Assets.Mechanics.Mafia
{
    public class MafiaPlayersAudioSwitcher : NetworkBehaviour
    {
        public IEnumerable<string> LocalPlayerListeners => _localPlayerListeners;
        public IEnumerable<string> LocalPlayerTalkers => _localPlayerTalkers;

        private NetworkVariable<int> _tableNumberNetworkVariable = new(-1);

        private GameState _gameState;
        private IAudioService _audioService;

        private bool _isGameInProgress;
        private List<string> _gamePlayerGuids = new();

        private ServerVerification _serverVerification;

        private MafiaEventMediatorClient _eventMediatorClient;

        private HashSet<string> _localPlayerListeners = new HashSet<string>();
        private HashSet<string> _localPlayerTalkers = new HashSet<string>();
        private string _localPlayerId;
        private AgoraConnectionManager _agoraConnectionManager;

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListeners();
        }

        public void Init(ServerVerification serverVerification, int tableNumber)
        {
            _audioService = FindObjectOfType<AgoraAudioService>();
            _serverVerification = serverVerification;
            _tableNumberNetworkVariable.Value = tableNumber;
        }

        public void InitClient(MafiaEventMediatorClient eventMediatorClient, string localPlayerId)
        {
            UnregisterListeners();
            _audioService = FindObjectOfType<AgoraAudioService>();
            _eventMediatorClient = eventMediatorClient;
            _localPlayerId = localPlayerId;
            RegisterListeners();
        }

        public void SetGameState(GameState gameState)
        {
            if (gameState == null)
            {
                RestoreDefaultAudioChannelClient().Forget();
                return;
            }

            if (!gameState.TryGetPlayerStateByUserId(_localPlayerId.ToString(), out _))
            {
                return;
            }

            ConnectToGameAudioChannelClient().Forget();
        }

        public async UniTask ConnectToGameAudioChannelClient()
        {
            await InitAgoraConnectionManagerClient();

            if (_agoraConnectionManager != null)
            {
                await _agoraConnectionManager.SetCustomAudioChannel(GetTableAudioChannel());
            }
            else
            {
                Debug.LogError($"Cannot connect to mafia channel: _agoraConnectionManager == null");
            }
        }

        public async UniTask RestoreDefaultAudioChannelClient()
        {
            await InitAgoraConnectionManagerClient();
            if (_agoraConnectionManager != null)
            {
                await _agoraConnectionManager.SetDefaultAudioChannel();
            }
        }

        public void SetState(GameState gameState)
        {
            if (_audioService == null)
            {
                return;
            }
            _gameState = gameState;

            if (_gameState == null || _gameState.GameStatus != MafiaGameStatus.InProgress)
            {
                if (_isGameInProgress)
                {
                    ResetAudio();
                    _isGameInProgress = false;
                }
                return;
            }

            if (!_isGameInProgress)
            {
                FillGamePlayerGuids(_gameState);
                _isGameInProgress = true;
            }

            RefreshAudioForGameState();

            //if (_gameState.GameStage == MafiaGameStage.Voting
            //    || _gameState.GameStage == MafiaGameStage.VotingResultsDiscussion
            //    || _gameState.GameStage == MafiaGameStage.CommonDiscussion
            //    || _gameState.GameStage == MafiaGameStage.DeadManLastWords)
            //{
            //    EnableAudioForAll();
            //}
        }

        public void RegisterListeners()
        {
            UnregisterListeners();
            if (_audioService == null)
            {
                return;
            }

            _audioService.UserSoundEnabled += OnAudioServiceUserSoundEnabled;
            _audioService.UserSoundDisabled += OnAudioServiceUserSoundDisabled;
        }

        public void UnregisterListeners()
        {
            if (_audioService == null)
            {
                return;
            }
            _audioService.UserSoundEnabled -= OnAudioServiceUserSoundEnabled;
            _audioService.UserSoundDisabled -= OnAudioServiceUserSoundDisabled;
        }

        private string GetTableAudioChannel()
        {
            if (_tableNumberNetworkVariable.Value <= 0)
            {
                Debug.LogError("Cannot get mafia channel");
                return "";
            }

            return $"mafia-table-{_tableNumberNetworkVariable.Value}";
        }

        private async UniTask InitAgoraConnectionManagerClient()
        {
            if (_agoraConnectionManager == null)
            {
                CompositionRootNetworkScene compositionRootNetworkScene = FindObjectOfType<CompositionRootNetworkScene>();
                if (compositionRootNetworkScene != null)
                {
                    PlayerLogic player = await compositionRootNetworkScene.GetLocalPlayerAsync();
                    _agoraConnectionManager = player.gameObject.GetComponent<AgoraConnectionManager>();
                }
            }
        }

        private void ResetAudio()
        {
            EnableAudioForAll();
            _gamePlayerGuids.Clear();
        }

        private void FillGamePlayerGuids(GameState gameState)
        {
            if (gameState == null || gameState.PlayerStates == null)
            {
                return;
            }

            _gamePlayerGuids = gameState.PlayerStates.Select(x => x.PlayerId.ToString()).ToList();
        }

        private void EnableAudioForAll()
        {
            foreach (var playerGuid1 in _gamePlayerGuids)
            {
                foreach (var playerGuid2 in _gamePlayerGuids)
                {
                    if (playerGuid1 == playerGuid2)
                    {
                        continue;
                    }
                    _audioService.UnmuteUserForUserServer(playerGuid1, playerGuid2);
                }
            }
        }

        private void RefreshAudioForGameState()
        {
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                return;
            }

            foreach (var playerState in _gameState.PlayerStates)
            {
                if (playerState.Role == MafiaPlayerRole.GameMaster)
                {
                    SetAudioForGameMaster(playerState.PlayerId.ToString());
                }
                else if (playerState.AvailableActions.Contains(MafiaPlayerAction.Watch))
                {
                    SetAudioForWatcher(playerState.PlayerId.ToString());
                }
                else
                {
                    SetAudioForSleeper(playerState.PlayerId.ToString());
                }
            }
        }

        private void SetAudioForWatcher(string watcherPlayerGuid)
        {
            foreach (var playerState in _gameState.PlayerStates)
            {
                string playerGuid = playerState.PlayerId.ToString();
                if (playerGuid == watcherPlayerGuid)
                {
                    continue;
                }
                if (playerState.AvailableActions.Contains(MafiaPlayerAction.Talk))
                {
                    _audioService.UnmuteUserForUserServer(watcherPlayerGuid, playerGuid);
                }
                else
                {
                    _audioService.MuteUserForUserServer(watcherPlayerGuid, playerGuid);
                }
            }
        }

        private void SetAudioForSleeper(string sleeperPlayerGuid)
        {
            foreach (var playerState in _gameState.PlayerStates)
            {
                string playerGuid = playerState.PlayerId.ToString();
                if (playerGuid == sleeperPlayerGuid)
                {
                    continue;
                }
                if (playerState.Role == MafiaPlayerRole.GameMaster)
                {
                    _audioService.UnmuteUserForUserServer(sleeperPlayerGuid, playerGuid);
                }
                else
                {
                    _audioService.MuteUserForUserServer(sleeperPlayerGuid, playerGuid);
                }
            }
        }

        private void SetAudioForGameMaster(string gameMasterPlayerGuid)
        {
            foreach (var playerState in _gameState.PlayerStates)
            {
                string playerGuid = playerState.PlayerId.ToString();
                if (playerGuid == gameMasterPlayerGuid)
                {
                    continue;
                }
                if (playerState.IsAlive || playerState.AvailableActions.Contains(MafiaPlayerAction.Talk))
                {
                    _audioService.UnmuteUserForUserServer(gameMasterPlayerGuid, playerGuid);
                }
                else
                {
                    _audioService.MuteUserForUserServer(gameMasterPlayerGuid, playerGuid);
                }
            }
        }

        private void OnAudioServiceUserSoundEnabled(string userGuid)
        {
            _localPlayerTalkers.Add(userGuid);
            _eventMediatorClient.RequestPlayersSoundChanged();
            SendUserSoundSwitchedServerRpc(_localPlayerId, userGuid, true);
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                Debug.LogWarning($"Enabled sound for user {userGuid}");
                return;
            }

            PlayerState player = _gameState.PlayerStates.FirstOrDefault(x => x.PlayerId.ToString() == userGuid);
            if (player != null)
            {
                Debug.LogWarning($"Enabled sound for user {player.Number}{player.PlayerName} ({player.PlayerId})");
            }
        }

        private void OnAudioServiceUserSoundDisabled(string userGuid)
        {
            _localPlayerTalkers.Remove(userGuid);
            _eventMediatorClient.RequestPlayersSoundChanged();
            SendUserSoundSwitchedServerRpc(_localPlayerId, userGuid, false);
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                Debug.LogWarning($"Disabled sound for user {userGuid}");
                return;
            }

            PlayerState player = _gameState.PlayerStates.FirstOrDefault(x => x.PlayerId.ToString() == userGuid);
            if (player != null)
            {
                Debug.LogWarning($"Disabled sound for user {player.Number}{player.PlayerName} ({player.PlayerId})");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendUserSoundSwitchedServerRpc(string senderUserGuid, string changedSoundUserGuid, bool isSoundEnabled)
        {
            ClientInfo clientInfo = _serverVerification.Users.FirstOrDefault(x => x.Id == changedSoundUserGuid);

            if (clientInfo == null)
            {
                return;
            }

            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientInfo.ServerId },
                }
            };

            SendUserSoundSwitchedConfirmationClientRpc(senderUserGuid, isSoundEnabled, clientRpcParams);
        }

        [ClientRpc]
        private void SendUserSoundSwitchedConfirmationClientRpc(string confirmingUserGuid, bool isSoundEnabled,
            ClientRpcParams clientRpcParams = default)
        {
            if (isSoundEnabled)
            {
                _localPlayerListeners.Add(confirmingUserGuid);
            }
            else
            {
                _localPlayerListeners.Remove(confirmingUserGuid);
            }

            _eventMediatorClient.RequestPlayersSoundChanged();
        }
    }
}
