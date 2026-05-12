using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public class NetworkTransmitterService : NetworkBehaviour
    {
        #region Inspector

        [SerializeField] private NetworkWebBrowser _networkWebBrowser;

        [SerializeField] private int _transmissionsDelayInSeconds;

        [SerializeField] private List<NetworkBrowserTransmitter> _transmittersList = new();

        #endregion

        private Coroutine _transmitterCoroutine;

        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;

            for (int i = 0; i < _transmittersList.Count; i++)
            {
                _transmittersList[i].SetWebBrowser(_networkWebBrowser);
            }
            _networkWebBrowser.AdminChanged += OnAdminChange;
            _networkWebBrowser.AdminUrlChanged += OnUrlChange;
        }

        public override void OnDestroy()
        {
            if (!IsClient) return;
            
            _networkWebBrowser.AdminChanged -= OnAdminChange;
            _networkWebBrowser.AdminUrlChanged -= OnUrlChange;
        }

        private void OnUrlChange(string url)
        {
        }

        private void OnAdminChange(ulong adminId)
        {
            if (adminId == NetworkManager.Singleton.LocalClientId)
                StartTransmitter();
            else
                StopTransmitter();
        }

        private void StartTransmitter()
        {
            _transmitterCoroutine = StartCoroutine(TransferСoroutine());
        }

        private void StopTransmitter()
        {
            if (_transmitterCoroutine != null)
                StopCoroutine(_transmitterCoroutine);
        }

        private IEnumerator TransferСoroutine()
        {
            while (true)
            {
                for (int i = 0; i < _transmittersList.Count; i++)
                {
                    NetworkBrowserTransmitter tempTransmitter = _transmittersList[i];

                    tempTransmitter.CheckSyncData();
                    _transmittersList[i].DataSynchronizationServerRpc(NetworkManager.Singleton.LocalClientId,
                        tempTransmitter.SyncData);
                }

                yield return new WaitForSeconds(_transmissionsDelayInSeconds);
            }
        }
    }
}