using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GalleryLeavesCollider : NetworkBehaviour
{
    public event Action<bool> OnTriggerStayAction;

    private bool _isStayInTrigger;
    private bool _isStayInTriggerPrevious;

    public NetworkVariable<bool> IsStayInTriggerNetwork;

    private void Awake() //Unity order 1
    {
        IsStayInTriggerNetwork = new NetworkVariable<bool>();
    }

    private void Start() //Unity order 2
    {
        IsStayInTriggerNetwork.OnValueChanged += OnValueChanged;
    }

    private void FixedUpdate() //Unity order 3
    {
        _isStayInTrigger = false;
    }

    private void OnTriggerStay(Collider other) //Unity order 4
    {
        if (other.GetComponent<XRDirectInteractor>() != null)
        {
            _isStayInTrigger = true;
        }
    }

    private void Update() //Unity order 5
    {
        if (!_isStayInTriggerPrevious && _isStayInTrigger)
        {
            OnTriggerStayAction?.Invoke(true);
            SetNetworkVariableServerRpc(true);
        }

        if (_isStayInTriggerPrevious && !_isStayInTrigger)
        {
            OnTriggerStayAction?.Invoke(false);
            SetNetworkVariableServerRpc(false);
        }
        _isStayInTriggerPrevious = _isStayInTrigger;
    }

    private void OnValueChanged(bool previousvalue, bool newvalue)
    {
        if (!previousvalue && newvalue) OnTriggerStayAction?.Invoke(true);
        if (previousvalue && !newvalue) OnTriggerStayAction?.Invoke(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNetworkVariableServerRpc(bool isStayInTrigger)
    {
        IsStayInTriggerNetwork.Value = isStayInTrigger;
    }
}
