using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPlayerSensor : MonoBehaviour
{
    public event Action<Collider> OnDoorNearEnter;
    public event Action<Collider> OnDoorNearExit;

    private void OnTriggerEnter(Collider other)
    {
        OnDoorNearEnter?.Invoke(other);
            
    }

    private void OnTriggerExit(Collider other)
    {
        OnDoorNearExit?.Invoke(other);
    }
}
