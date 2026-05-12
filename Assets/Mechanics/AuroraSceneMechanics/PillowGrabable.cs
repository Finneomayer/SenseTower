using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.AuroraSceneMechanics
{
    public class PillowGrabable : NetworkBehaviour
    {
        public XRGrabInteractable Interactable;
        private Rigidbody _rigidbody;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong obj)
        {
            if (NetworkManager.ConnectedClientsList.Count > 0)
            {
                NetworkObject.ChangeOwnership(NetworkManager.ConnectedClientsIds[0]);
                SetGravityClientRpc();
            }
        }

        public void Init()
        {
            Interactable.selectEntered.AddListener(OnSelectEnter);
            Interactable.selectExited.AddListener(OnSelectExit);
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnSelectExit(SelectExitEventArgs arg0)
        {
            if (IsOwner)
            {
                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;
            }
            else
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
            }
        }

        private void OnSelectEnter(SelectEnterEventArgs arg0)
        {
            SetOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetOwnershipServerRpc(ulong id = 0)
        {
            NetworkObject.ChangeOwnership(id);
            SetGravityClientRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RemoveOwnerServerRpc()
        {
            NetworkObject.RemoveOwnership();
            SetGravityClientRpc();
        }

        [ClientRpc]
        private void SetGravityClientRpc()
        {
            if (IsOwner)
            {
                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;
            }
            else
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
            }
        }
    }
}
