using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkStartInvoker : MonoBehaviour
{
    public event Action OnStartNetwork; //NetworkManager is subscriber if necessary delayed start

    public bool NetworkStartRequested { get; private set; }

    public void StartNetwork()
    {
        OnStartNetwork?.Invoke();
        NetworkStartRequested = true;
    }
}
