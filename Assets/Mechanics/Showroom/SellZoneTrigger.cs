using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SellZoneTrigger : MonoBehaviour
{
    public event Action<ObjectToSell> ObjectToSellUpdated;
    private const float CheckDelay = 0.5f; //second
    
    private ObjectToSell _objectToSell;
    private float _objectSetTime;
    private bool _isBusy;
    private ulong _occupiedId;

    private List<GrabbingHand> _grabbingHands = new();

    private void Start()
    {
        StartCoroutine(CheckingHandsObjectsRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        GrabbingHand grabbingHand;
        if ((grabbingHand = other.GetComponent<GrabbingHand>()) != null)
        {
            if (_grabbingHands.Contains(grabbingHand))
            {
                return;
            }
            _grabbingHands.Add(grabbingHand);
            grabbingHand.GrabbingStopped += GrabbingHand_GrabbingStopped;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GrabbingHand grabbingHand;
        if ((grabbingHand = other.GetComponent<GrabbingHand>()) != null)
        {
            _grabbingHands.Remove(grabbingHand);
            grabbingHand.GrabbingStopped -= GrabbingHand_GrabbingStopped;
        }

        //Workaroud. Because Unity invokes OnTriggerExit on object, which is non kinematic when I PUT it into this trigger zone.
        //Now it's ignored, and invokes only when i take object from this trigger zone.
        if (Time.time - _objectSetTime < CheckDelay) return;
        if (_isBusy && _occupiedId != NetworkManager.Singleton.LocalClientId)
            return;

        if (other.TryGetComponent(out ObjectToSellCollider objCollider))
        {
            ObjectToSell objectToSell = objCollider.Object;
            if (_objectToSell == objectToSell)
            {
                _objectToSell = null;
                ObjectToSellUpdated?.Invoke(null);
            }
            _objectSetTime = Time.time;
        }
    }

    private void GrabbingHand_GrabbingStopped(GrabbingHand grabbingHand)
    {
        if (_isBusy)
            return;

        if (_objectToSell != null)
        {
            return;
        }

        foreach (var item in grabbingHand.InventoryObjects)
        {
            if (item.gameObject.TryGetComponent(out ObjectToSell objectToSell))
            {
                _objectToSell = objectToSell;
                ObjectToSellUpdated?.Invoke(_objectToSell);
                _objectSetTime = Time.time;
                return;
            }
        }
    }

    public void ToogleWork(bool isBusy, ulong occupiedId)
    {
        _occupiedId = occupiedId;
        _isBusy = isBusy;
    }

    private IEnumerator CheckingHandsObjectsRoutine()
    {
        var delay = new WaitForSeconds(0.1f);
        while (true)
        {
            if (_objectToSell == null)
            {
                yield return delay;
                continue;
            }

            foreach (var hand in _grabbingHands)
            {
                if (hand.IsGrabbingInProgress)
                {
                    foreach (var item in hand.InventoryObjects)
                    {
                        if (item.gameObject == _objectToSell.gameObject)
                        {
                            _objectToSell = null;
                            ObjectToSellUpdated?.Invoke(null);
                            break;
                        }
                    }
                    if (_objectToSell == null)
                    {
                        break;
                    }
                }
            }

            yield return delay;
        }
    }
}
