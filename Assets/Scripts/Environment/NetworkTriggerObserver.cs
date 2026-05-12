using System;
using Assets.Scripts.Environmental.Presentation.Browser;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

namespace Assets.Scripts.Environmental
{
    [RequireComponent(typeof(Collider))]
    public class NetworkTriggerObserver : MonoBehaviour
    {
        public event Action<GameObject> LocalClientEnterTrigger;
        public event Action<ulong, bool> RemoteClientEnterTrigger; //bool Remote client is full fledged
        public event Action LocalClientExitTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            var playerObject = other.GetComponent<Camera>();
            ulong clientId = 0;
            
            if (playerObject != null)
            {
                var network = playerObject.transform.GetComponentInParent<NetworkPlayer>();
                if (network != null)
                    clientId = network.OwnerClientId;
                if (clientId == NetworkManager.Singleton.LocalClientId)
                    LocalClientEnterTrigger?.Invoke(other.gameObject);
            }
            else
            {
                NetworkPlayer networkPlayer = other.transform.GetComponentInParent<NetworkPlayer>();
                if (networkPlayer != null)
                {
                    if (networkPlayer.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                    {
                        ClientIdView client;
                        bool isFullFledgedUser = false;
                        if (networkPlayer.TryGetComponent<ClientIdView>(out client))
                            isFullFledgedUser = client.IsFullFledgedUser;
                        RemoteClientEnterTrigger?.Invoke(networkPlayer.OwnerClientId, isFullFledgedUser); //need to prevent giving coin
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            NetworkPlayer networkPlayer = other.transform.GetComponentInParent<NetworkPlayer>();
            if (networkPlayer != null)
            {
                if (networkPlayer.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                {
                    ClientIdView client;
                    bool isFullFledgedUser = false;
                    if (networkPlayer.TryGetComponent<ClientIdView>(out client))
                        isFullFledgedUser = client.IsFullFledgedUser;
                    RemoteClientEnterTrigger?.Invoke(networkPlayer.OwnerClientId, isFullFledgedUser);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            var playerObject = other.GetComponent<Camera>();
            ulong clientId = 0;

            if (playerObject != null)
            {
                var network = playerObject.transform.GetComponentInParent<NetworkPlayer>();
                if(network != null)
                    clientId = network.OwnerClientId;
                if (clientId != 0)
                    LocalClientExitTrigger?.Invoke();
            }             
        }
    }
}