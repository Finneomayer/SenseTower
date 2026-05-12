using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.Network.Scripts
{
    public class RoomUsersWatcher : NetworkBehaviour
    {
        private const float RoomUsersSendingDelay = 1f;

        private Dictionary<ulong, ClientInSpaceData> _roomClientsMap;
        private string _roomId;

        private IServerApiService _serverApiService;
        private IClientData _clientData;

        public bool IsInitialized { get; private set; }
        public string RoomId => _roomId;

        public event Action<ulong> UserConnectedToRoom;
        public event Action<List<string>> ConnectedUserIdListReceived;

        [Inject]
        public void Construct(IClientData clientData, IServerApiService serverApiService)
        {
            _clientData = clientData;
            _serverApiService = serverApiService;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _roomClientsMap = new();
                _roomId = DataExtensions.GetSpaceID();
                //_roomId = "E81E71B9-E685-40C7-9D4E-49F9A328C1DB"; // for testing in editor
                if (!string.IsNullOrEmpty(_roomId))
                {
                    TransmittingToSpaceServiceRoutine().Forget();
                }
                else
                {
                    Debug.LogWarning("Room IdKey environment variable is not assigned. " +
                        "ServerDataWatcher data transmission disabled.");
                }

                NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            }

            if (IsClient)
            {
                Guid? userId = _clientData.UserId;
                if (!userId.HasValue)
                {
                    Debug.LogWarning($"<color=red>{name}. ClientData.UserId == null</color>");
                    return;
                }

                SetUserDataServerRpc(NetworkManager.LocalClientId, userId.ToString(), Application.platform);
            }

            IsInitialized = true;
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }
        }

        public bool IsUserInRoom(string userGuid)
        {
            if (_roomClientsMap == null)
            {
                return false;
            }

            return _roomClientsMap.Values.FirstOrDefault(u => u.ClientId.ToString() == userGuid) != null;
        }

        public List<ulong> GetClientIdList()
        {
            return _roomClientsMap.Keys.ToList();
        }

        public string GetUserGuid(ulong clientId)
        {
            if (_roomClientsMap.TryGetValue(clientId, out ClientInSpaceData client))
            {
                return client.ClientId.ToString();
            }
            return null;
        }

        public bool TryGetClientId(string userGuid, out ulong clientId)
        {
            clientId = 0;
            foreach (var item in _roomClientsMap)
            {
                if (item.Value.ClientId.ToString() == userGuid)
                {
                    clientId = item.Key;
                    return true;
                }
            }
            return false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestConnectedUsersServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            SerializableUserList serializableClientList = new();
            if (_roomClientsMap == null)
            {
                SendConnectedUsersClientRpc(serializableClientList, clientRpcParams);
                return;
            }

            foreach (var item in _roomClientsMap)
            {
                if (NetworkManager.ConnectedClientsIds.Contains(item.Key))
                {
                    serializableClientList.UserIdList.Add(item.Value.ClientId.ToString());
                }
            }

            SendConnectedUsersClientRpc(serializableClientList, clientRpcParams);
        }

        [ClientRpc]
        public void SendConnectedUsersClientRpc(SerializableUserList clientList, ClientRpcParams clientRpcParams = default)
        {
            ConnectedUserIdListReceived?.Invoke(clientList.UserIdList);
        }

        private void OnClientDisconnectCallback(ulong disconnectedClientId)
        {
            _roomClientsMap?.Remove(disconnectedClientId);
        }

        private async UniTask TransmittingToSpaceServiceRoutine()
        {
            while (true)
            {
                RemoveDisconnectedUsers(_roomClientsMap);

                RegisterUsersInSpaceData usersInSpaceData = GetCurrentUsersInSpaceData();

                float requestDuration = 0;
                if (usersInSpaceData.ClientsData.Length > 0)
                {
                    float startRequestTime = Time.unscaledTime;

                    await SendToSpaceService(usersInSpaceData);

                    requestDuration = Time.unscaledTime - startRequestTime;
                }

                if (requestDuration < RoomUsersSendingDelay)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(RoomUsersSendingDelay - requestDuration));
                }
            }
        }

        private void RemoveDisconnectedUsers(Dictionary<ulong, ClientInSpaceData> clientUserIdMap)
        {
            if (clientUserIdMap == null)
            {
                return;
            }
            for (int i = clientUserIdMap.Count - 1; i >= 0; i--)
            {
                ulong clientId = clientUserIdMap.ElementAt(i).Key;
                if (!NetworkManager.ConnectedClientsIds.Contains(clientId))
                {
                    clientUserIdMap.Remove(clientId);
                }
            }
        }

        private RegisterUsersInSpaceData GetCurrentUsersInSpaceData()
        {
            RegisterUsersInSpaceData usersInSpaceData = new();

            usersInSpaceData.SpaceId = new Guid(_roomId);
            usersInSpaceData.ClientsData = _roomClientsMap.Values.ToArray();

            return usersInSpaceData;
        }

        private async UniTask SendToSpaceService(RegisterUsersInSpaceData usersInSpaceData)
        {
            await _serverApiService.SendServerUsers(usersInSpaceData);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetUserDataServerRpc(ulong clientId, string userGuid, RuntimePlatform platform)
        {
            if (NetworkManager.ConnectedClientsIds.Contains(clientId))
            {
                ClientInSpaceData clientInSpaceData = new ClientInSpaceData()
                {
                    ClientId = Guid.Parse(userGuid),
                    ClientType = platform == RuntimePlatform.Android ? ClientType.VrClient : ClientType.WinClient
                };
                _roomClientsMap[clientId] = clientInSpaceData;
                UserConnectedToRoom?.Invoke(clientId);
            }
        }
    }

    public class SerializableUserList : INetworkSerializable
    {
        public List<string> UserIdList = new();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                int count = UserIdList.Count;
                serializer.SerializeValue(ref count);
                foreach (var clientGuid in UserIdList)
                {
                    string userId = clientGuid;
                    serializer.SerializeValue(ref userId);
                }
            }
            else
            {
                UserIdList = new();
                int count = 0;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    string userId = null;
                    serializer.SerializeValue(ref userId);
                    UserIdList.Add(userId);
                }
            }
        }
    }
}