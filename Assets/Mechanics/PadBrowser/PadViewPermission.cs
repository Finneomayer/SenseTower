using Assets.Mechanics.Browser;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Netcode;
using UnityEngine;

public class PadViewPermission : NetworkBehaviour
{
    [SerializeField] private Browser _browser;
    [SerializeField] private BrowserAdminControlPanel _browserAdminPanel;
    [SerializeField] private AdminPlace _adminPlace;

    public NetworkVariable<bool> IsVisible = new(false);

    public bool IsVisibleToOthers => IsVisible != null && IsVisible.Value;

    public event Action VisibilityStateChanged;

    private void OnEnable()
    {
        _browserAdminPanel.PadClickVisible += SetPadVisibleState;
    }

    private void OnDisable()
    {
        _browserAdminPanel.PadClickVisible -= SetPadVisibleState;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsClient)
        {
            return;
        }

        IsVisible.OnValueChanged += VisibleOnValueChanged;

        _adminPlace.AdminChange += OnAdminChange;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient)
        {
            return;
        }

        IsVisible.OnValueChanged -= VisibleOnValueChanged;

        _adminPlace.AdminChange -= OnAdminChange;

        base.OnNetworkDespawn();
    }

    private void SetPadVisibleState(bool isVisible)
    {
        SetPadVisibleServerRPC(isVisible);
    }

    private void VisibleOnValueChanged(bool oldv, bool newv)
    {
        VisibilityStateChanged?.Invoke();
    }

    private void OnAdminChange(ulong arg1, ulong arg2)
    {
        if (!IsOwner)
        {
            return;
        }
        if (IsVisible.Value)
        {
            if (!_adminPlace.IsUserAdmin(NetworkManager.LocalClientId))
            {
                SetPadVisibleState(false);
            }
        }
    }

    [ServerRpc]
    private void SetPadVisibleServerRPC(bool isVisible)
    {
        IsVisible.Value = isVisible;
    }
}
