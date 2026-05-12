using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : Slider
{
    public event Action<float> PointerExit; 
    public event Action<float> PointerDrag;

    public override void OnPointerExit(PointerEventData eventData)
    {
        PointerExit?.Invoke(value);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        PointerDrag?.Invoke(value);
    }

    public float GetValue() //this is a workaround because the standard value works every other time
    {
        return handleRect.anchorMin.x; 
    }
}
