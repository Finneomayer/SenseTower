using System;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Server;

namespace Assets.Mechanics.Network.Scripts
{
    public class RoomUsersKicker : NetworkBehaviour
    {
        [SerializeField]
        private RoomUsersWatcher RoomUsersWatcher;

        public void KickUserServer(string userGuid)
        {
            if (RoomUsersWatcher.TryGetClientId(userGuid, out ulong clientId))
            {
                KickUser(clientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void KickUserServerRpc(string userGuid, UserKickReason userKickReason)
        {
            if (RoomUsersWatcher.TryGetClientId(userGuid, out ulong clientId))
            {
                KickUser(clientId, userKickReason);
            }
        }

        [ContextMenu("Kick as PrivateSpace")]
        public void KickUserTestInEditorPrivateSpace()
        {
            foreach (var clientId in NetworkManager.ConnectedClientsIds)
            {
                KickUser(clientId, UserKickReason.PrivateSpace);
            }
        }

        [ContextMenu("Kick as OwnerIsNotInSpace")]
        public void KickUserTestInEditorOwnerIsNotInSpace()
        {
            foreach (var clientId in NetworkManager.ConnectedClientsIds)
            {
                KickUser(clientId, UserKickReason.OwnerIsNotInSpace);
            }
        }

        private void KickUser(ulong clientId)
        {
            if (!NetworkManager.ConnectedClientsIds.Contains(clientId))
            {
                return;
            }

            NetworkManager.DisconnectClientWithNotification(clientId);
        }

        private void KickUser(ulong clientId, UserKickReason userKickReason)
        {
            if (!NetworkManager.ConnectedClientsIds.Contains(clientId))
            {
                return;
            }

            UserDisconnectData userDisconnectData = new()
            {
                DisconnectMessage = userKickReason.ToString(),
                CanBeInSenseTower = true
            };
            NetworkManager.DisconnectClientWithNotification(clientId, userDisconnectData.Serialize());
        }

    }

    public enum UserKickReason
    {
        PrivateSpace = 1,
        OwnerIsNotInSpace = 2
    }
}