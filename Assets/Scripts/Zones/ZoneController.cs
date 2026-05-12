using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Audio;
using Assets.Scripts.Zones;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZoneController : NetworkBehaviour
{
    //events for ZoneView subscription
    public event Action<ulong,string, bool> AdminChanged; //bool: isAdmin = true
    public event Action<Dictionary<ulong,string>, bool> ParticipantChanged; //bool: isAdmin = true
    public event Action<bool> MuteChanged;
    public event Action<bool> LockChanged;

    [SerializeField] private string channelName;
    [SerializeField] private ZoneActivator _zoneActivator;
    [SerializeField] private Canvas _zoneCanvas;

    [SerializeField] private List<Place> _teleportPoints;
    [SerializeField] private List<TeleportationArea> _teleportAreas;
    [SerializeField] private bool _isPrivate;

    [SerializeField] private GameObject _empty;
    [SerializeField] private GameObject _admined;
    [SerializeField] private GameObject _closed;

    [SerializeField] private ZoneController _kickZoneDefault;
    [SerializeField]private SerializableZoneClientDictionary _participantsNameDictionary = new();
    [SerializeField] private NetworkList<ulong> _participantsId;
    [HideInInspector] private NetworkVariable<ulong> _adminId;
    [HideInInspector] private NetworkVariable<bool> _isMute;
    [HideInInspector] private NetworkVariable<bool> _isLocked;
    [HideInInspector] public ZonesModel ZonesModel;

    public bool IsMuted => _isMute != null && _isMute.Value;
    public string PrivateChannelName => channelName;
    public bool IsLocked => _isLocked != null && _isLocked.Value;

    public bool IsLocalUserAdmin => _adminId != null && ZonesModel != null
                                                     && _adminId.Value == ZonesModel.OwnerId;

    private void Awake()
    {
        _participantsId =
            new NetworkList<ulong>(); //workaround for "NetworkList - UnityException: EditorPrefsGetInt is not allowed to be called"
        _adminId = new NetworkVariable<ulong>();
        _isMute = new NetworkVariable<bool>();
        _isLocked = new NetworkVariable<bool>();

        ZonesModel = GetComponentInParent<ZonesModel>();
        foreach (var item in _teleportPoints)
        {
            item.Init(ZonesModel, this);
        }
    }


    [ContextMenu(nameof(ShowAllUsersInZone))]
    private void ShowAllUsersInZone()
    {
        Debug.Log($"admin id is :{_adminId.Value}");
        Debug.Log($"local client id is :{NetworkManager.Singleton.LocalClientId}");
        foreach (ulong userId in _participantsId)
        {
            Debug.Log(userId);
        }
    }

    public override void OnNetworkSpawn()
    {
        _adminId.OnValueChanged += OnChangeAdminId;
        _participantsId.OnListChanged += ParticipantOnListChanged;
        _isMute.OnValueChanged += MuteChange;
        _isLocked.OnValueChanged += OnLockChanged;

        MuteChanged?.Invoke(_isMute.Value);
        LockChanged?.Invoke(_isLocked.Value);
        SwitchZoneActivator(_adminId.Value == 0);

        _zoneCanvas.renderMode = RenderMode.WorldSpace;
        _zoneCanvas.worldCamera = Camera.main;

        #region ClientEvents

        //for private point teleports
        for (int i = 0; i < _teleportPoints.Count; i++)
        {
            var place = _teleportPoints[i];
            place.SetPlaceNumber(i);
            place.Teleporting += ChangeZone;
            place.HoverEntered += Place_OnHoverEnter;
            place.HoverExited += Place_OnHoverExit;
        }

        //for non private areas
        foreach (var area in _teleportAreas)
        {
            area.teleporting.AddListener((args) =>
            {
                ChangeZone();
                if (ZonesModel.Place != null)
                    ZonesModel.Place
                        .LeavePlace(); //Free previous place, but not occupy Area place, because it's for multi using
            });
        }

        #endregion

        #region ServerEvents

        if (NetworkEventsManager.Singleton != null)
        {
            NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        #endregion
    }

    public override void OnDestroy()
    {
        _participantsId.OnListChanged -= ParticipantOnListChanged;
        _isMute.OnValueChanged -= MuteChange;
        _isLocked.OnValueChanged -= OnLockChanged;

        foreach (var place in _teleportPoints)
        {
            place.Teleporting -= ChangeZone;
            place.HoverEntered -= Place_OnHoverEnter;
            place.HoverExited -= Place_OnHoverExit;
        }

        base.OnDestroy();
    }

    public bool UserInZone(ulong userId)
    {
        return _participantsId.Contains(userId);
    }

    private void ParticipantOnListChanged(NetworkListEvent<ulong> changeEvent)
    {
        GetParticipantsNameServerRpc();
    }
    
    private void OnChangeAdminId(ulong previousValue, ulong newValue)
    {
        if (IsServer)
        {
            Debug.Log($"Admin zone \"{gameObject.name}\" now: {newValue}");
        }

        NotifyParticipantsChanged();

        if (_zoneActivator != null)
            _zoneActivator.HidePanel();

        RefreshZoneActivator();
    }

    public bool IsMeAdmin()
    {
        return _adminId.Value == ZonesModel.OwnerId;
    }

    private void NotifyParticipantsChanged()
    {
        List<ulong> participants = new List<ulong>();

        foreach (var participant in _participantsId)
        {
            participants.Add(participant);
        }

        //if (ZonesModel.Agora != null && ZonesModel.Agora.GetCurrentPlayerSpatialAudio() != null)
        //{
        //    ulong scriptInvoker = ZonesModel.Agora.GetCurrentPlayerSpatialAudio().OwnerClientId;
        //    
        //    if(scriptInvoker == NetworkManager.Singleton.LocalClientId)
        //        ExcludeZoneClients(participants, _adminId.Value);
        //}

        if(_participantsNameDictionary.zoneClients !=null)
            ParticipantChanged?.Invoke(_participantsNameDictionary.zoneClients, _adminId.Value == ZonesModel.OwnerId);

        AdminChanged?.Invoke(_adminId.Value,ZonesModel.GetPlayerName(), _adminId.Value == ZonesModel.OwnerId);
    }

    private void ExcludeZoneClients(List<ulong> userClients, ulong adminId)
    {
        ulong currentUserId = NetworkManager.Singleton.LocalClientId;

        if (userClients.Contains(currentUserId) ||
            NetworkManager.Singleton.LocalClientId == adminId)
        {
            if (ZonesModel.Agora != null && ZonesModel.Agora.GetCurrentPlayerSpatialAudio() != null)
                ZonesModel.Agora.GetCurrentPlayerSpatialAudio().RemoveAllExcludeClients();
            
            foreach (ulong userClient in userClients)
            {
                if (currentUserId == userClient) continue;
                if (ZonesModel.Agora != null && ZonesModel.Agora.GetCurrentPlayerSpatialAudio() != null)
                    ZonesModel.Agora.GetCurrentPlayerSpatialAudio().MaxVolume(userClient);
            }
            if (currentUserId != adminId &&ZonesModel.Agora != null && ZonesModel.Agora.GetCurrentPlayerSpatialAudio() != null)
                ZonesModel.Agora.GetCurrentPlayerSpatialAudio().MaxVolume(adminId);
        }
    }

    private void MuteChange(bool oldValue, bool newValue)
    {
        MuteChanged?.Invoke(newValue);
    }

    private void OnLockChanged(bool oldValue, bool newValue)
    {
        LockChanged?.Invoke(newValue);

        // TODO: add lock barrier to visualize locked state
        //_lockedTop.gameObject.SetActive(newValue);
    }

    private void RefreshZoneActivator()
    {
        if (_adminId.Value == 0)
        {
            SwitchZoneActivator(true);
            ZoneActivatorCanInteract(false);
        }
        else
        {
            ZoneActivatorCanInteract(ZonesModel.OwnerId == _adminId.Value);
            SwitchZoneActivator(ZonesModel.OwnerId == _adminId.Value);
        }
    }

    #region ClientMethods
    private void Place_OnHoverExit()
    {
        foreach (var teleport in _teleportPoints)
        {
            teleport.HideSignal();
        }
    }

    private void Place_OnHoverEnter()
    {
        foreach (var teleport in _teleportPoints)
        {
            teleport.ShowSignal();
        }
    }
    
    private void ChangeZone()
    {
        if (!IsClient) return;

        if (ZonesModel.ZoneController != null) //leave previous zone
        {
            if (ZonesModel.ZoneController == this && _isPrivate) //the second condition is that the return is not triggered in common areas
            {
                RefreshZoneActivator();
                return;
            }
            ZonesModel.ZoneController.LeaveZoneClient(ZonesModel.OwnerId);
        }

        ZonesModel.SetZone(this); //new zone

        JoinZoneServerRPC(ZonesModel.OwnerId,ZonesModel.GetPlayerName());
    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinZoneServerRPC(ulong id, string participantName)
    {
        if (_adminId.Value == id)
        {
            return;
        }

        if (_adminId.Value == 0)
        {
            _adminId.Value = id;
            
            if(!_participantsNameDictionary.zoneClients.ContainsKey(id))
                _participantsNameDictionary.zoneClients.Add(id,participantName);
            SerParticipantsNameClientRpc(_participantsNameDictionary);
        }
        else
        {
            if (_isLocked.Value)
            {
                KickParticipantServerRPC(id);
                return;
            }

            if(!_participantsNameDictionary.zoneClients.ContainsKey(id))
                _participantsNameDictionary.zoneClients.Add(id,participantName);
                
            if (!_participantsId.Contains(id))
            {
                _participantsId.Add(id);
            }
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveZoneServerRPC(ulong id)
    {
        LeaveZoneServer(id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetParticipantsNameServerRpc()
    {
        SerParticipantsNameClientRpc(_participantsNameDictionary);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeAdminServerRPC(ulong id,string userName)
    {
        if (_adminId.Value == id)
        {
            return;
        }

        if(!_participantsNameDictionary.zoneClients.ContainsKey(id))
            _participantsNameDictionary.zoneClients.Add(id,userName);
        
        if (_adminId.Value != 0)
        {
            _participantsId.Add(_adminId.Value);
        }
        _adminId.Value = id;
        
        _participantsId.Remove(id);
    }

    private void LeaveZoneClient(ulong id) //works on previous zone
    {
        if (!IsClient) return;
        if (ZonesModel != null && ZonesModel.Agora != null)
        {
            if(ZonesModel.Agora.TryGetComponent(out AgoraAudioService audioService))
            {
                if (audioService.MutedUsersID.ContainsKey(id))
                {
                    audioService.UnmuteUser(id);
                }
            }
        }

        LeaveZoneServerRPC(id);
    }

    private bool LeaveZoneServer(ulong id)
    {
        if (_adminId.Value == id) //admin from this zone left
        {
            if(_participantsNameDictionary.zoneClients.ContainsKey(id))
                _participantsNameDictionary.zoneClients.Remove(id);

            if (_participantsId.Count > 0)
            {
                _adminId.Value = _participantsId[0]; //directly to network variable because this is server method
                
                _participantsId.RemoveAt(0); //directly to network variable because this is server method
            }
            else
            {
                _isMute.Value = false;
                _adminId.Value = 0; //directly to network variable because this is server method
            }
            _isLocked.Value = false;
            
            return true;
        }
        else
        {
            if(_participantsNameDictionary.zoneClients.ContainsKey(id))
                _participantsNameDictionary.zoneClients.Remove(id);
            
            return _participantsId.Remove(id); //non admin from this zone left
        }
    }

    private void ZoneActivatorCanInteract(bool isInteract) 
    {
        if (_zoneActivator == null) return;

        _zoneActivator.CanInteract = isInteract;
    }

    private void SwitchZoneActivator(bool isVisible)
    {
        if (!_isPrivate) return;
        
        if (_zoneActivator == null) return;

        if (isVisible && ZonesModel != null && ZonesModel.PlayerOwner != null)
            _zoneActivator.SetLookTransform(ZonesModel.PlayerOwner.transform);
        
        _zoneActivator.gameObject.SetActive(isVisible);
    }

    private void FixedUpdate()
    {
        if (_isPrivate)
        {
            //temp
            if (IsClient)
            {
                if (_adminId.Value == 0)
                {
                    _admined.SetActive(false);
                    _empty.SetActive(true);
                }
                else
                {
                    _admined.SetActive(true);
                    _empty.SetActive(false);
                }
                //SwitchZoneActivator(ZonesModel.OwnerId == _adminId.Value);
            }

            if (IsServer) //check and remove disconnected users
            {
                if (_adminId.Value != 0 && !NetworkManager.Singleton.ConnectedClientsIds.Contains<ulong>(_adminId.Value)) OnClientDisconnect(_adminId.Value);

                if (_participantsId.Count > 0)
                {
                    foreach (var participantId in _participantsId)
                    {
                        if (!NetworkManager.Singleton.ConnectedClientsIds.Contains<ulong>(participantId)) OnClientDisconnect(participantId);
                    }
                }
            }
        }
    }

    //method to call from ZoneView
    public void ChangeAdmin(ulong id, string adminName = default)
    {
        if (string.IsNullOrEmpty(adminName))
            adminName = ZonesModel.GetPlayerName();
        
        ChangeAdminServerRPC(id,adminName);
    }

    [ClientRpc]
    public void KickParticipantClientRpc(ulong id)
    {
        if (id == ZonesModel.OwnerId)
        {
            ZonesModel.PlayerOwner.SetPositionToZero();
            ZonesModel.Place.LeavePlace();
            ZonesModel.SetZone(_kickZoneDefault);
        }
    }

    [ClientRpc]
    private void SerParticipantsNameClientRpc(SerializableZoneClientDictionary zoneClientDictionary)
    {
        _participantsNameDictionary = zoneClientDictionary;
        NotifyParticipantsChanged();
    }

    public List<ulong> GetAllParticipantsId()
    {
        List<ulong> participants = new();

        if (_adminId != null && _adminId.Value != 0) participants.Add(_adminId.Value);
        if (_participantsId != null && _participantsId.Count > 0)
        {
            foreach (var participant in _participantsId)
            {
                participants.Add(participant);
            }
        }
        return participants;
    }

    #endregion

    #region ServerMethods
    /// <summary>
    /// only for disconnect from server event
    /// </summary>
    /// <param name="playerId"></param>
    public void OnClientDisconnect(ulong id)
    {
        //TODO: Optimization is needed because every zone controller try to remove this client
        if (!IsServer)
        {
            return;
        }

        if (LeaveZoneServer(id))
        {
            Debug.Log($"User disconnected. Removed from zone {gameObject.name}.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void KickParticipantServerRPC(ulong id)
    {
        LeaveZoneServerRPC(id);       
        KickParticipantClientRpc(id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeAudioChannelServerRpc(bool isMute)
    {
        _isMute.Value = isMute;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeLockedServerRpc(bool isLocked)
    {
        _isLocked.Value = isLocked;
    }
    #endregion

    #region InnerClass

    public class SerializableZoneClientDictionary: INetworkSerializable
    {
        public Dictionary<ulong, string> zoneClients = new ();
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                int count = 0;
                if (zoneClients == null)
                {
                    serializer.SerializeValue(ref count);
                    return;
                }
                count = zoneClients.Count;
                serializer.SerializeValue(ref count);
                foreach (var zoneClient in zoneClients)
                {
                    ulong clientId = zoneClient.Key;
                    string clientName = zoneClient.Value;
                    serializer.SerializeValue(ref clientId);
                    serializer.SerializeValue(ref clientName);
                    
                }
            }
            else
            {
                zoneClients = new();
                int count = 0;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    ulong clientId = 0;
                    string clientName = string.Empty;
                    serializer.SerializeValue(ref clientId);
                    serializer.SerializeValue(ref clientName);
                    zoneClients[clientId] = clientName;
                }
            }
        }
    }

    #endregion
}
