using System;
using System.Collections.Generic;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Client;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Audio
{
    public class AgoraAudioService : NetworkBehaviour, IAudioService
    {
        #region Inspector

        [SerializeField] private AgoraVoice _agoraVoice;
        [SerializeField] private RoomUsersWatcher _roomUsersWatcher;

        #endregion

        public Dictionary<ulong,string> MutedUsersID => _mutedUsers;
        
        private Dictionary<ulong,string> _mutedUsers = new();
        private IClientData _clientData;
        private SpatialAudio _localPlayerSpatialAudio;

        public event Action<string> UserSoundEnabled;
        public event Action<string> UserSoundDisabled;

        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                if (_roomUsersWatcher == null)
                    _roomUsersWatcher = FindObjectOfType<RoomUsersWatcher>();

                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            }

            if (!IsClient)
                return;

            GetMutedUsersServerRpc();
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }

        private void Awake()
        {
            if (_agoraVoice == null)
            {
                _agoraVoice = GetComponent<AgoraVoice>();
            }
        }

        private void OnEnable()
        {
            if (_agoraVoice == null)
            {
                return;
            }
            _agoraVoice.LocalPlayerSpatialSoundSetted += OnAgoraLocalPlayerSpatialSoundSetted;
        }

        private void OnDisable()
        {
            UnregisterLocalUserSpatialAudioListeners();
            if (_agoraVoice == null)
            {
                return;
            }
            _agoraVoice.LocalPlayerSpatialSoundSetted -= OnAgoraLocalPlayerSpatialSoundSetted;
        }

        private void RegisterLocalUserSpatialAudioListeners()
        {
            UnregisterLocalUserSpatialAudioListeners();
            if (_localPlayerSpatialAudio == null)
            {
                return;
            }
            _localPlayerSpatialAudio.UserSoundEnabled += OnSpatialAudioUserSoundEnabled;
            _localPlayerSpatialAudio.UserSoundDisabled += OnSpatialAudioUserSoundDisabled;
        }

        private void UnregisterLocalUserSpatialAudioListeners()
        {
            if (_localPlayerSpatialAudio == null)
            {
                return;
            }
            _localPlayerSpatialAudio.UserSoundEnabled -= OnSpatialAudioUserSoundEnabled;
            _localPlayerSpatialAudio.UserSoundDisabled -= OnSpatialAudioUserSoundDisabled;
        }

        private void OnAgoraLocalPlayerSpatialSoundSetted(SpatialAudio spatialAudio)
        {
            UnregisterLocalUserSpatialAudioListeners();
            _localPlayerSpatialAudio = spatialAudio;
            RegisterLocalUserSpatialAudioListeners();
        }

        private void OnSpatialAudioUserSoundEnabled(string userGuid)
        {
            UserSoundEnabled?.Invoke(userGuid);
        }

        private void OnSpatialAudioUserSoundDisabled(string userGuid)
        {
            UserSoundDisabled?.Invoke(userGuid);
        }

        public void MuteUserForUserServer(string userGuid, string userGuidToMute)
        {
            if (_roomUsersWatcher == null)
                return;

            if (_roomUsersWatcher.TryGetClientId(userGuid, out ulong clientId)
                && _roomUsersWatcher.TryGetClientId(userGuidToMute, out ulong clientIdToMute))
            {
                ClientRpcParams clientRpcParams = new()
                {
                    Send = new ClientRpcSendParams()
                    {
                        TargetClientIds = new ulong[] { clientId },
                    }
                };
                MuteUserClientRpc(clientIdToMute, userGuidToMute, clientRpcParams);
            }
        }

        public void UnmuteUserForUserServer(string userGuid, string userGuidToUnmute)
        {
            if (_roomUsersWatcher == null)
                return;

            if (_roomUsersWatcher.TryGetClientId(userGuid, out ulong clientId)
                && _roomUsersWatcher.TryGetClientId(userGuidToUnmute, out ulong clientIdToMute))
            {
                ClientRpcParams clientRpcParams = new()
                {
                    Send = new ClientRpcSendParams()
                    {
                        TargetClientIds = new ulong[] { clientId },
                    }
                };
                UnMuteClientRpc(clientIdToMute, userGuidToUnmute, clientRpcParams);
            }
        }

        public void MuteUser(string userGuid)
        {
            MuteUserServerRpc(userGuid);
        }

        public void MuteUser(ulong userId)
        {
            MuteUserServerRpc(userId);
        }

        public void UnmuteUser(string userGuid)
        {
            UnmuteUserServerRpc(userGuid);
        }

        public void UnmuteUser(ulong userId)
        {
            UnmuteUserServerRpc(userId);
        }

        #region ServerRpc

        [ServerRpc(RequireOwnership = false)]
        private void GetMutedUsersServerRpc()
        {
            foreach (var mutedUser in _mutedUsers)
            {
                MuteUserClientRpc(mutedUser.Key,mutedUser.Value);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void MuteUserServerRpc(ulong userId)
        {
            if (_roomUsersWatcher == null)
                return;
            string userGuid = _roomUsersWatcher.GetUserGuid(userId);
            if (!string.IsNullOrEmpty(userGuid))
            {
                AddUserInMuteList(userGuid,userId);
                MuteUserClientRpc(userId,userGuid);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void MuteUserServerRpc(string userGuid)
        {
            if (_roomUsersWatcher == null)
                return;

            if (_roomUsersWatcher.TryGetClientId(userGuid, out ulong clientId))
            {
                AddUserInMuteList(userGuid,clientId);
                MuteUserClientRpc(clientId,userGuid);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void UnmuteUserServerRpc(string userGuid)
        {
            if (_roomUsersWatcher == null)
                return;
            
            if (_roomUsersWatcher.TryGetClientId(userGuid, out ulong clientId))
            {
                UnMuteClientRpc(clientId,userGuid);
                RemoveUserInMuteList(userGuid,clientId);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void UnmuteUserServerRpc(ulong userId)
        {
            if (_roomUsersWatcher == null)
                return;
            
            string userGuid = _roomUsersWatcher.GetUserGuid(userId);
            if (!string.IsNullOrEmpty(userGuid))
            {
                UnMuteClientRpc(userId,userGuid);
                RemoveUserInMuteList(userGuid,userId);
            }
        }
        #endregion

        #region ClientRpc

        [ClientRpc]
        private void MuteUserClientRpc(ulong userId, string GuID, ClientRpcParams clientRpcParams = default)
        {
            if(NetworkManager.Singleton.LocalClientId != userId)
                _agoraVoice.GetCurrentPlayerSpatialAudio().MinVolume(userId);
            
            AddUserInMuteList(GuID,userId);
        }

        [ClientRpc]
        private void UnMuteClientRpc(ulong userId, string GuID, ClientRpcParams clientRpcParams = default)
        {
            if(NetworkManager.Singleton.LocalClientId != userId)
                _agoraVoice.GetCurrentPlayerSpatialAudio().DisableMinVolume(userId);
   
            RemoveUserInMuteList(GuID,userId);
        }


        #endregion

        private void AddUserInMuteList(string GuID, ulong userId)
        {
            if (!_mutedUsers.ContainsKey(userId))
                _mutedUsers.Add(userId,GuID);
        }

        private void RemoveUserInMuteList(string GuID, ulong userId)
        {
            if (_mutedUsers.ContainsKey(userId))
                _mutedUsers.Remove(userId);
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            if (_mutedUsers.ContainsKey(clientId))
            {
                UnMuteClientRpc(clientId,_mutedUsers[clientId]);
                RemoveUserInMuteList(_mutedUsers[clientId],clientId);
            }
        }
    }
}