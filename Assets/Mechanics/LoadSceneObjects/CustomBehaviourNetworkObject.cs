using System;
using Unity.Netcode;

namespace Mechanics.LoadSceneObjects
{
    public class CustomBehaviourNetworkObject : NetworkBehaviour
    {
        public event Action StateChanged;

        private string _currentStateString = "";

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsClient)
            {
                RequestStateFromServer();
            }
        }

        public string GetState()
        {
            return _currentStateString;
        }

        public void SetState(string jsonString)
        {
            SendStateServerRpc(jsonString);
        }

        public void SetStateServer(string jsonString)
        {
            _currentStateString = jsonString;
            SendStateClientRpc(jsonString);
        }

        public void SetStateWithoutNotificationServer(string jsonString)
        {
            _currentStateString = jsonString;
        }

        public void RequestStateFromServer()
        {
            GetStateServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendStateServerRpc(string jsonString)
        {
            _currentStateString = jsonString;
            SendStateClientRpc(_currentStateString);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetStateServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] { clientId };

            SendStateClientRpc(_currentStateString, clientRpcParams);
        }

        [ClientRpc]
        private void SendStateClientRpc(string jsonString, ClientRpcParams clientRpcParams = default)
        {
            _currentStateString = jsonString;
            StateChanged?.Invoke();
        }
    }
}