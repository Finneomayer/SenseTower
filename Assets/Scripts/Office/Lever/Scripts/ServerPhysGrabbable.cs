using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SyncServerPhysGrabble))]
public class ServerPhysGrabbable : InterectObjectGrabbable
{
    private SyncServerPhysGrabble _syncGrabble;

    [SerializeField]
    private bool _isLimitation = false;
    [SerializeField]
    private Transform _limitedTarget;
    [SerializeField]
    private float _maxDistanceTarget = 2f;

    protected override void Start()
    {
        base.Start();
        _syncGrabble = GetComponent<SyncServerPhysGrabble>();
    }

    protected override void SetPositionAndRotation(Vector3 finalPosition, Quaternion finalRotation)
    {
        if (_isLimitation && _limitedTarget != null)
        {
            var distance = Vector3.Distance(finalPosition, _limitedTarget.position);
            if (distance > _maxDistanceTarget)
            {
                var magnitude = (finalPosition - _limitedTarget.position).normalized;
                finalPosition = _limitedTarget.position + _maxDistanceTarget * magnitude;
            }
        }

        base.SetPositionAndRotation(finalPosition, finalRotation);
        var id = NetworkManager.Singleton.LocalClientId;
        _syncGrabble.SetTransformServerRpc(finalPosition, finalRotation, id);
    }

    
}
