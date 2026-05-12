using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebugNetworkObjects : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var objects = GameObject.FindObjectsOfType<NetworkObject>();

            foreach (var obj in objects)
            {
                string parent = "root";
                if (obj.transform.parent != null) parent = obj.transform.parent.name;
                Debug.LogWarning($"{obj.GetHashCode()} -- {parent}/{obj.name}");
            }
        }
    }
}
