using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Mirror_selfie
{
    public class MirrorFactory : NetworkBehaviour
    {
        [SerializeField] private MirrorSelfie _mirrorPrefab;

        private MirrorSelfie _mirrorInstance;

        public event Action<GameObject> MirrorCreated;
        public event Action MirrorDestroyRequested;

        public void CreateMirror(Vector3 position, Quaternion rotation)
        {
            SpawnPrefabServerRpc(NetworkManager.Singleton.LocalClientId, position);
        }

        public void DespawnMirror()
        {
            DespawnNetworkObject();
        }

        private void DespawnNetworkObject()
        {
            if (_mirrorInstance != null)
            {
                UnregisterMirorEventsListeners();
                DespawnObjectServerRpc(_mirrorInstance.NetworkObject);
            }
        }

        private void RegisterMirrorEventsListeners()
        {
            if (_mirrorInstance != null)
            {
                _mirrorInstance.CloseRequested += OnCloseRequested;
            }
        }
        private void UnregisterMirorEventsListeners()
        {
            if (_mirrorInstance != null)
            {
                _mirrorInstance.CloseRequested -= OnCloseRequested;
            }
        }

        private void OnCloseRequested()
        {
            MirrorDestroyRequested?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPrefabServerRpc(ulong uId, Vector3 position)
        {
            MirrorSelfie mirror = Instantiate(_mirrorPrefab);

            mirror.transform.position = position;

            mirror.NetworkObject.SpawnWithOwnership(uId);

            SetClientSettingsClientRpc(mirror.NetworkObject);
        }
        [ClientRpc]
        private void SetClientSettingsClientRpc(NetworkObjectReference networkReference)
        {
            if (networkReference.TryGet(out NetworkObject networkMirror))
            {
                if (networkMirror.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    _mirrorInstance = networkMirror.GetComponent<MirrorSelfie>();
                    RegisterMirrorEventsListeners();

                    MirrorCreated?.Invoke(networkMirror.gameObject);
                }
            }

        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnObjectServerRpc(NetworkObjectReference networkObjectReference)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                networkObject.Despawn();
            }
        }
    }
}
