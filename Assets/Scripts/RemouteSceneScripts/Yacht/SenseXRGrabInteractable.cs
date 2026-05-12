using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SenseXRGrabInteractable : XRGrabInteractable
{
    protected override void Grab()
    {
        var parentTransform = transform.parent;
        var pos = transform.position;
        base.Grab();
        transform.parent = parentTransform;
        transform.position = pos;
    }
}
