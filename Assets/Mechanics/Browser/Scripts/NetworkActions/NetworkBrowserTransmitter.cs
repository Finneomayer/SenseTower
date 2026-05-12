using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public abstract class NetworkBrowserTransmitter : NetworkBehaviour
    {
        public string SyncData { get; protected set; }
        protected NetworkWebBrowser _networkWebBrowser;

        public void SetWebBrowser(NetworkWebBrowser networkWebBrowser)
        {
            _networkWebBrowser = networkWebBrowser;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DataSynchronizationServerRpc(ulong clientId, string data)
        {
            List<ulong> _clients = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            _clients.Remove(clientId);

            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = _clients;

            ApplyChangesClientRpc(data, clientRpcParams);
        }

        public abstract void CheckSyncData();

        public abstract void ApplyChanges();

        [ClientRpc]
        private void ApplyChangesClientRpc(string data, ClientRpcParams clientRpcParams = default)
        {
            SyncData = data;
            ApplyChanges();
        }
    }
}