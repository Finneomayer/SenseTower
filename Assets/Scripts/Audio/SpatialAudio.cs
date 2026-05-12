using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Zones;
using UnityEngine.Android;
using Unity.Netcode;
using UnityEngine;
using Oculus.Avatar2;
using Assets.Scripts.Shared;

public class SpatialAudio : NetworkBehaviour
{
    [Serializable]
    private class UserHead
    {
        public uint AgoraClientId { get; set; }
        public string UserGuid { get; set; }
        public ulong NetworkClientId { get; }
        public Assets.Scripts.Shared.NetworkPlayer NetworkPlayer { get; }

        private Transform _headTransform;
        private bool _maxVolume;
        private bool _minVolume;

        public UserHead(string userGuid, ulong networkId, uint agoraClientId, Assets.Scripts.Shared.NetworkPlayer networkPlayer)
        {
            UserGuid = userGuid;
            NetworkClientId = networkId;
            AgoraClientId = agoraClientId;
            NetworkPlayer = networkPlayer;
        }

        public bool TryGetHeadTransform(out Transform headTransform)
        {
            headTransform = null;
            if (_headTransform == null)
            {
                _headTransform = NetworkPlayer.GetRemotePlayerHead();
            }

            if (_headTransform != null)
            {
                headTransform = _headTransform;
                return true;
            }

            return false;
        }

        public bool MaxValue {
            get => _maxVolume;
            set => _maxVolume = value;
        }
        public bool MinValue {
            get => _minVolume;
            set => _minVolume = value;
        }
    }

    private const float MAX_CHAT_PROXIMITY = 1.5f;
    private const float MAX_CHAT_DISTANCE = 18f;

    #region Inspector

    [SerializeField] private ZonesModel _zonesModel;
    [SerializeField] private AgoraVoice _agoraVoice;
    [SerializeField] private List<UserHead> _players;
    #endregion

    #region PrivateVariables
    
    [SerializeField]
    private NetworkVariable<uint> networkedUID = new NetworkVariable<uint>(
        default,
        NetworkVariableBase.DefaultReadPerm,
        NetworkVariableWritePermission.Owner);

    private uint _myUid;
    private Coroutine _maxVolumePlayer;
    private Coroutine _minVolumePlayer;
    #endregion

    public event Action<string> UserSoundEnabled;
    public event Action<string> UserSoundDisabled;

    #region PublicMethods

    public void MaxVolume(ulong clientId)
    {
        if (!IsOwner) return;
        
        if(_maxVolumePlayer != null)
            StopCoroutine(_maxVolumePlayer);
        
        _maxVolumePlayer = StartCoroutine(MaxVolumePlayerInSpatialAudio(clientId));
    }
    public void MinVolume(ulong clientId)
    {
        if (!IsOwner) return;

        // TODO: change for MaxVolume too if everything works fine
        //if(_minVolumePlayer != null)
        //    StopCoroutine(_minVolumePlayer);

        _minVolumePlayer = StartCoroutine(MinVolumePlayerInSpatialAudio(clientId));
    }
    
    private IEnumerator MaxVolumePlayerInSpatialAudio(ulong clientId)
    {
        yield return new WaitUntil(()=> _players.Exists(element=> element.NetworkClientId == clientId));
        var tempExcludePlayer =  _players.Find(element => element.NetworkClientId == clientId);
        if(tempExcludePlayer != null)
            tempExcludePlayer.MaxValue = true;
    }
    
    public void DisableMaxVolume(ulong clientId)
    {
        if (!IsOwner) return;
            
        var excludePlayer =  _players.Find(element => element.NetworkClientId == clientId);
        
        if(excludePlayer != null)
            excludePlayer.MaxValue = false;
    }

    public void DisableMinVolume(ulong clientId)
    {
        if (!IsOwner) return;

        _minVolumePlayer = StartCoroutine(DisableMinVolumePlayerInSpatialAudio(clientId));
    }

    private IEnumerator MinVolumePlayerInSpatialAudio(ulong clientId)
    {
        yield return new WaitUntil(()=> _players.Exists(element=> element.NetworkClientId == clientId));
        var tempExcludePlayer =  _players.Find(element => element.NetworkClientId == clientId);
        if (tempExcludePlayer != null)
        {
            tempExcludePlayer.MinValue = true;
            UserSoundDisabled?.Invoke(tempExcludePlayer.UserGuid);
        }
    }

