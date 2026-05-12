using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KinematicSwitcher : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable _padXR;
    [SerializeField] private Rigidbody _rb;

    private void Update()
    {
        if (!_padXR.isHovered) _rb.isKinematic = true;
    }
}
