using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Server;
using System.Linq;

namespace Assets.Scripts.Shared
{
    public class NetworkPlayerConnection : NetworkBehaviour
    {
        private const int FastDisconnectTimeoutSec = 3;

        private float _lastHeartbeatTime;
        private Coroutine _checkingForDisconnectionServerRoutine;
        private ServerVerification _serverVerification;

        private void OnApplicationFocus(bool focus)
        {
            if (!IsClient || !IsOwner || Application.platform != RuntimePlatform.Android)
            {
                return;
            }

            SetFastDisconnectEnabledServerRpc(!focus);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _serverVerification = FindObjectOfType<ServerVerification>();

            if (!IsClient || !IsOwner)
            {
                return;
            }

            StartCoroutine(HeartbeatTransmissionRoutine());
        }

        public override void OnNetworkDespawn()
        {
            StopAllCoroutines();
            base.OnNetworkDespawn();
        }

        private IEnumerator HeartbeatTransmissionRoutine()
        {
            WaitForSeconds waitForOneSecond = new (1);
            while (NetworkManager != null)
            {
                SendHeartbeatServerRpc();
                yield return waitForOneSecond;
            }
        }

        private IEnumerator CheckingForDisconnectionServerRoutine()
        {
            WaitForSeconds waitForOneSecond = new(1);
            _lastHeartbeatTime = Time.unscaledTime;
            while (NetworkManager != null)
            {
                yield return waitForOneSecond;
                
                if (NetworkManager == null || NetworkObject == null
                    || !NetworkManager.ConnectedClientsIds.Contains(NetworkObject.OwnerClientId))
                {
                    break;
                }
                
                if (Time.unscaledTime - _lastHeartbeatTime > FastDisconnectTimeoutSec)
                {
                    NetworkManager.DisconnectClient(NetworkObject.OwnerClientId);
                    NetworkEventsManager.Singleton.RaiseOnClientDisconnectedCallback(NetworkObject.OwnerClientId);
                    break;
                }
            }
            _checkingForDisconnectionServerRoutine = null;
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void SendHeartbeatServerRpc()
        {
            _lastHeartbeatTime = Time.unscaledTime;

            if (_serverVerification != null)
            {
                _serverVerification.NotifyUserIsActive(NetworkObject.OwnerClientId);
            }
        }

        [ServerRpc]
        private void SetFastDisconnectEnabledServerRpc(bool isEnabled)
        {
            if (_checkingForDisconnectionServerRoutine != null)
            {
                StopCoroutine(_checkingForDisconnectionServerRoutine);
                _checkingForDisconnectionServerRoutine = null;
            }

            if (isEnabled)
            {
                _checkingForDisconnectionServerRoutine = StartCoroutine(CheckingForDisconnectionServerRoutine());
            }
        }
    }
}