    private IEnumerator DisableMinVolumePlayerInSpatialAudio(ulong clientId)
    {
        yield return new WaitUntil(() => _players.Exists(element => element.NetworkClientId == clientId));
        var tempExcludePlayer = _players.Find(element => element.NetworkClientId == clientId);
        if (tempExcludePlayer != null)
        {
            tempExcludePlayer.MinValue = false;
            UserSoundEnabled?.Invoke(tempExcludePlayer.UserGuid);
        }
    }

    public void RemoveAllExcludeClients()
    {
        foreach (UserHead player in _players)
        {
            DisableMaxVolume(player.NetworkClientId);
            DisableMinVolume(player.NetworkClientId);
        }
    }

    #endregion
    
    #region UnityMethods
    private void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        if (!IsOwner) return;
        _players = new List<UserHead>();
    }

    public override void OnDestroy()
    {
        if (ReferenceEquals(_agoraVoice, null))
        {
            base.OnDestroy();
            return;
        }
        
        _agoraVoice.PlayerLeaveChannel -= OnPLayerLeaveAgoraChannel;
        _agoraVoice.LocalPlayerJoinChannel -= OnLocalPlayerJoinAgoraChannel;
        _agoraVoice.RemotePlayerJoinChannel -= OnRemotePlayerJoinAgoraChannel;

        base.OnDestroy();
    }

    private void Update()
    {
        UpdateSpatialAudio();
    }

#endregion

    #region Network
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _agoraVoice = FindObjectOfType<AgoraVoice>();
        _zonesModel = FindObjectOfType<ZonesModel>();
        
        if (!ReferenceEquals(_agoraVoice,null)) 
        {
            _agoraVoice.LocalPlayerJoinChannel += OnLocalPlayerJoinAgoraChannel;
            _agoraVoice.RemotePlayerJoinChannel += OnRemotePlayerJoinAgoraChannel;
            _agoraVoice.PlayerLeaveChannel += OnPLayerLeaveAgoraChannel;
            _agoraVoice.SaveCurrentPlayerSpatialAudio(this);
        }
    }

    [ServerRpc]
    private void SetUidServerRpc(uint uid)
    {
        networkedUID.Value = uid;
    }

#endregion
    
    #region Callbacks
    
    private void OnLocalPlayerJoinAgoraChannel(uint uid)
    {
        if (!IsOwner) return;
        _myUid = uid;
        networkedUID.Value = uid;
        StartCoroutine(AddNetworkPlayerToSpatialList());
    }
    
    private void OnRemotePlayerJoinAgoraChannel(uint uid)
    {
        if (!IsOwner) return;
        networkedUID.Value = _myUid;
        StartCoroutine(AddNetworkPlayerToSpatialList());
    }

    private void OnPLayerLeaveAgoraChannel(uint uid)
    {
        DeletePlayer(uid);
    }

