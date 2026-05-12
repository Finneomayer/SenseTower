using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;

public class SetFoveationLevel : MonoBehaviour
{
    [SerializeField] private int _foveationLevel;

    void Start()
    {
#if !UNITY_SERVER
        if (Utils.GetSystemHeadsetType() != SystemHeadset.None)
        {
            Utils.SetFoveationLevel(_foveationLevel);
        }
#endif
    }
}
