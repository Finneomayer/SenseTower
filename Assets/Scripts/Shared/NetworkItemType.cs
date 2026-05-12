using Assets.Scripts.Shared;
using Infrastructure.Factory;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkItemType : NetworkBehaviour
{
    #region Inspector
    [SerializeField] private Transform _aimObject;
    
    private string _towerObjectId;
    private string _remoteObjectKey;
    private string _remoteObjectRepositoryUrl;
    private string _userNickName;
    private string _ownerId;

    #endregion

    public Transform AimObject => _aimObject;
    public string TowerObjectId => _towerObjectId;
    public string OwnerId => _ownerId;
    public string RemoteObjectKey => _remoteObjectKey;
    public string RemoteObjectRepositoryUrl => _remoteObjectRepositoryUrl;

    public string UserNickName => _userNickName;

    private void FixedUpdate()
    {
        if (_aimObject == null) return;
        if (IsOwner)
        {
            transform.position = _aimObject.position;
            transform.rotation = _aimObject.rotation;
        }
        else 
        {
            _aimObject.position = transform.position;
            _aimObject.rotation = transform.rotation;
        }
    }

    public void Init(string ownerId,string towerObjectId, string remoteObjectKey, string remoteObjectRepositoryUrl, string userNickName)
    {
        _ownerId = ownerId;
        _towerObjectId = towerObjectId;
        _remoteObjectKey = remoteObjectKey;
        _userNickName = userNickName;
        _remoteObjectRepositoryUrl = remoteObjectRepositoryUrl;
    }

    public void SetAimObject(Transform transform) 
    {
        _aimObject = transform;
    }
    
    public override void OnDestroy()
    {
        if(_aimObject!=null)
            Destroy(_aimObject.gameObject);
    }

    [ContextMenu("deinit")]
    [ServerRpc(RequireOwnership = false)]
    public void DespawnCurrentObjectServerRpc()
    {
        NetworkObject.Despawn();
        Destroy(NetworkObject);
    }

    public void StopFollow()
    {
        if (gameObject.TryGetComponent( out NetworkTransformTransmitter networkTransformTransmitter))
        {
            networkTransformTransmitter.StopMoving();
        }
    }

    public void StartFollow()
    {
        if (gameObject.TryGetComponent( out NetworkTransformTransmitter networkTransformTransmitter))
        {
            networkTransformTransmitter.StartMoving();
        }
    }
}
