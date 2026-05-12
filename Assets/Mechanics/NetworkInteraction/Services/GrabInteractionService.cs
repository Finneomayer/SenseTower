using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.Network.Scripts;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.NetworkInteraction.Services
{
    public class GrabInteractionService : NetworkBehaviour, IGrabInteraction
    {
        private List<ObjectData> _objectsInSpace = new();

        private ulong _ownerServerId = 0;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnDisable()
        {
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        public void ChangeOwnership(NetworkObject networkObject, ulong newValue)
        {
            ChangeOwnerServerRPC(networkObject, newValue);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeOwnerServerRPC(NetworkObjectReference networkObjectReference, ulong currentHolderId)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                networkObject.DontDestroyWithOwner = true;
                if (TryGetData(networkObject.NetworkObjectId, out ObjectData objectData))
                {
                    objectData.CurrentHolderId = currentHolderId;
                }
                else
                {
                    objectData.NetworkObject = networkObject;
                    objectData.NetworkObject.DontDestroyWithOwner = true;
                    objectData.OwnerId = networkObject.IsOwnedByServer ? _ownerServerId : networkObject.OwnerClientId;
                    objectData.CurrentHolderId = currentHolderId;
                    _objectsInSpace.Add(objectData);
                }

                networkObject.ChangeOwnership(currentHolderId);
                SetClientObjectClientRpc(networkObject);
            }
        }

        [ClientRpc]
        private void SetClientObjectClientRpc(NetworkObjectReference networkObjectReference)
        {
            if (!networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                return;
            }

            NetworkXrGrab grab;
            if (networkObject.TryGetComponent(out NetworkItemType networkItemType) && networkItemType.AimObject != null)
            {
                networkItemType.AimObject.TryGetComponent(out grab);
            }
            else
            {
                networkObject.TryGetComponent(out grab);
            }

            if (networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
            {
                if (networkItemType != null)
                {
                    networkItemType.StopFollow();
                }
                if (grab != null)
                {
                    grab.ForceDrop();
                }
            }
            else
            {
                if (networkItemType != null)
                {
                    networkItemType.StartFollow();
                }
            }
        }

        private bool TryGetData(ulong objectId, out ObjectData tempObjectData)
        {
            bool result = false;
            foreach (ObjectData objectData in _objectsInSpace)
            {
                if (objectData.NetworkObject.NetworkObjectId == objectId)
                {
                    tempObjectData = objectData;
                    result = true;
                    break;
                }
            }

            tempObjectData = default;
            return result;
        }

        private void DestroyObjectsWithOwnerId(ulong ownerId)
        {
            List<NetworkObject> ownerObjects = new();
            List<ObjectData> holderObjects = new();
            foreach (ObjectData objectData in _objectsInSpace)
            {
                if (objectData.OwnerId == _ownerServerId)
                {
                    continue;
                }

                if (objectData.OwnerId == ownerId)
                {
                    ownerObjects.Add(objectData.NetworkObject);
                }
                else if (objectData.CurrentHolderId == ownerId)
                {
                    holderObjects.Add(objectData);
                }
            }

            if (ownerObjects.Count != 0)
            {
                foreach (NetworkObject ownerObject in ownerObjects)
                {
                    if (ownerObject == null)
                        continue;

                    var tempObj = _objectsInSpace.FirstOrDefault(element => element.NetworkObject == ownerObject);
                    if (tempObj.NetworkObject != null)
                        _objectsInSpace.Remove(tempObj);
                    
                    if(ownerObject.IsSpawned)
                        ownerObject.Despawn();
                    
                    if(ownerObject.gameObject != null)
                        Destroy(ownerObject.gameObject);
                }
            }

            if (holderObjects.Count != 0)
            {
                foreach (ObjectData ownerObject in holderObjects)
                {
                    if (ownerObject.NetworkObject == null)
                        continue;

                    if (NetworkManager.ConnectedClients.ContainsKey(ownerObject.OwnerId))
                    {
                        if(ownerObject.NetworkObject.IsSpawned)
                            ownerObject.NetworkObject.ChangeOwnership(ownerObject.OwnerId);
                    }
                    else
                    {
                        if (ownerObject.OwnerId == _ownerServerId &&  ownerObject.NetworkObject.IsSpawned)
                        {
                            ownerObject.NetworkObject.RemoveOwnership();
                        }
                        else
                        {
                            _objectsInSpace.Remove(ownerObject);
                            
                            if(ownerObject.NetworkObject.IsSpawned)
                                ownerObject.NetworkObject.Despawn();
                            
                            if(ownerObject.NetworkObject != null && ownerObject.NetworkObject.gameObject != null)
                                Destroy(ownerObject.NetworkObject.gameObject);
                        }
                    }
                }
            }
        }

        private void OnClientDisconnect(ulong ownerId)
        {
            DestroyObjectsWithOwnerId(ownerId);
        }

        #region InnerClass

        private struct ObjectData
        {
            public ulong OwnerId;
            public ulong CurrentHolderId;
            public NetworkObject NetworkObject;
        }

        #endregion
    }
}