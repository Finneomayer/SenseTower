using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BuyZoneTrigger : MonoBehaviour
{
    public event Action<ObjectToBuy> ObjectToSellUpdated;

    private ObjectToBuy _objectToBuy;
    private bool _isBusy;
    private ulong _occupiedId;

    private List<ObjectToBuy> _objectsInZone = new();

    private void OnTriggerEnter(Collider other)
    {
        if (_isBusy)
            return;

        if (other.TryGetComponent(out ObjectToBuyCollider objectToBuyCollider))
        {
            ObjectToBuy objectToBuy = objectToBuyCollider.Object;
            if (_objectsInZone.Contains(objectToBuy))
            {
                return;
            }
            _objectsInZone.Add(objectToBuy);

            if (!_isBusy)
            {
                objectToBuy.OnGrabExit += OnGrabExitedPut;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out ObjectToBuyCollider objectToBuyCollider))
        {
            return;
        }

        ObjectToBuy objectToBuy = objectToBuyCollider.Object;
        _objectsInZone.Remove(objectToBuy);
        objectToBuy.OnGrabExit -= OnGrabExitedPut;

        if (_objectToBuy == null || objectToBuy != _objectToBuy)
            return;

        _objectToBuy = null;

        if (!_isBusy)
        {
            return;
        }

        if (_occupiedId != NetworkManager.Singleton.LocalClientId)
            return;

        ObjectToSellUpdated?.Invoke(_objectToBuy);
    }

    private void OnGrabExitedPut(ObjectToBuy objectToBuy)
    {
        if (_objectToBuy != null) return;

        _objectToBuy = objectToBuy;
        ObjectToSellUpdated?.Invoke(_objectToBuy);
    }

    public void ToogleWork(bool isBusy, ulong occupiedId)
    {
        _occupiedId = occupiedId;
        _isBusy = isBusy;
    }
}