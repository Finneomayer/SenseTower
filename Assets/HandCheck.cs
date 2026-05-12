using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandCheck : MonoBehaviour
{
    private OVRHand _hand;

    // Start is called before the first frame update
    void Start()
    {
        _hand = GetComponent<OVRHand>();

        var direct = new XRDirectInteractor();
        //direct.OnSelectEntered();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.LogWarning($"{_hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring)} --" +
                         $"{_hand.GetFingerIsPinching(OVRHand.HandFinger.Index)}");
    }
}
