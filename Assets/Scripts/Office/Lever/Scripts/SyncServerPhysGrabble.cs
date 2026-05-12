using Unity.Netcode;
using UnityEngine;

public class SyncServerPhysGrabble : NetworkBehaviour
{
    //ToDo  не ведаю по какой причине, но нетворк бех САМ меняет скейл. 
    // на сервере он остается нормальным. на клиенте меняется. замечено на кальяне и рычаге.
    // дурная щтука. если все норм, скейл на 1 поставиьт
    [SerializeField]
    private float _scale = 1;

    [SerializeField]
    private bool IsKinematic = true;
    private Rigidbody rb;

    private Vector3 _currentVelocity;
    private Vector3 _currentAngularVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
#if UNITY_SERVER
        if (rb != null)
        {
            rb.isKinematic = true;
        }
#endif
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            transform.localScale *= _scale;
            var id = NetworkManager.Singleton.LocalClientId;

            if (IsKinematic)
            {
                SetStartClinetTransformServerRpc(id);
            }
            else
            {
                SetStartClinetForceServerRpc(id);
            }
        }
    }

    #region update
    [ServerRpc(RequireOwnership = false)]
    public void SetTransformServerRpc(Vector3 position, Quaternion quaternion, ulong id)
    {
        SetTransform(position, quaternion);
        SetTransformClientRpc(position, quaternion, id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetForceServerRpc(Vector3 velocity, Vector3 angularVelocity, ulong id)
    {
        _currentVelocity = velocity;
        _currentAngularVelocity = angularVelocity;
        //SetForce(velocity, angularVelocity);
        SetForceClientRpc(velocity, angularVelocity, id);
    }

    [ClientRpc]
    private void SetTransformClientRpc(Vector3 position, Quaternion quaternion, ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId != id)
        {
            SetTransform(position, quaternion);
        }
    }

    [ClientRpc]
    private void SetForceClientRpc(Vector3 velocity, Vector3 angularVelocity, ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId != id)
        {
            SetForce(velocity, angularVelocity);
        }
    }
    #endregion

    #region start
    [ServerRpc(RequireOwnership = false)]
    private void SetStartClinetTransformServerRpc(ulong id)
    {
        SetStartTransformClientRpc(transform.position, transform.rotation, id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStartClinetForceServerRpc(ulong id)
    {
        if (rb != null)
        {
            SetStartForceClientRpc(_currentVelocity, _currentAngularVelocity, id);
        }
    }

    [ClientRpc]
    private void SetStartTransformClientRpc(Vector3 position, Quaternion quaternion, ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            SetTransform(position, quaternion);
        }
    }

    [ClientRpc]
    private void SetStartForceClientRpc(Vector3 velocity, Vector3 angularVelocity, ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            SetForce(velocity, angularVelocity);
        }
    }
    #endregion

    private void SetTransform(Vector3 position, Quaternion quaternion)
    {
        transform.position = position;
        transform.rotation = quaternion;
    }

    private void SetForce(Vector3 velocity, Vector3 angularVelocity)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}