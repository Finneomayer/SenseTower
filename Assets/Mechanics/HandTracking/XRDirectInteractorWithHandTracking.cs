using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDirectInteractorWithHandTracking : XRDirectInteractor, IXRSelectInteractor
{
    public event Action StartGrabingHand;
    public event Action StopGrabingHand;

    [SerializeField] private OVRHand _hand;
    [SerializeField] private TriggerListenerForHandTracking _handTrigger;
    private bool _previousFrameActivate = false; //0


    private void Update()
    {
        if (!_previousFrameActivate && _hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            StartGrabingHand?.Invoke();

            if (_handTrigger.HoveringObject != null)
            {
                StartManualInteraction((IXRSelectInteractable)_handTrigger.HoveringObject);
            }
        }
        if (_previousFrameActivate && !_hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (isPerformingManualInteraction) EndManualInteraction();
            StopGrabingHand?.Invoke();
        }

        _previousFrameActivate = _hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
    }

    [ContextMenu("StartGrabbingHand")]
    public void StartGrabbingHandMenu()
    {
        StartGrabingHand?.Invoke();
    }

    [ContextMenu("StopGrabbingHand")]
    public void StopGrabbingHandMenu()
    {
        if (isPerformingManualInteraction) EndManualInteraction();
        StopGrabingHand?.Invoke();
    }
}
