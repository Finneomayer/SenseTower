using System;
using System.Collections.Generic;
using Assets.Mechanics.Mafia.UI;
using Mechanics.Mafia.Services;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class NetworkMafiaGameControllerService : NetworkBehaviour, IMafiaGameControllerService
    {
        #region Inspector

        #endregion
        //private MafiaAdminPanelViewPanel _mafiaAdminPanelView;
        //private OnPlayerUI _onPlayerUI;

        private MafiaGameStage _gameState;
        private NetworkList<ulong> _participants;
        private NetworkVariable<ulong> _adminId;

        private Dictionary<ulong, MafiaPlayerRole> _roleAndUserMapping = new();

        //public event Action GameStarted;
        //public event Action NextStageRequested;

        private void Awake()
        {
            _participants = new NetworkList<ulong>();
            _adminId = new NetworkVariable<ulong>();
            //_onPlayerUI = FindObjectOfType<OnPlayerUI>();
        }

        public override void OnNetworkSpawn()
        {
            _adminId.OnValueChanged += OnAdminChanged;
            _participants.OnListChanged += OnParticipantsChange;
        }

        private void OnDisable()
        {
            _participants.OnListChanged -= OnParticipantsChange;
            _adminId.OnValueChanged -= OnAdminChanged;
        }

        public void InitMafiaAdminPanel(MafiaAdminPanelViewPanel mafiaAdminPanel)
        {
            //_mafiaAdminPanelView = mafiaAdminPanel;
        }

        public ulong GetAdmin()
        {
            return _adminId.Value;
        }

        public void StartGame()
        {
            StartGameServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        public void SetAdmin(ulong clientId)
        {
            SetGameAdminServerRpc(clientId);
        }

        public void AddUser(ulong clientId)
        {
            AddUserServerRpc(clientId);
        }

        public void KickUser(ulong clientId)
        {
            OnKickUserServerRpc(NetworkManager.Singleton.LocalClientId, clientId);
        }

        public void CompleteGame()
        {
            CompleteGameServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        public void NextGameStage()
        {
            NextGameStageServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        #region Server

        [ServerRpc(RequireOwnership = false)]
        private void OnKickUserServerRpc(ulong senderId, ulong kickId)
        {
            if (!Validate(senderId))
                return;

            if (_participants.Contains(kickId))
                _participants.Remove(kickId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(ulong senderId)
        {
            if (!Validate(senderId))
                return;
            _gameState = MafiaGameStage.EverybodyIsSleeping;
            FillUsersRole();
            OnStartGameClientRpc(_gameState, GetReceiverId());

            //GameStarted?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddUserServerRpc(ulong clientId)
        {
            if (!_participants.Contains(clientId))
                _participants.Add(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetGameAdminServerRpc(ulong clientId)
        {
            _adminId.Value = clientId;
        }

        [ServerRpc(RequireOwnership = false)]
        private void NextGameStageServerRpc(ulong senderId)
        {
            if (!Validate(senderId))
                return;

            if (!_roleAndUserMapping.ContainsValue(MafiaPlayerRole.Mafia))
            {
                //_gameState = MafiaGameStage.CompleteGame;
                OnCompleteGameClientRpc(GetReceiverId());
            }
            else
            {
                //if (_gameState == MafiaGameStage.EverybodyIsSleeping)
                //{
                //    _gameState = MafiaGameStage.NightPlayerTurn;
                //}
                //else if(_gameState == MafiaGameStage.NightPlayerTurn)
                //{
                //    _gameState = MafiaGameStage.Voting;
                //}
                //else if (_gameState == MafiaGameStage.Voting)
                //{
                //    _gameState = MafiaGameStage.EverybodyIsSleeping;
                //}

                //OnNextGameStageClientRpc(_gameState,MafiaPlayerRole.Mafia, GetParamsByRole(MafiaPlayerRole.Mafia));
                //OnNextGameStageClientRpc(_gameState, MafiaPlayerRole.Peaceful,GetParamsByRole(MafiaPlayerRole.Peaceful));
            }

            //NextStageRequested?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void CompleteGameServerRpc(ulong senderId)
        {
            if (!Validate(senderId))
                return;

            OnCompleteGameClientRpc(GetReceiverId());
        }

        #endregion

        #region Client

        [ClientRpc]
        private void OnStartGameClientRpc(MafiaGameStage gameState, ClientRpcParams rpcParams = default)
        {
            OnStartGame();
        }

        [ClientRpc]
        private void OnNextGameStageClientRpc(MafiaGameStage gameState,MafiaPlayerRole role, ClientRpcParams rpcParams = default)
        {
            OnNextStage(gameState,role);
        }

        [ClientRpc]
        private void OnCompleteGameClientRpc(ClientRpcParams rpcParams = default)
        {
            OnCompleteGame();
        }

        #endregion

        private void OnCompleteGame()
        {
        }

        private void OnStartGame()
        {
            if (NetworkManager.Singleton.LocalClientId == _adminId.Value)
                return;

            if (!_participants.Contains(NetworkManager.Singleton.LocalClientId))
                return;

            //_onPlayerUI.FadeToBlackDefault();
            //GameStarted?.Invoke();
        }

        private void OnNextStage(MafiaGameStage gameState, MafiaPlayerRole role)
        {
            if (Validate(NetworkManager.Singleton.LocalClientId))
                return;
            
            //if (role == MafiaPlayerRole.Mafia && gameState == MafiaGameStage.NightPlayerTurn)
            //    _onPlayerUI.FadeToTransparent();
            //else if (role == MafiaPlayerRole.Peaceful && gameState == MafiaGameStage.NightPlayerTurn) 
            //    _onPlayerUI.FadeToBlackDefault();
            
            //if(gameState == MafiaGameStage.Voting)
            //    _onPlayerUI.FadeToTransparent();
        }

        private void OnParticipantsChange(NetworkListEvent<ulong> value)
        {
        }

        private void OnAdminChanged(ulong previousValue, ulong newValue)
        {
            //if (NetworkManager.Singleton.LocalClientId != newValue)
            //    _mafiaAdminPanelView.HidePanel();
        }

        private bool Validate(ulong senderId)
        {
            return _adminId.Value == senderId;
        }

        private ClientRpcParams GetReceiverId()
        {
            ClientRpcParams clientsRpcParams = new();
            ulong[] receivers = new ulong[_participants.Count];

            for (int i = 0; i < _participants.Count; i++)
            {
                receivers[i] = _participants[i];
            }

            clientsRpcParams.Send.TargetClientIds = receivers;
            return clientsRpcParams;
        }

        private void FillUsersRole()
        {
            //for (int i = 0; i < _participants.Count; i++)
            //{
            //    if (i % 2 == 0)
            //        _roleAndUserMapping[_participants[i]] = MafiaPlayerRole.Mafia;
            //    else
            //        _roleAndUserMapping[_participants[i]] = MafiaPlayerRole.Peaceful;
            //}
        }

        private ClientRpcParams GetParamsByRole(MafiaPlayerRole playerRole)
        {
            ClientRpcParams clientRpcParams = new();
            
            List<ulong> roleIds = new();
            foreach (KeyValuePair<ulong, MafiaPlayerRole> valuePair in _roleAndUserMapping)
            {
                if (valuePair.Value == playerRole)
                    roleIds.Add(valuePair.Key);
            }

            clientRpcParams.Send.TargetClientIds = roleIds;
            return clientRpcParams;
        }
    }
}