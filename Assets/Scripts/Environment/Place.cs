using System;
using System.Collections;
using System.Linq;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Player;
using Assets.Scripts.Zones;
using Sense.Interectable.Teleportation;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class Place : NetworkBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public NetworkVariable<ulong> IsOccupiedID = new NetworkVariable<ulong>(0);
    [SerializeField] private GameObject FreeSelectedSignal;
    [SerializeField] private GameObject FreeSignal;
    [SerializeField] private GameObject FreeSelectedSignalCircle;

    [Header("Signal shifting anchors:")] [SerializeField]
    private Transform TopPoint;

    [SerializeField] private Transform BottomPoint;

    [Header("Switchable components:")] [SerializeField]
    private Collider _collider;

    [SerializeField] private GameObject _placeView;

    [Header("Debug info:")] [SerializeField]
    private bool DebugEnabled;

    [SerializeField] private GameObject DebugObjectsParent;
    [SerializeField] private TextMeshProUGUI PlaceLabel;
    [SerializeField] private ChairTransformController _chairTransformController;

    public CustomTeleportationAnchor TeleportAnchor => _teleport;
    public bool IsOccupiedByLocalUser => NetworkManager != null && IsOccupiedID.Value == NetworkManager.LocalClientId;
   
    public event Action Teleporting;
    public event Action HoverEntered;
    public event Action HoverExited;
    public event Action<PlayerLogic> OccupiedByLocalPlayer;
    public event Action UnoccupiedByLocalPlayer;
    public static event Action<Place> PlaceSpawned;
    public static event Action<Place> PlaceDespawned;

    private CustomTeleportationAnchor _teleport;
    private ZonesModel _zonesModel;
    private ZoneController _zoneController;
    private bool _isCurrentPlace;
    private bool _isShowSignal = false;

    private Coroutine _setOccupiedIdRoutine;

    private void Awake()
    {
        _teleport = GetComponent<CustomTeleportationAnchor>();
    }

    private void OnEnable()
    {
        #region ClientEvents

        _teleport.teleporting.AddListener(OnTeleporting);
        _teleport.hoverEntered.AddListener(OnHoverEnter);
        _teleport.hoverExited.AddListener(OnHoverExit);

        #endregion

        #region ServerEvents

        if (NetworkEventsManager.Singleton != null)
        {
            NetworkEventsManager.Singleton.OnClientDisconnectCallback += LeavePlaceServer;
        }

        #endregion        
    }

    private void OnDisable()
    {
        #region ClientEvents

        _teleport.teleporting.RemoveListener(OnTeleporting);
        _teleport.hoverEntered.RemoveListener(OnHoverEnter);
        _teleport.hoverExited.RemoveListener(OnHoverExit);

        #endregion

        #region ServerEvents

        if (NetworkEventsManager.Singleton != null)
        {
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= LeavePlaceServer;
        }

        #endregion       
    }

    private void Start()
    {
        DebugObjectsParent.SetActive(DebugEnabled);
    }

    public void SwitchPlace(bool flag)
    {
        if (_collider != null) _collider.enabled = flag;
        if (_placeView != null) _placeView.SetActive(flag);
    }

    public void SwitchPlaceCollider(bool flag)
    {
        if (_collider != null) _collider.enabled = flag;
    }

    public override void OnNetworkSpawn()
    {
#if !UNITY_SERVER
        PlaceSpawned?.Invoke(this);
#endif
        
        BindChairServerRpc(IsOccupiedID.Value);
    }

    public override void OnNetworkDespawn()
    {
#if !UNITY_SERVER
        PlaceDespawned?.Invoke(this);
#endif
    }

    public void Init(ZonesModel zonesModel, ZoneController zoneController)
    {
        if (_zonesModel == null)
            _zonesModel = zonesModel;

        if (_zoneController == null)
            _zoneController = zoneController;
    }

    #region ClientMethods

    /// <summary>
    /// for area place (only leave previous place, but not occupying new area place) because it's common
    /// </summary>
    public void LeavePlace()
    {
        if (_zonesModel.Place != null)
        {
            _zonesModel.Place.SetFreeServer();
            _zonesModel.RestorePlayerPositionAndRotation();
        }
    }

    [ContextMenu("Seat on Chair")]
    public void SetPlaceOnEditor()
    {
        LeaveAndOccupyPlace();
    }

    /// <summary>
    /// for point place (leave previous and occupy new one)
    /// </summary>
    /// <param name="clientPlace">for getting old place</param>
    public void LeaveAndOccupyPlace()
    {
        LeaveAndOccupyPlaceExternal(this);
    }

    /// <summary>
    /// change place from Extendable zone
    /// </summary>
    /// <param name="newPlace"></param>
    public void LeaveAndOccupyPlaceExternal(Place newPlace)
    {
        if (_zonesModel == null)
        {
            _zonesModel = FindObjectOfType<ZonesModel>();
        }

        if (_zonesModel.Place == null)
        {
            _zonesModel.SavePlayerPositionAndRotation();
        }

        if (_zonesModel.Place != null) _zonesModel.Place.SetFreeServer(); //old place
        _zonesModel.SetPlace(newPlace);

        _zonesModel.Place.SetOccupiedServer(NetworkManager.LocalClientId); //this place
    }

    /// <summary>
    /// translates from client to server
    /// </summary>
    /// <param name="playerId"></param>
    public void SetOccupiedServer(ulong id)
    {
        if (_setOccupiedIdRoutine != null)
        {
            StopCoroutine(_setOccupiedIdRoutine);
        }

        _setOccupiedIdRoutine = StartCoroutine(SetOccupiedIdRoutine(id));

        if (_zonesModel.Place != null)
        {
            OccupiedByLocalPlayer?.Invoke(_zonesModel.PlayerOwner);
        }
        else
        {
            UnoccupiedByLocalPlayer?.Invoke();
        }
    }

    /// <summary>
    /// translates from client to server
    /// </summary>
    public void SetFreeServer() //to set free previous player place (works on previous place)
    {
        _zonesModel.SetPlace(null);
        SetOccupiedServer(0);
    }

    private void FixedUpdate()
    {
        if (IsClient)
        {
            _teleport.enabled = IsOccupiedID.Value == 0 && IsPlaceZoneAvailableForLocalUser();
            if (DebugEnabled)
            {
                DebugUpdate();
            }
        }

        if (IsServer) //protection from occupying place by user witch is offline 
        {
            if (IsOccupiedID.Value != 0 && !NetworkManager.ConnectedClientsIds.Contains(IsOccupiedID.Value))
            {
                LeavePlaceServer(IsOccupiedID.Value);
            }
        }
    }

    private void DebugUpdate()
    {
        SetLabelOccupied(IsOccupiedID.Value != 0);
    }

    public void ShowSignal()
    {
        if (!IsClient) return;
        _isShowSignal = true;
        if (!_teleport.enabled)
        {
            FreeSignal.SetActive(false);
            FreeSelectedSignal.SetActive(false);
        }
        else
        {
            if (_isCurrentPlace)
            {
                FreeSignal.SetActive(false);
                FreeSelectedSignal.SetActive(true);
                FreeSelectedSignalCircle.SetActive(true);
            }
            else
            {
                FreeSignal.SetActive(true);
                FreeSelectedSignal.SetActive(false);
                FreeSelectedSignalCircle.SetActive(false);
                FreeSelectedSignal.transform.position = TopPoint.position;
            }
        }
    }

    public void HideSignal()
    {
        _isShowSignal = false;
        if (gameObject.activeInHierarchy) StartCoroutine(HideSignalDelayedCoroutine());
    }

    public void SetPlaceNumber(int number)
    {
        PlaceLabel.text = number.ToString();
    }

    private bool IsPlaceZoneAvailableForLocalUser()
    {
        if (_zonesModel == null || _zoneController == null) return true;

        if (_zonesModel.ZoneController == _zoneController)
        {
            return true;
        }

        if (_zoneController != null && _zoneController.IsLocked)
        {
            return false;
        }

        if (_zonesModel.ZoneController != null && _zonesModel.ZoneController.IsLocked)
        {
            return _zonesModel.ZoneController.IsLocalUserAdmin;
        }

        return true;
    }

    private void SetLabelOccupied(bool isOccupied)
    {
        PlaceLabel.color = isOccupied ? Color.red : Color.green;
    }

    private IEnumerator HideSignalDelayedCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        if (!_isShowSignal)
        {
            FreeSignal.SetActive(false);
            FreeSelectedSignal.SetActive(false);
            FreeSelectedSignalCircle.SetActive(false);
        }
    }

    #endregion

    #region ServerMethods

    /// <summary>
    /// only for disconnect from server event
    /// </summary>
    /// <param name="clientId"></param>
    private void LeavePlaceServer(ulong clientId)
    {
        if (!IsServer) return;
        if (IsOccupiedID.Value == clientId) //if disconnected player occupying THIS place
        {
            Debug.Log($"User disconnected. Removed from place {PlaceLabel.text}.");
            IsOccupiedID.Value = 0;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetServerRPC(ulong requestingPlayerID, ulong newOccupiedPlayerID)
    {
        // User cannot occupy someone else's place.
        // Two users can teleport on the same place almost simultaneously.
        // In that case we will kick the user who try to set IsOccupiedID value later than the first
        if (IsOccupiedID.Value != 0 && requestingPlayerID != IsOccupiedID.Value)
        {
            if (newOccupiedPlayerID == requestingPlayerID)
            {
                LeaveOccupiedEarlierPlaceClientRpc(requestingPlayerID);
            }

            return;
        }

        IsOccupiedID.Value = newOccupiedPlayerID;

        NetworkObject clientNetworkObject = GetNetworkClient(newOccupiedPlayerID);
        if (clientNetworkObject != null)
            SendOccupiedClientClientRpc(clientNetworkObject);
        else
            UnBindChairClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BindChairServerRpc(ulong newValue)
    {
        NetworkObject clientNetworkObject = GetNetworkClient(newValue);
        if (clientNetworkObject != null)
            SendOccupiedClientClientRpc(clientNetworkObject);
        else
            UnBindChairClientRpc();
    }

    private NetworkObject GetNetworkClient(ulong playerID)
    {
        NetworkObject clientNetworkObject = null;
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (playerID == client.Key)
            {
                clientNetworkObject = client.Value.PlayerObject;
            }
        }

        return clientNetworkObject;
    }

    [ClientRpc]
    private void SendOccupiedClientClientRpc(NetworkObjectReference placeOccupiedClientObject)
    {
        if (placeOccupiedClientObject.TryGet(out NetworkObject client))
        {
            SetChairTransformTarget(client.transform);
        }
    }


    [ClientRpc]
    private void LeaveOccupiedEarlierPlaceClientRpc(ulong playerID)
    {
        SetChairTransformTarget(null);
        if (NetworkManager.LocalClientId != playerID)
        {
            return;
        }

        if (_zonesModel.ZoneController != null)
        {
            _zonesModel.ZoneController.KickParticipantServerRPC(NetworkManager.LocalClientId);
        }
        else if (_zonesModel.PlayerOwner != null)
        {
            _zonesModel.PlayerOwner.SetPositionToZero();
            LeavePlace();
        }
    }

    [ClientRpc]
    private void UnBindChairClientRpc()
    {
        SetChairTransformTarget(null);
    }

    private void SetChairTransformTarget(Transform target)
    {
        if (_chairTransformController == null) return;

        _chairTransformController.SetTarget(target);
    }

    private IEnumerator SetOccupiedIdRoutine(ulong id)
    {
        // Waiting some time because user can make immediate teleport to other place,
        // so we can interrupt this changing of IsOccupiedID on time
        yield return new WaitForSeconds(0.1f);

        if (id != 0)
        {
            Teleporting?.Invoke();
        }

        SetServerRPC(NetworkManager.LocalClientId, id);
        _setOccupiedIdRoutine = null;
    }

    #endregion

    private void HandleHoverEnter()
    {
        if (_zonesModel != null && !_zonesModel.IsTeleportingAllowed)
        {
            return;
        }
        _isCurrentPlace = true;
        HoverEntered?.Invoke();
        ShowSignal();
    }

    private void HandleHoverExit()
    {
        _isCurrentPlace = false;
        HoverExited?.Invoke();
        HideSignal();
    }

    private void HandleTeleporting()
    {
        if (!_teleport.enabled)
        {
            return;
        }
        if (_zonesModel != null && !_zonesModel.IsTeleportingAllowed)
        {
            return;
        }
        LeaveAndOccupyPlace();
    }

    private void OnTeleporting(TeleportingEventArgs args)
    {       
        HandleTeleporting();
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        HandleHoverEnter();
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        HandleHoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            HandleTeleporting();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            HandleHoverEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            HandleHoverExit();
        }
    }
}