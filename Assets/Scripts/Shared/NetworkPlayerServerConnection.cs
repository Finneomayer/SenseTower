using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Linq;
using Client;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Zenject;

namespace Assets.Scripts.Shared
{
    public class NetworkPlayerServerConnection : NetworkBehaviour
    {
        [SerializeField]
        private float DisconnectionTimeoutWithAccessToInternet = 10;

        private float _lastResponseTimeFromServer;
        private ClientRpcParams _ownerRpcParams;

        private bool _debugDisconnected;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                _ownerRpcParams = new()
                {
                    Send = new ClientRpcSendParams()
                    {
                        TargetClientIds = new ulong[] { NetworkObject.OwnerClientId }
                    }
                };
                StartCoroutine(ServerHeartbeatTransmissionRoutine());
                return;
            }

            if (IsClient && IsOwner)
            {
                _lastResponseTimeFromServer = Time.unscaledTime;
                StartCoroutine(CheckingDisconnectionOnClient());
            }
        }

        public override void OnNetworkDespawn()
        {
            StopAllCoroutines();
            base.OnNetworkDespawn();
        }

        [ContextMenu("Simulate disconnect")]
        private void DebugDisconnect()
        {
            _debugDisconnected = true;
        }

        private IEnumerator ServerHeartbeatTransmissionRoutine()
        {
            WaitForSeconds waitForOneSecond = new(1);
            while (NetworkManager != null 
                && NetworkManager.ConnectedClientsIds.Contains(_ownerRpcParams.Send.TargetClientIds[0]))
            {
                SendHeartbeatClientRpc(_ownerRpcParams);
                yield return waitForOneSecond;
            }
        }

        private IEnumerator CheckingDisconnectionOnClient()
        {
            WaitForSeconds waitForOneSecond = new(1);

            bool wasDisconnected = true;
            float lastReconnectedTime = Time.unscaledTime;

            while (NetworkManager != null)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    wasDisconnected = true;
                }
                else
                {
                    if (wasDisconnected)
                    {
                        lastReconnectedTime = Time.unscaledTime;
                        wasDisconnected = false;
                    }

                    if (Time.unscaledTime - _lastResponseTimeFromServer > DisconnectionTimeoutWithAccessToInternet
                        && Time.unscaledTime - lastReconnectedTime > DisconnectionTimeoutWithAccessToInternet)
                    {
                        Debug.Log("Scene reloading...");
                        SceneChangerView sceneChanger = FindObjectOfType<SceneChangerView>();
                        if (sceneChanger != null)
                        {
                            sceneChanger.ReloadCurrentSpace(keepPlayerPosition: true);
                        }
                        break;
                    }
                }

                yield return waitForOneSecond;
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void SendHeartbeatClientRpc(ClientRpcParams clientRpcParams = default)
        {
            if (_debugDisconnected)
            {
                return;
            }
            _lastResponseTimeFromServer = Time.unscaledTime;
        }
    }
}
