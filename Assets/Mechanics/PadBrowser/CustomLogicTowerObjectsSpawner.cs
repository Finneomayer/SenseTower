using Data;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.TowerObjects;

namespace Infrastructure.Factory
{
    public class CustomLogicTowerObjectsSpawner : NetworkBehaviour
    {
        [SerializeField]
        private CustomLogicTowerObjectsSet CustomLogicTowerObjectsSet;

        private Dictionary<string, SpawnedObjectData> _spawnedObjectsMap = new();

        public event Action<string> ObjectSpawned;
        public event Action<string> ObjectDespawned;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsClient)
            {
                RequestSpawnedObjectsServerRpc(NetworkManager.LocalClientId);
            }

            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }
        }

        public void SpawnWithLocalUserOwnership(TowerObjectDto towerObjectDto, 
            Vector3 position, Quaternion rotation)
        {
            SpawnServerRpc(NetworkManager.Singleton.LocalClientId, towerObjectDto.ObjectKey,
                towerObjectDto.Id.ToString(), towerObjectDto.OwnerName, towerObjectDto.OwnerId.ToString(), position, rotation);
        }

        public void Despawn(string towerObjectId)
        {
            DespawnServerRpc(towerObjectId);
        }

        public bool TryGetSpawnedObjectData(string towerObjectId, out SpawnedObjectData spawnedObjectData)
        {
            if (_spawnedObjectsMap.TryGetValue(towerObjectId, out spawnedObjectData))
            {
                return true;
            }

            return false;
        }

        public bool TryGetInventoryPrefab(string key, out GameObject prefab)
        {
            CustomLogicTowerObjectsSet.TryGetInventoryPrefabByKey(key, out prefab);
            return prefab != null;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(ulong clientId, string key, string towerObjectId,
            string ownerName, string ownerId, Vector3 position, Quaternion rotation)
        {
            CustomLogicTowerObjectsSet.TryGetPrefabByKey(key, out GameObject prefab);
            if (prefab == null)
            {
                Debug.Log($"SpawnServerRpc. {key} prefab == null");
                return;
            }

            if (_spawnedObjectsMap.TryGetValue(towerObjectId, out SpawnedObjectData _))
            {
                Debug.Log($"SpawnServerRpc. {key} {towerObjectId} already instantiated");
                return;
            }

            GameObject go = Instantiate(prefab, position, rotation);
            NetworkObject instance = go.GetComponent<NetworkObject>();

            instance.SpawnWithOwnership(clientId);

            SpawnedObjectData spawnedObjectData = new SpawnedObjectData()
            {
                ObjectKey = key,
                TowerObjectId = towerObjectId,
                OwnerName = ownerName,
                OwnerId = ownerId,
                NetworkObject = instance,
            };

            _spawnedObjectsMap.Add(towerObjectId, spawnedObjectData);

            SpawnPrefabClientRpc(towerObjectId, spawnedObjectData);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSpawnedObjectsServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] { clientId };


            SpawnedObjectData spawnedObjectData = new SpawnedObjectData();
            foreach (var item in _spawnedObjectsMap)
            {
                spawnedObjectData.ObjectKey = item.ToString();
                SpawnPrefabClientRpc(item.Key, item.Value, clientRpcParams);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc(string towerObjectId)
        {
            if (!_spawnedObjectsMap.TryGetValue(towerObjectId, out SpawnedObjectData spawnedObjectData))
            {
                return;
            }
            _spawnedObjectsMap.Remove(towerObjectId);

            if (spawnedObjectData.NetworkObject != null)
            {
                spawnedObjectData.NetworkObject.Despawn();
            }

            DespawnInstanceClientRpc(towerObjectId);
        }

        [ClientRpc]
        private void SpawnPrefabClientRpc(string towerObjectId, SpawnedObjectData spawnedObjectData, 
            ClientRpcParams clientRpcParams = default)
        {
            _spawnedObjectsMap.Add(towerObjectId, spawnedObjectData);
            ObjectSpawned?.Invoke(towerObjectId);
        }

        [ClientRpc]
        private void DespawnInstanceClientRpc(string towerObjectId)
        {
            if (_spawnedObjectsMap.TryGetValue(towerObjectId, out SpawnedObjectData spawnedObjectData))
            {
                _spawnedObjectsMap.Remove(towerObjectId);
            }

            ObjectDespawned?.Invoke(towerObjectId);
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            for (int i = _spawnedObjectsMap.Count - 1; i >= 0; i--)
            {
                var keyValue = _spawnedObjectsMap.ElementAt(i);
                if (keyValue.Value == null || keyValue.Value.NetworkObject == null 
                    || keyValue.Value.NetworkObject.OwnerClientId == clientId)
                {
                    _spawnedObjectsMap.Remove(keyValue.Key);
                    DespawnInstanceClientRpc(keyValue.Key);
                }
            }
        }
    }

    public class SpawnedObjectData : INetworkSerializable
    {
        public string ObjectKey;
        public string TowerObjectId;
        public string OwnerName;
        public string OwnerId;
        public NetworkObject NetworkObject;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ObjectKey);
            serializer.SerializeValue(ref OwnerName);
            serializer.SerializeValue(ref OwnerId);

            NetworkObjectReference networkObjectReference;
            if (serializer.IsWriter)
            {
                networkObjectReference = NetworkObject;
                networkObjectReference.NetworkSerialize(serializer);
            }
            else
            {
                networkObjectReference = new();
                networkObjectReference.NetworkSerialize(serializer);
                networkObjectReference.TryGet(out NetworkObject);
            }
        }
    }
}