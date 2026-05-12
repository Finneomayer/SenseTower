using Unity.Netcode;

namespace Assets.Mechanics.Network.Scripts
{
    public static class NetworkManagerExtensions
    {
        public static void DisconnectClientWithNotification(this NetworkManager networkManager, ulong clientId)
        {
            if (networkManager == null)
            {
                return;
            }
            networkManager.DisconnectClient(clientId);
            NotifyDisconnected(clientId);
        }

        public static void DisconnectClientWithNotification(this NetworkManager networkManager, ulong clientId, string reason)
        {
            if (networkManager == null)
            {
                return;
            }
            networkManager.DisconnectClient(clientId, reason);
            NotifyDisconnected(clientId);
        }

        private static void NotifyDisconnected(ulong clientId)
        {
            if (NetworkEventsManager.Singleton == null)
            {
                return;
            }
            NetworkEventsManager.Singleton.RaiseOnClientDisconnectedCallback(clientId);
        }
    }
}