#endregion
    
    #region PrivateMethods
    
    private void UpdateSpatialAudio()
    {
        if (!IsOwner) return;

        Camera ownerCamera = Camera.main;
        if (ownerCamera == null)
        {
            return;
        }

        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].NetworkPlayer == null)
            {
                continue;
            }

            bool canUserRestrictions = false;
            
            if (_zonesModel.ZoneController != null)
            {
                if (_zonesModel.ZoneController.UserInZone(_players[i].NetworkClientId))
                {
                    canUserRestrictions = true;
                }
            }
            else
            {
                canUserRestrictions = true;
            }

            if (!_players[i].TryGetHeadTransform(out Transform otherPlayerHeadTransform))
            {
                Debug.Log(
                    $"<color=purple>SpatialAudio.</color> avatarHeadTransform == null. Client id: {_players[i].NetworkClientId}. Reassigning...");
                otherPlayerHeadTransform = _players[i].NetworkPlayer.transform;
            }

            float pan = GetPanByPlayerOrientation(otherPlayerHeadTransform, ownerCamera.transform);

            if (canUserRestrictions)
            {
                if (_players[i].MinValue)
                {
                    _agoraVoice.GetAudioEffectManager().SetRemoteVoicePosition(_players[i].AgoraClientId, pan, 0);
                    continue;
                }

                if (_players[i].MaxValue)
                {
                    _agoraVoice.GetAudioEffectManager().SetRemoteVoicePosition(_players[i].AgoraClientId, pan, 100);
                    continue;
                }
            }

            float distanceToPlayer = Vector3.Distance(ownerCamera.transform.position, otherPlayerHeadTransform.position);
            float gain = GetGainByPlayerDistance(distanceToPlayer);
          
            _agoraVoice.GetAudioEffectManager().SetRemoteVoicePosition(_players[i].AgoraClientId, pan,gain);
        }
    }

    /// <summary>
    /// Maximum value 1
    /// Minimum value -1
    /// </summary>
    /// <param name="otherPlayerHeadTransform"></param>
    /// <param name="ownerHeadTransform"></param>
    /// <returns></returns>
    private float GetPanByPlayerOrientation(Transform otherPlayerHeadTransform, Transform ownerHeadTransform)
    {
        Vector3 directionToRemotePlayer = otherPlayerHeadTransform.position - ownerHeadTransform.position;
        directionToRemotePlayer.Normalize();
        
        float pan = Vector3.Dot(ownerHeadTransform.right, directionToRemotePlayer);
        
        return pan;
    }

    /// <summary>
    /// Maximum value 100
    /// Minimum value 0
    /// </summary>
    /// <param name="distanceToPlayer"></param>
    /// <returns></returns>
    private float GetGainByPlayerDistance(float distanceToPlayer)
    {
        if (distanceToPlayer >= MAX_CHAT_DISTANCE)
        {
            return 0;
        }
        if (distanceToPlayer < MAX_CHAT_PROXIMITY)
        {
            distanceToPlayer = MAX_CHAT_PROXIMITY;
        }

        //float gain = (distanceToPlayer - 5) / (MAX_CHAT_PROXIMITY - 5);
        float gain = (1 - (distanceToPlayer / 10)) * 100f;
        //gain *= 100;
        if (gain < 20)
            gain = 20;
        return gain;
    }
    
    private void DeletePlayer(uint uid)
    {
        if (!IsOwner) return;

        for (int i = 0; i < _players.Count; i++)
        {
            if (uid == _players[i].AgoraClientId)
            {
                _players.RemoveAt(i);
                break;
            }
        }
    }
    
    #endregion

    #region Coroutines

    private IEnumerator AddNetworkPlayerToSpatialList()
    {
        if (IsOwner == false)
        {
            yield break;
        }
        yield return new WaitForSeconds(2f);
        SpatialAudio[] otherPlayers = FindObjectsOfType<SpatialAudio>();
        foreach (SpatialAudio item in otherPlayers)
        {
            if (item == this || item == null)
            {
                continue;
            }

            uint agoraNetworkID = item.networkedUID.Value;
            ulong networkID = item.OwnerClientId;
            float networkTimer = 2f;
            float networkWaitTime = 0;

            while (agoraNetworkID == 0)
            {
                agoraNetworkID = item.networkedUID.Value;
                networkID = item.OwnerClientId;

                networkWaitTime += Time.deltaTime;
                if (networkWaitTime >= networkTimer)
                {
                    Debug.LogError($"{item.name} not found network ID");
                    break;
                }

                yield return new WaitForSeconds(0.5f);

                // Object could be destroyed while we waited
                if (item == null)
                {
                    break;
                }
            }

            if (item == null)
            {
                continue;
            }

            bool isPlayerAlreadyInList = false;

            foreach (var player in _players)
            {
                if (agoraNetworkID == player.AgoraClientId)
                {
                    isPlayerAlreadyInList = true;
                }
            }

            if (!isPlayerAlreadyInList)
            {
                if (item.gameObject.TryGetComponent(out Assets.Scripts.Shared.NetworkPlayer networkPlayer))
                {
                    if (_players.Exists(element=>element.NetworkClientId == networkID))
                    {
                        var tempClient =_players.Find(e => e.NetworkClientId == networkID);
                        tempClient.AgoraClientId = agoraNetworkID;
                    }
                    else
                    {
                        string userGuid = "";
                        if (item.gameObject.TryGetComponent(out ClientIdView clientIdView))
                        {
                            userGuid = clientIdView.PlayerAccountId;
                        }
                        _players.Add(new UserHead(userGuid, networkID, agoraNetworkID, networkPlayer));
                    }
                }
                else
                {
                    Debug.LogError("Can't get spatial sound player");
                }
            }
        }
    }

    private IEnumerator AddNetworkUid(uint id)
    {
        int i = 0;
        while (networkedUID.Value == 0)
        {
            SetUidServerRpc(id);
            if (i > 0) Debug.LogError($"add networkUID, i = {i}");
            i++;
            yield return new WaitForSeconds(0.3f);
        }
    }
    #endregion
}