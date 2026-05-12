using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoubleXRGrabInteractable : XRGrabInteractable
{
    [SerializeField] private Transform _leftAttachTransform;
    [SerializeField] private Transform _rightAttachTransform;

    protected override void Awake()
    {
        base.Awake();
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<XRInteractionManager>();
        }
        selectMode = InteractableSelectMode.Multiple;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (interactorsSelecting.Count == 1)
        {
            base.ProcessInteractable(updatePhase);
            if (interactorsSelecting[0].transform.name == "LeftHand Grab")
            {
                attachTransform = _leftAttachTransform;
            }
            else if (interactorsSelecting[0].transform.name == "RightHand Grab")
            {                
                attachTransform = _rightAttachTransform;
            }
        }
        else if (interactorsSelecting.Count == 2 && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            ProcessDoubleGrip();
        }
    }

    private void ProcessDoubleGrip()
    {
        //Getting required Transforms
        Transform rightAttach = GetAttachTransform(null);
        Transform rightHand = interactorsSelecting[0].transform;

        Transform leftAttach = _leftAttachTransform;
        Transform leftHand = interactorsSelecting[1].transform;

        var distance = Vector3.Distance(rightHand.position, leftHand.position) * 3.6f;
        if (distance > 4f) distance = 4f;
        if (distance < 0.7f) distance = 0.7f;
        transform.localScale = new Vector3(distance, distance, distance);
    }
}
