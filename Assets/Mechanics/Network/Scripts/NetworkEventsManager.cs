using Unity.Netcode;
using UnityEngine;
using System;

namespace Assets.Mechanics.Network.Scripts
{
    public class NetworkEventsManager : MonoBehaviour
    {
        [SerializeField]
        private NetworkManager NetworkManager;

        public static NetworkEventsManager Singleton;

        public event Action<ulong> OnClientConnectedCallback;
        public event Action<ulong> OnClientDisconnectCallback;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(this);
                return;
            }
        }

        private void OnEnable()
        {
            if (NetworkManager == null)
            {
                return;
            }
            NetworkManager.OnClientConnectedCallback += OnNetworkManagerClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnNetworkManagerClientDisconnectCallback;
        }

        private void OnDisable()
        {
            if (NetworkManager == null)
            {
                return;
            }
            NetworkManager.OnClientConnectedCallback -= OnNetworkManagerClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= OnNetworkManagerClientDisconnectCallback;
        }

        public void RaiseOnClientDisconnectedCallback(ulong clientId)
        {
            OnNetworkManagerClientDisconnectCallback(clientId);
        }

        private void OnNetworkManagerClientConnectedCallback(ulong clientId)
        {
            OnClientConnectedCallback?.Invoke(clientId);
        }

        private void OnNetworkManagerClientDisconnectCallback(ulong clientId)
        {
            OnClientDisconnectCallback?.Invoke(clientId);
        }
    }
}
