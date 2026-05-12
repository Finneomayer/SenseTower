using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Assets.Scripts.Server;
using static Assets.Scripts.Server.ServerVerification;

namespace Assets.Mechanics.Mafia
{
    public class MafiaNetworkBehaviour : NetworkBehaviour
    {
        [SerializeField]
        private MafiaPlayersAutoKickBehaviour AutoKickBehaviour;

        private GameState _gameState;
        private MafiaEventMediatorServer _eventMediatorServer;
        private MafiaEventMediatorClient _eventMediatorClient;
        private string _localUserId;

        private ServerVerification _serverVerification;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnNetworkManagerClientConnectedCallback;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= OnNetworkManagerClientConnectedCallback;
            }
            base.OnNetworkDespawn();
        }

        public void Init(MafiaEventMediatorServer eventMediator, ServerVerification serverVerification)
        {
            _eventMediatorServer = eventMediator;
            _serverVerification = serverVerification;
            AutoKickBehaviour.Init(eventMediator, serverVerification);
        }

        public void InitClient(MafiaEventMediatorClient eventMediator, string localUserId)
        {
            _eventMediatorClient = eventMediator;
            _localUserId = localUserId;
            AutoKickBehaviour.InitClient(eventMediator);
        }

        public void StartGameClient(MafiaPlayerRole[] playerRolesPreset)
        {
            StartGameServerRpc(NetworkManager.Singleton.LocalClientId, playerRolesPreset);
        }

        public void CompleteGameClient()
        {
            CompleteGameServerRpc(_localUserId);
        }

        public void SetNextGameStageClient()
        {
            SetNextGameStageServerRpc(_localUserId);
        }

        public void SetState(GameState gameState)
        {
            _gameState = gameState;

            AutoKickBehaviour.SetState(gameState);

            // TODO: separate transmission for players because of their different visibility
            if (_gameState != null)
            {
                SetStateClientRpc(_gameState);
            }
            else
            {
                CompleteGameClientRpc();
            }           
        }

        public void SelectPlayerClient(int seatNumber)
        {
            SelectPlayerServerRpc(_localUserId, seatNumber);
        }

        public void RestoreGameForUserClient()
        {
            RestoreGameForUserServerRpc(_localUserId);
        }

        public void KickUserClient(Guid userGuid)
        {
            KickUserServerRpc(userGuid.ToString());
        }

        [ServerRpc(RequireOwnership = false)]
        private void KickUserServerRpc(string userGuid)
        {
            _eventMediatorServer.RequestKickUser(Guid.Parse(userGuid));
        }

        [ServerRpc(RequireOwnership = false)]
        private void RestoreGameForUserServerRpc(string playerGuid)
        {
            if (_gameState != null && !string.IsNullOrEmpty(playerGuid))
            {
                _eventMediatorServer.RequestPlayerReconnectToGame(Guid.Parse(playerGuid));
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(ulong clientId, MafiaPlayerRole[] playerRolesPreset)
        {
            _eventMediatorServer.RequestStartGame(clientId, playerRolesPreset);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CompleteGameServerRpc(string userId)
        {
            _eventMediatorServer.RequestCompleteGame();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetNextGameStageServerRpc(string userId)
        {
            GoToNextStageServer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">Who is requesting selection</param>
        /// <param name="seatNumber">Which place is selected</param>
        [ServerRpc(RequireOwnership = false)]
        private void SelectPlayerServerRpc(string userId, int seatNumber)
        {
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                return;
            }

            _gameState.TryGetPlayerStateByUserId(userId, out PlayerState requestingPlayerState);
            if (requestingPlayerState == null)
            {
                return;
            }

            if (!IsPlayerActionAvailable(userId, MafiaPlayerAction.Vote))
            {
                return;
            }

            PlayerState playerStateForSelect = _gameState.PlayerStates.FirstOrDefault((x) => x.Number == seatNumber);
            if (playerStateForSelect == null || !_gameState.CanSelectPlayer(requestingPlayerState, playerStateForSelect))
            {
                return;
            }

            //Who is requesting selection                             //Which place is selected
            if (requestingPlayerState.SelectedNumberOfOtherPlayer == playerStateForSelect.Number)
            {
                requestingPlayerState.SelectedNameOfOtherPlayer = string.Empty;
                requestingPlayerState.SelectedNumberOfOtherPlayer = PlayerState.DefaultSelectedNumber;
            }
            else
            {
                requestingPlayerState.SelectedNameOfOtherPlayer = playerStateForSelect.PlayerName;
                requestingPlayerState.SelectedNumberOfOtherPlayer = playerStateForSelect.Number;
            }

            foreach (var playerState in _gameState.PlayerStates)
            {
                playerState.VoteCount = GetVoteCount(playerState.Number);
                Debug.LogWarning($"Player id {playerState.PlayerId} vote count = {playerState.VoteCount}");
            }

            //var activePlayers = NetworkManager.ConnectedClientsIds.Where(
            //    x => IsClientActionAvailable(x, PlayerAction.Watch));

            //ClientRpcParams clientRpcParams = new()
            //{
            //    Send = new ClientRpcSendParams()
            //    {
            //        TargetClientIds = activePlayers.ToArray(),
            //    }
            //};

            SelectPlayerClientRpc(requestingPlayerState.Number, seatNumber);//, clientRpcParams);
        }

        [ClientRpc]
        private void ApprovePlayerClientRpc(ClientRpcParams clientRpcParams = default)
        {
            _eventMediatorClient.RequestRestoreGameForUser();
        }

        [ClientRpc]
        private void SetStateClientRpc(GameState newGameState, ClientRpcParams clientRpcParams = default)
        {
            _gameState = newGameState;
            _eventMediatorClient.RaiseNewClientGameStateReceived(_gameState);
        }

        [ClientRpc]
        private void CompleteGameClientRpc()
        {
            _gameState = null;
            _eventMediatorClient.RaiseNewClientGameStateReceived(_gameState);
        }

        [ClientRpc]
        private void SelectPlayerClientRpc(int seatSelector, int seatSelected, ClientRpcParams clientRpcParams = default)
        {
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                return;
            }

            // TODO: move checking to server side
            foreach (var playerState in _gameState.PlayerStates)
            {
                if (playerState.PlayerId == Guid.Parse(_localUserId) && !playerState.AvailableActions.Contains(MafiaPlayerAction.Watch))
                {
                    return;
                }
            }

            PlayerState selectorPlayerState = GetPlayerStateBySeatNumber(seatSelector);
            PlayerState selectedPlayerState = GetPlayerStateBySeatNumber(seatSelected);

            if (selectorPlayerState.SelectedNumberOfOtherPlayer == selectedPlayerState.Number)
            {
                selectorPlayerState.SelectedNameOfOtherPlayer = string.Empty;
                selectorPlayerState.SelectedNumberOfOtherPlayer = PlayerState.DefaultSelectedNumber;
            }
            else
            {
                selectorPlayerState.SelectedNameOfOtherPlayer = selectedPlayerState.PlayerName;
                selectorPlayerState.SelectedNumberOfOtherPlayer = selectedPlayerState.Number;
            }

            foreach (var playerState in _gameState.PlayerStates)
            {
                playerState.VoteCount = GetVoteCount(playerState.Number);
                Debug.Log($"Player id {playerState.PlayerId} vote count = {playerState.VoteCount}");
            }

            _eventMediatorClient.RaisePlayerStatesChanged();
        }

        private void GoToNextStageServer()
        {
            Dictionary<Guid, Guid> votes = new();
            foreach (var playerState in _gameState.PlayerStates)
            {
                if (playerState.Role == MafiaPlayerRole.GameMaster)
                {
                    continue;
                }
                if (playerState.SelectedNumberOfOtherPlayer == PlayerState.DefaultSelectedNumber)
                {
                    continue;
                }
                if (!_gameState.TryGetPlayerStateBySeatNumber(playerState.SelectedNumberOfOtherPlayer, out PlayerState selectedPlayer))
                {
                    continue;
                }

                votes[playerState.PlayerId] = selectedPlayer.PlayerId;
            }

            NextMafiaGameStageRequestDto sendingData = new NextMafiaGameStageRequestDto()
            {
                Votes = votes
            };
            _eventMediatorServer.RequestNextGameStage(sendingData);
        }

        private int GetVoteCount(int seatNumber)
        {
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                return 0;
            }

            return _gameState.PlayerStates.Count(x => x.SelectedNumberOfOtherPlayer == seatNumber && x.Role != MafiaPlayerRole.GameMaster);
        }

        private PlayerState GetPlayerStateBySeatNumber(int seatNumber)
        {
            if (_gameState == null || _gameState.PlayerStates == null)
            {
                return null;
            }
            return _gameState.PlayerStates.FirstOrDefault((x) => x.Number == seatNumber);
        }

        private bool IsPlayerActionAvailable(string userId, MafiaPlayerAction playerAction)
        {
            if (!_gameState.TryGetPlayerStateByUserId(userId, out PlayerState playerState))
            {
                return false;
            }

            return playerState.AvailableActions != null && playerState.AvailableActions.Contains(playerAction);
        }

        private void OnNetworkManagerClientConnectedCallback(ulong clientId)
        {
            TryApprovePlayerServer(clientId).Forget();
        }

        private async UniTask TryApprovePlayerServer(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId },
                }
            };

            if (_gameState == null)
            {
                return;
            }

            Guid playerGuid = Guid.Empty;
            // Waiting for user registration on server
            while (_gameState != null && NetworkManager != null && playerGuid == Guid.Empty)
            {
                await UniTask.Delay(500);
                ClientInfo clientInfo = _serverVerification.Users.FirstOrDefault(x => x.ServerId == clientId);
                if (clientInfo != null)
                {
                    playerGuid = Guid.Parse(clientInfo.Id);
                }
            }

            bool restoringGameApproved = _gameState != null && _gameState.PlayerStates != null && playerGuid != Guid.Empty
                && _gameState.PlayerStates.FirstOrDefault(x => x.PlayerId == playerGuid) != null;

            if (restoringGameApproved)
            {
                ApprovePlayerClientRpc(clientRpcParams);
            }
            else
            {
                if (_gameState != null)
                {
                    SetStateClientRpc(_gameState, clientRpcParams);
                }
            }
        }
    }
}
