using System;
using Unity.Netcode;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class PadFactory : NetworkBehaviour
    {
        #region inspector
        [SerializeField] private PresentationPad _padPrefab;
        #endregion

        private PresentationPad _padInstance;

        public event Action<GameObject> PadCreated;
        public event Action PadDestroyRequested;   

        public void CreatePad(Vector3 position, Quaternion rotation)
        {
            SpawnPrefabServerRpc(NetworkManager.Singleton.LocalClientId, position);
        }

        public void DespawnPad()
        {
            DespawnNetworkObject();
        }

        private void DespawnNetworkObject()
        {
            if (_padInstance != null)
            {
                UnregisterPadEventsListeners();
                DespawnObjectServerRpc(_padInstance.NetworkObject);              
            }
        }

        private void RegisterPadEventsListeners()
        {
            if (_padInstance != null)
            {
                _padInstance.PadCloseRequested += OnPadCloseRequested;
            }
        }

        private void UnregisterPadEventsListeners()
        {
            if (_padInstance != null)
            {
                _padInstance.PadCloseRequested -= OnPadCloseRequested;
            }
        }

        private void OnPadCloseRequested()
        {
            PadDestroyRequested?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPrefabServerRpc(ulong uId, Vector3 position)
        {
            PresentationPad pad = Instantiate(_padPrefab);

            pad.transform.position = position;

            pad.NetworkObject.SpawnWithOwnership(uId);
            
            SetClientSettingsClientRpc(pad.NetworkObject);   
        }

        [ClientRpc]
        private void SetClientSettingsClientRpc(NetworkObjectReference networkReference)
        {
            if (networkReference.TryGet(out NetworkObject networkPad))
            {
                if (networkPad.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    _padInstance = networkPad.GetComponent<PresentationPad>();
                    RegisterPadEventsListeners();

                    CompositionRootNetworkScene compositionRoot = FindObjectOfType<CompositionRootNetworkScene>();
                    if (compositionRoot != null)
                    {
                        compositionRoot.InitPresentationNetwork(_padInstance);
                    }

                    PadCreated?.Invoke(networkPad.gameObject);
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