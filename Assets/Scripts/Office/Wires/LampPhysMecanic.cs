using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(LeverControllerInput))]
[RequireComponent(typeof(Rigidbody))]
public class LampPhysMecanic : Indicator
{
    private Rigidbody rb;   
    private SyncServerPhysGrabble _syncGrabble;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _syncGrabble = GetComponent<SyncServerPhysGrabble>();
    }

    void Update()
    {
        if (IsServer) return;

        if (IndicatorActive != 0 )
        {
            var id = NetworkManager.Singleton.LocalClientId;
            _syncGrabble.SetForceServerRpc(rb.velocity, rb.angularVelocity, id);
        }

    }
}
