using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.FriendsList;
using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Client;
using Mechanics.SignalBusModels;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Mechanics.FriendsList
{

    public class NetworkFriendListService : NetworkBehaviour
    {
        #region Inspector

        [SerializeField] private ulong ReceiverId;

        #endregion
        private List<AddFriendRequest> _makeFriendRequests = new();

        private SignalBus _signalBus;
        private IClientData _clientData;
        private IFriendsService _friendService;

        [Inject]
        private void Construct(IClientData clientData, SignalBus signalBus, IFriendsService friendsService)
        {
            _friendService = friendsService;
            _clientData = clientData;
            _signalBus = signalBus;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }
            base.OnNetworkDespawn();
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            foreach (var item in _makeFriendRequests)
            {
                if (item.SenderUserId == clientId)
                {
                    DeleteFriendRequestClientRpc(item.SenderUserGuid, item.ReceiverUserGuid);
                }
                if (item.ReceiverUserId == clientId)
                {
                    DeleteFriendRequestClientRpc(item.SenderUserGuid, item.ReceiverUserGuid);
                }
            }

            _makeFriendRequests.RemoveAll(r => r.SenderUserId == clientId || r.ReceiverUserId == clientId);
        }

        [ContextMenu("Make Friend From Editor")]
        public void MakeFriendFromEditor()
        {
            TryMakeFriend(ReceiverId);
        }

        public void TryMakeFriend(ulong receiverUserId)
        {
            AddFriendRequest addFriendRequest = new();
            
            addFriendRequest.SenderUserId = NetworkManager.Singleton.LocalClientId;
            addFriendRequest.SenderUserGuid = _clientData.UserId.ToString();
            addFriendRequest.SenderUserName = _clientData.UserName;
            addFriendRequest.SenderUserToken = _clientData.AccessToken;
            addFriendRequest.SenderAvatarId = AvatarSessionData.AvatarAssetId.HasValue? AvatarSessionData.AvatarAssetId.Value : 0;

            addFriendRequest.ReceiverUserId = receiverUserId;
            addFriendRequest.ReceiverUserGuid = string.Empty;
            addFriendRequest.ReceiveUserName = string.Empty;
            addFriendRequest.ReceiverUserToken = string.Empty;
            addFriendRequest.ReceiverAvatarId = 0;
            
            TryFillMakeFriendRequestServerRpc(addFriendRequest);
        }

        #region Server

        [ServerRpc(RequireOwnership = false)]
        private void TryFillMakeFriendRequestServerRpc(AddFriendRequest addFriendRequest)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] {addFriendRequest.ReceiverUserId};

            ReceiveMakeFriendRequestClientRpc(addFriendRequest, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetReceiverAnswerOnRequestServerRpc(AddFriendRequest addFriendRequest)
        {
            foreach (var item in _makeFriendRequests)
            {
                if (item.SenderUserGuid == addFriendRequest.SenderUserGuid && item.ReceiverUserGuid == addFriendRequest.ReceiverUserGuid
                    || item.ReceiverUserGuid == addFriendRequest.SenderUserGuid && item.SenderUserGuid == addFriendRequest.ReceiverUserGuid)
                {
                    return;
                }
            }

            _makeFriendRequests.Add(addFriendRequest);

            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] { addFriendRequest.SenderUserId, addFriendRequest.ReceiverUserId };

            FillingReceiverRequestFriendDataClientRpc(addFriendRequest, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TryMakeTwoUsersFriendServerRpc(string confirmedUserId, string friendingUserId)
        {
            AddFriendRequest currentRequest = null;
            foreach (AddFriendRequest makeFriendRequest in _makeFriendRequests)
            {
                if (makeFriendRequest.SenderUserGuid == confirmedUserId && makeFriendRequest.ReceiverUserGuid == friendingUserId)
                {
                    makeFriendRequest.SenderConfirmed = true;
                    currentRequest = makeFriendRequest;
                    break;
                }
                else if (makeFriendRequest.ReceiverUserGuid == confirmedUserId && makeFriendRequest.SenderUserGuid == friendingUserId)
                {
                    makeFriendRequest.ReceiverConfirmed = true;
                    currentRequest = makeFriendRequest;
                    break;
                }
            }

            if (currentRequest == null)
            {
                return;
            }

            if (!currentRequest.SenderConfirmed || !currentRequest.ReceiverConfirmed)
            {
                return;
            }

            MakeFriend(currentRequest);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DeleteFriendServerRpc(string senderUserId, string receiverUserId)
        {
            DeleteFriendClientRpc(senderUserId, receiverUserId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DeleteFriendRequestServerRpc(string senderUserId, string receiverUserId)
        {
            foreach (var r in _makeFriendRequests)
            {
                if (r.SenderUserGuid == senderUserId && r.ReceiverUserGuid == receiverUserId
                    || r.ReceiverUserGuid == senderUserId && r.SenderUserGuid == receiverUserId)
                {
                    DeleteFriendRequestServer(r.SenderUserId, r.ReceiverUserId, senderUserId, receiverUserId);
                    break;
                }
            }

            _makeFriendRequests.RemoveAll(r => r.SenderUserGuid == senderUserId && r.ReceiverUserGuid == receiverUserId
                || r.ReceiverUserGuid == senderUserId && r.SenderUserGuid == receiverUserId);
        }

        private void DeleteFriendRequestServer(ulong senderClientId, ulong receiverClientId, string senderUserId, string receiverUserId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds =
            new[] { senderClientId, receiverClientId };

            DeleteFriendRequestClientRpc(senderUserId, receiverUserId, clientRpcParams);
        }

        // Invoke on Server
        private async void MakeFriend(AddFriendRequest addFriendRequest)
        {
            if (string.IsNullOrEmpty(addFriendRequest.SenderUserToken) && string.IsNullOrEmpty(addFriendRequest.ReceiverUserToken))
                return;
            
            bool friendingResult = 
                await _friendService.MakeTwoUsersFriendServer(addFriendRequest.SenderUserToken, addFriendRequest.ReceiverUserToken);

            if (friendingResult)
            {
                DeleteFriendRequestServer(addFriendRequest.SenderUserId, addFriendRequest.ReceiverUserId,
                    addFriendRequest.SenderUserGuid, addFriendRequest.ReceiverUserGuid);
                _makeFriendRequests.Remove(addFriendRequest);
            }
        }

        #endregion

        #region Client

        [ClientRpc]
        private void ReceiveMakeFriendRequestClientRpc(AddFriendRequest addFriendRequest,
            ClientRpcParams clientRpcParams = default)
        {
            addFriendRequest.ReceiverUserGuid = _clientData.UserId.ToString();
            addFriendRequest.ReceiveUserName = _clientData.UserName;
            addFriendRequest.ReceiverAvatarId = AvatarSessionData.AvatarAssetId.HasValue? AvatarSessionData.AvatarAssetId.Value : 0;
            addFriendRequest.ReceiverUserToken = _clientData.AccessToken;

            GetReceiverAnswerOnRequestServerRpc(addFriendRequest);
        }

        [ClientRpc]
        private void FillingReceiverRequestFriendDataClientRpc(AddFriendRequest addFriendRequest,
            ClientRpcParams clientRpcParams = default)
        {
            AddToFriendListRequestSignal addToFriendListRequestSignal = new();

            if (NetworkManager.Singleton.LocalClientId == addFriendRequest.SenderUserId)
            {
                addToFriendListRequestSignal.AvatarId = addFriendRequest.ReceiverAvatarId;
                addToFriendListRequestSignal.UserName = addFriendRequest.ReceiveUserName;
                addToFriendListRequestSignal.UserId = addFriendRequest.ReceiverUserGuid;
            }
            else if (NetworkManager.Singleton.LocalClientId == addFriendRequest.ReceiverUserId)
            {
                addToFriendListRequestSignal.AvatarId = addFriendRequest.SenderAvatarId;
                addToFriendListRequestSignal.UserName = addFriendRequest.SenderUserName;
                addToFriendListRequestSignal.UserId = addFriendRequest.SenderUserGuid;
            }

            addToFriendListRequestSignal.SenderId = addFriendRequest.SenderUserId;
            addToFriendListRequestSignal.ReceiverId = addFriendRequest.ReceiverUserId;
            _signalBus.Fire(addToFriendListRequestSignal);
        }

        [ClientRpc]
        private void DeleteFriendRequestClientRpc(string senderId, string receiverId, ClientRpcParams clientRpcParams = default)
        {
            RemoveFromFriendListRequestSignal removeFromFriendListRequestSignal = new();

            if (_clientData.UserId.ToString().Equals(senderId))
            {
                removeFromFriendListRequestSignal.UserId = Guid.Parse(receiverId);
            }
            else if (_clientData.UserId.ToString().Equals(receiverId))
            {
                removeFromFriendListRequestSignal.UserId = Guid.Parse(senderId);
            }
            else
            {
                return;
            }

            removeFromFriendListRequestSignal.IsRequest = true;

            _signalBus.Fire(removeFromFriendListRequestSignal);
        }

        [ClientRpc]
        private void DeleteFriendClientRpc(string senderId, string receiverId, ClientRpcParams clientRpcParams = default)
        {
            RemoveFromFriendListRequestSignal removeFromFriendListRequestSignal = new();

            if (_clientData.UserId.ToString().Equals(senderId))
            {
                removeFromFriendListRequestSignal.UserId = Guid.Parse(receiverId);
            }
            else if (_clientData.UserId.ToString().Equals(receiverId))
            {
                removeFromFriendListRequestSignal.UserId = Guid.Parse(senderId);
            }
            else
            {
                return;
            }

            removeFromFriendListRequestSignal.IsRequest = false;

            _signalBus.Fire(removeFromFriendListRequestSignal);
        }

        #endregion

        public void AddRequestMakeFriendToServer(string userId)
        {
            TryMakeTwoUsersFriendServerRpc(_clientData.UserId.ToString(), userId);
        }

        public void RemoveFriendFromFriendList(string userId)
        {
            DeleteFriendServerRpc(_clientData.UserId.ToString(),userId);
        }

        public void RemoveFriendRequest(string userId)
        {
            DeleteFriendRequestServerRpc(_clientData.UserId.ToString(), userId);
        }

        #region InnerClass

        public class AddFriendRequest : INetworkSerializable
        {
            public ulong SenderUserId;
            public string SenderUserGuid;
            public int SenderAvatarId;
            public string SenderUserName;
            public string SenderUserToken;
            public bool SenderConfirmed;

            public ulong ReceiverUserId;
            public string ReceiverUserGuid;
            public string ReceiveUserName;
            public string ReceiverUserToken;
            public int ReceiverAvatarId;
            public bool ReceiverConfirmed;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref SenderUserId);
                serializer.SerializeValue(ref SenderUserGuid);
                serializer.SerializeValue(ref SenderAvatarId);
                serializer.SerializeValue(ref SenderUserName);
                serializer.SerializeValue(ref SenderUserToken);
                serializer.SerializeValue(ref SenderConfirmed);

                serializer.SerializeValue(ref ReceiverUserId);
                serializer.SerializeValue(ref ReceiverUserGuid);
                serializer.SerializeValue(ref ReceiveUserName);
                serializer.SerializeValue(ref ReceiverUserToken);
                serializer.SerializeValue(ref ReceiverAvatarId);
                serializer.SerializeValue(ref ReceiverConfirmed);

            }
        }

        #endregion
    }
}