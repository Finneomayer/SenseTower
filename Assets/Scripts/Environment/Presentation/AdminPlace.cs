using System;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Environmental.Presentation.Browser;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Management;

public class AdminPlace : NetworkBehaviour
{
    private const ulong NotHasSetAdminId = 0;

    #region Inspector
    public NetworkVariable<FixedString128Bytes> LogoTowerObjectId;
    [SerializeField] private PlaceAdminService _placeAdminService;
    [SerializeField] private PlaceAdminService _secondPlaceAdminService;

    [SerializeField] private NetworkVariable<ulong> _occupiedID = new NetworkVariable<ulong>(NotHasSetAdminId);

    #endregion

    public event Action<ulong, ulong> AdminChange;

    private bool _canUserToogle = true;

    private void Start()
    {
#if UNITY_SERVER
        NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        return;
#endif
        if (_secondPlaceAdminService != null)
        {
            if (Application.platform == RuntimePlatform.Android
                || (XRGeneralSettings.Instance != null
                    && XRGeneralSettings.Instance.Manager != null
                    && XRGeneralSettings.Instance.Manager.isInitializationComplete))
            {
                _secondPlaceAdminService.gameObject.SetActive(false);
                _secondPlaceAdminService = null;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        SubscribeEvents();

        if (IsAdminSet())
            OnValueChanged(0, _occupiedID.Value);
    }

    public void SetInteractionObject(GameObject interactionGameObject)
    {
        if (_placeAdminService != null)
        {
            _placeAdminService.Show();
            _placeAdminService.SetInteractableObject(interactionGameObject);

        }
        if (_secondPlaceAdminService != null)
        {
            _secondPlaceAdminService.Show();
            _secondPlaceAdminService.SetInteractableObject(interactionGameObject);

        }
    }

    public void SetAdmin(ulong clientId)
    {
        SetAdminServerRpc(clientId);
    }

    public void CheckAdmin()
    {
        if (IsAdminSet())
            OnValueChanged(0, _occupiedID.Value);
    }

    public override void OnNetworkDespawn()
    {
        UnsubscribeEvents();
    }

    public void ToogleManageAdminPlaceByUser(bool value)
    {
        _canUserToogle = value;

        if (!_canUserToogle)
            DeactivateAdminPlace();
    }

    public void ClearAdmin()
    {
        ClearAdminServerRpc(NetworkManager.LocalClientId);
    }

    public void ShowAdminPlace()
    {
        _placeAdminService.Show();

        if (_secondPlaceAdminService != null)
            _secondPlaceAdminService.Show();
    }

    public void HideAdminPlace()
    {
        _placeAdminService.Hide();

        if (_secondPlaceAdminService != null)
            _secondPlaceAdminService.Hide();
    }

    public void DeactivateAdminPlace()
    {
        _placeAdminService.Deactivate();

        if (_secondPlaceAdminService != null)
            _secondPlaceAdminService.Deactivate();
    }

    public void ActivateAdminPlace()
    {
        if (_canUserToogle)
            _placeAdminService.Activate();

        if (_secondPlaceAdminService != null)
            _secondPlaceAdminService.Activate();
    }

    public bool IsAdminSet()
    {
        return _occupiedID.Value != NotHasSetAdminId;
    }

    public bool IsUserAdmin(ulong clientId)
    {
        return _occupiedID.Value == clientId;
    }

    private void SubscribeEvents()
    {
        _placeAdminService.SetAdmin += OnSetAdminEventInvoke;
        _placeAdminService.ClearAdmin += OnClearAdminEventInvoke;
        _placeAdminService.Activate();

        if (_secondPlaceAdminService != null)
        {
            _secondPlaceAdminService.SetAdmin += OnSetAdminEventInvoke;
            _secondPlaceAdminService.ClearAdmin += OnClearAdminEventInvoke;
            _secondPlaceAdminService.Activate();
        }

        _occupiedID.OnValueChanged += OnValueChanged;
    }

    private void UnsubscribeEvents()
    {
        _placeAdminService.SetAdmin -= OnSetAdminEventInvoke;
        _placeAdminService.ClearAdmin -= OnClearAdminEventInvoke;
        _placeAdminService.Deactivate();

        if (_secondPlaceAdminService != null)
        {
            _secondPlaceAdminService.SetAdmin -= OnSetAdminEventInvoke;
            _secondPlaceAdminService.ClearAdmin -= OnClearAdminEventInvoke;
            _secondPlaceAdminService.Deactivate();
        }

        _occupiedID.OnValueChanged -= OnValueChanged;

        if (IsServer)
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    #region Events

    private void OnValueChanged(ulong previousValue, ulong newValue)
    {
        AdminChange?.Invoke(previousValue, newValue);
        if (newValue == NetworkManager.Singleton.LocalClientId)
        {
            _placeAdminService.Hide();

            if (_secondPlaceAdminService != null)
                _secondPlaceAdminService.Hide();
        }

        if (previousValue == NetworkManager.Singleton.LocalClientId)
        {
            _placeAdminService.Show();

            if (_secondPlaceAdminService != null)
                _secondPlaceAdminService.Show();
        }

        if (!_canUserToogle) return;

        if (IsAdminSet())
            DeactivateAdminPlace();
        else
            ActivateAdminPlace();
    }

    private void OnSetAdminEventInvoke(ulong clientId)
    {
        if (clientId == _occupiedID.Value)
        {
            _placeAdminService.Deactivate();

            if (_secondPlaceAdminService != null)
                _secondPlaceAdminService.Deactivate();

            AdminChange?.Invoke(0, clientId);
            return;
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            if (clientId != _occupiedID.Value)
                SetAdminServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnClearAdminEventInvoke(ulong clientId)
    {
        ClearAdminServerRpc(clientId);
    }

    /// <summary>
    /// Invoke On server
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnect(ulong clientId)
    {
        if (_occupiedID.Value == clientId)
        {
            _occupiedID.Value = NotHasSetAdminId;
        }
    }

    #endregion

    #region Server

    [ServerRpc(RequireOwnership = false)]
    private void SetAdminServerRpc(ulong clientId)
    {
        if (IsAdminSet())
            return;

        _occupiedID.Value = clientId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClearAdminServerRpc(ulong clientId)
    {
        if (clientId == _occupiedID.Value)
        {
            _occupiedID.Value = NotHasSetAdminId;
        }
    }

    #endregion
}