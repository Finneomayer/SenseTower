using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TriggerListenerForHandTracking : MonoBehaviour
{
    public XRGrabInteractable HoveringObject;

    private new void OnTriggerEnter(Collider other)
    {
        HandGrabbingObject interactable;
        if (other.TryGetComponent<HandGrabbingObject>(out interactable))
        {
            HoveringObject = interactable.XrGrab;
        }
    }

    private new void OnTriggerExit(Collider other)
    {
        HandGrabbingObject interactable;
        if (other.TryGetComponent<HandGrabbingObject>(out interactable))
        {
            if (HoveringObject.name == interactable.XrGrab.name) HoveringObject = null;
        }
    }
}
