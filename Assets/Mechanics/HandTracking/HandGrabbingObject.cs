using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandGrabbingObject : MonoBehaviour
{
    public XRGrabInteractable XrGrab;

    private void OnEnable()
    {
        XrGrab = GetComponentInParent<XRGrabInteractable>();
    }
}
