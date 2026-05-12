using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Localization;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Server
{

    public class ServerVerification : NetworkBehaviour
    {
        public List<ClientInfo> Users { get { return _users; } private set { } }

        #region PrivateVariables

        [SerializeField]
        private List<ClientInfo> _users = new();
        private string _roomId = string.Empty;
            
        private IRegistrationInSpacesService _registrationInSpacesService;

        public event Action LocalClientAccessGranted;
        #endregion
       
        [Inject]
        private void Construct(IRegistrationInSpacesService registrationInSpacesService)
        {
            _registrationInSpacesService = registrationInSpacesService;
        }

        private void Init()
        {
            StartCoroutine(CheckUsersCoroutine());
        }
        
        public void AddUserToCheckTokenList(string uid, string login, ulong userServerId, string userToken, string lang)
        {
            AddUserToCheckTokenListServerRpc(uid, login, userServerId, userToken, lang);
        }
        
        #region Server
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                _roomId = DataExtensions.GetSpaceID();
                Init();
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += DisableUserToCheckTokenList;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += DisableUserToCheckTokenList;
            }
            base.OnNetworkDespawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeTokenOnUserListServerRpc(string uid, string token)
        {
            ClientInfo tempUser = _users.Find(element => element.Id == uid);
            
            if (tempUser != null && tempUser.CheckingTokenData != null)
            {
                tempUser.CheckingTokenData.Token = token;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddUserToCheckTokenListServerRpc(string userId, string login, ulong userServerId, string userToken, string lang)
        {
            CheckingTokenDto checkingTokenData = new()
            {
                UserId = Guid.Parse(userId),
                Token = userToken,
                Lang = lang,
                ServerRegistrationTime = DateTimeOffset.UtcNow,
            };

            ClientInfo tempClient = new ClientInfo
            {
                Id = userId,
                UserName = login,
                ServerId = userServerId,
                CheckingTokenData = checkingTokenData,
                LastActiveTime = Time.unscaledTime,
            };

            _users.Add(tempClient);
        }

        public void NotifyUserIsActive(ulong userServerId)
        {
            ClientInfo user =_users.FirstOrDefault((u) => u.ServerId == userServerId);
            if (user != null)
            {
                user.LastActiveTime = Time.unscaledTime;
            }
        }

        #endregion

        private async UniTaskVoid ValidConnectedClients()
        {
            var unts = new UniTaskCompletionSource();

            if (string.IsNullOrEmpty(_roomId))
            {
                Debug.LogError($"Space id {_roomId} is null. Check clients disable");
                await unts.Task;
                return;
            }

            CheckingTokenDto[] usersTokens = _users.Select(user => user.CheckingTokenData).ToArray();
            AccessResultDto[] usersAccesses = 
                await _registrationInSpacesService.CheckUsersAccessServer(_roomId, usersTokens);

            if (usersAccesses == null)
            {
                await unts.Task;
                return;
            }

            RemoveNotConnectedUsers();
            DisconnectNotValidUsers(usersAccesses);
            ApproveValidUsers(usersAccesses);

            await unts.Task;
        }

        [ClientRpc]
        private void ApproveAccessToRoomClientRpc(ClientRpcParams clientRpcParams = default)
        {
            LocalClientAccessGranted?.Invoke();
        }

        private void DisconnectUserByAccess(ClientInfo clientInfo, AccessResultDto userAccess)
        {
            if (clientInfo == null || userAccess == null)
            {
                return;
            }

            if (!NetworkManager.ConnectedClientsIds.Contains(clientInfo.ServerId))
            {
                return;
            }

            UserDisconnectData userDisconnectData = new()
            {
                DisconnectMessage = userAccess.Message,
                CanBeInSenseTower = userAccess.CanBeInSenseTower.HasValue
                    ? userAccess.CanBeInSenseTower.Value : true,
            };
            NetworkManager.DisconnectClientWithNotification(clientInfo.ServerId, userDisconnectData.Serialize());

            Debug.LogWarning($"Client disconnected by server: message = {userAccess.Message}, " +
                $"spaceId = {userAccess.SpaceId}, canBeHere = {userAccess.CanBeHere}, " +
                $"clientId = {clientInfo.ServerId}, userID = {clientInfo.Id}");
        }

        private void RemoveNotConnectedUsers()
        {
            for (int i = _users.Count - 1; i >= 0; i--)
            {
                if (!NetworkManager.ConnectedClientsIds.Contains(_users[i].ServerId))
                {
                    _users.RemoveAt(i);
                }
            }
        }

        private void ApproveValidUsers(AccessResultDto[] usersAccesses)
        {
            foreach (var userAccess in usersAccesses)
            {
                if (!userAccess.CanBeHere || !userAccess.UserId.HasValue)
                {
                    continue;
                }

                var serverUser = _users.FirstOrDefault(u => u.CheckingTokenData != null 
                    && u.CheckingTokenData.UserId == userAccess.UserId.Value);

                if (serverUser == null || serverUser.AccessApproved)
                {
                    continue;
                }

                serverUser.AccessApproved = true;

                if (NetworkManager.ConnectedClientsIds.Contains(serverUser.ServerId))
                {
                    ClientRpcParams clientRpcParams = new();
                    clientRpcParams.Send.TargetClientIds = new[] { serverUser.ServerId };
                    ApproveAccessToRoomClientRpc(clientRpcParams);
                }
            }
        }

        private void DisconnectNotValidUsers(AccessResultDto[] usersAccesses)
        {
            foreach (var userAccess in usersAccesses)
            {
                if (userAccess.CanBeHere || !userAccess.UserId.HasValue)
                {
                    continue;
                }
                for (int i = _users.Count - 1; i >= 0; i--)
                {
                    var serverUser = _users[i];
                    if (serverUser.CheckingTokenData == null)
                    {
                        continue;
                    }

                    if (serverUser.CheckingTokenData.UserId != userAccess.UserId.Value)
                    {
                        continue;
                    }

                    if (userAccess.ServerRegistrationTime.HasValue && serverUser.CheckingTokenData.ServerRegistrationTime.HasValue 
                        && serverUser.CheckingTokenData.ServerRegistrationTime.Value != userAccess.ServerRegistrationTime.Value)
                    {
                        continue;
                    }

                    _users.Remove(serverUser);
                    DisconnectUserByAccess(serverUser, userAccess);
                }
            }
        }

        private void DisableUserToCheckTokenList(ulong serverId)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (serverId == _users[i].ServerId)
                {
                    _users.RemoveAt(i);
                    break;
                }
            }
        }
        
        private IEnumerator CheckUsersCoroutine()
        {
            while (true)
            {
                if (_users.Count > 0)
                {
                    yield return ValidConnectedClients();
                }
              
                yield return new WaitForSeconds(1);
            }
        }
        
        #region InnerClass

        [Serializable]
        public class ClientInfo
        {
            [SerializeField] private string _id;
            [SerializeField] private string _userName;
            [SerializeField] private ulong _serverId;
            [SerializeField] private string _token;
            public string Id { get; set; }
            public string UserName { get; set; }
            public CheckingTokenDto CheckingTokenData { get; set; }
            public ulong ServerId { get; set; }
            public float LastActiveTime { get; set; }
            public bool AccessApproved { get; set; }
        }

        #endregion
    }
}