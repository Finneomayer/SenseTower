using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class FingerInteractor : MonoBehaviour
{
    private Collider _collider;

    private readonly List<XRGrabInteractable> _grabbables = new();

    private GrabbingHand _grabbingHand;
    private Coroutine _setTemporaryDisabledRoutine;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out XRGrabInteractable xrGrab))
        {
            if (_grabbables.Contains(xrGrab))
            {
                return;
            }
            _grabbables.Add(xrGrab);
            RefreshTriggerState();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out XRGrabInteractable xrGrab))
        {
            _grabbables.Remove(xrGrab);
            //RefreshTriggerState();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRGrabInteractable xrGrab))
        {
            if (_grabbables.Contains(xrGrab))
            {
                return;
            }
            _grabbables.Add(xrGrab);
            RefreshTriggerState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRGrabInteractable xrGrab))
        {
            _grabbables.Remove(xrGrab);
            //RefreshTriggerState();
        }
    }

    private void OnEnable()
    {
        UnregisterListeners();
        RegisterListeners();
    }

    private void OnDisable()
    {
        UnregisterListeners();
    }

    public void Init(GrabbingHand grabbingHand)
    {
        UnregisterListeners();
        _grabbingHand = grabbingHand;
        RegisterListeners();
    }

    private void RefreshTriggerState()
    {
        if (_collider == null)
        {
            return;
        }

        _collider.isTrigger = false;

        if (_grabbingHand == null)
        {
            return;
        }
        if (!_grabbingHand.IsGrabbingInProgress)
        {
            return;
        }

        if (_grabbables.Count > 0)
        {
            _collider.isTrigger = true;
        }
    }

    private void RegisterListeners()
    {
        if (_grabbingHand == null)
        {
            return;
        }
        _grabbingHand.GrabbingStarted += OnGrabbingHandGrabbingStarted;
        _grabbingHand.GrabbingStopped += OnGrabbingHandGrabbingStopped;
    }

    private void UnregisterListeners()
    {
        if (_grabbingHand == null)
        {
            return;
        }
        _grabbingHand.GrabbingStarted -= OnGrabbingHandGrabbingStarted;
        _grabbingHand.GrabbingStopped -= OnGrabbingHandGrabbingStopped;
    }

    private void OnGrabbingHandGrabbingStarted(GrabbingHand grabbingHand)
    {
        if (_setTemporaryDisabledRoutine != null)
        {
            StopCoroutine(_setTemporaryDisabledRoutine);
            _setTemporaryDisabledRoutine = null;
        }
        RefreshTriggerState();
    }

    private void OnGrabbingHandGrabbingStopped(GrabbingHand grabbingHand)
    {
        if (_setTemporaryDisabledRoutine != null)
        {
            StopCoroutine(_setTemporaryDisabledRoutine);
        }
        _setTemporaryDisabledRoutine = StartCoroutine(SetTemporaryDisabled(0.5f));
    }

    private IEnumerator SetTemporaryDisabled(float delayInSec)
    {
        _collider.isTrigger = true;
        yield return new WaitForSeconds(delayInSec);

        RefreshTriggerState();

        _setTemporaryDisabledRoutine = null;
    }
}