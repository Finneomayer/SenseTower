using Stubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadCollider : MonoBehaviour
{
    public event Action<Transform> PadTableZoneEnter;
    public event Action PadTableZoneExit;
    private Transform _anchorYcoordinateForPad;

    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.tag == "KeyboardZoneTable")
        {
            if (_anchorYcoordinateForPad == null) _anchorYcoordinateForPad = other.GetComponent<TableHeightForPad>().Height;
            PadTableZoneEnter?.Invoke(_anchorYcoordinateForPad);
        }        
        
        if (other.gameObject.tag == "PlayerFingers")
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {        
        if (other.gameObject.tag == "KeyboardZoneTable")
        {
            PadTableZoneExit?.Invoke();
        }            
    }
}
