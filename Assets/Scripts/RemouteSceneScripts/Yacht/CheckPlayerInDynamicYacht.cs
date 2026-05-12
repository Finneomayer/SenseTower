using Assets.Scripts.Shared;
using Unity.Netcode;
using UnityEngine;

namespace Sense.RemouteScene
{
    public class CheckPlayerInDynamicYacht : NetworkBehaviour
    {
        private GameObject _UIBinder;
        private CharacterController _player;
        private float _lastSetParentTime;
        private Vector3 _targetPosition;

        private void Start()
        {
            var component = FindObjectOfType<UIBinder>();
            if (component != null)
                _UIBinder = component.gameObject;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (_player != null)
            {
                return;
            }

            if (IsServer)
            {
                return;
            }

            var network = other.GetComponent<NetworkObject>();
            if (network == null || !network.IsOwner)
            {
                return;
            }

            var player = other.GetComponent<CharacterController>();
            if (player != null)
            {
                _player = player;
            }
        }

        private void FixedUpdate()
        {
            if (_player == null)
            {
                return;
            }

            if (Time.time - _lastSetParentTime < 0.5f)
            {
                _player.enabled = false;
                _player.transform.position = _targetPosition;
                _player.enabled = true;
                return;
            }

            Collider[] colliders = Physics.OverlapBox(_player.transform.position + _player.center,
                _player.bounds.extents, Quaternion.identity);

            bool isPlayerInside = false;
            foreach (var item in colliders)
            {
                if (item.gameObject == gameObject)
                {
                    isPlayerInside = true;
                    break;
                }
            }

            if (isPlayerInside)
            {
                if (_player.transform.parent == null)
                {
                    Debug.LogWarning("Setting player inside yacht");
                    _targetPosition = _player.transform.position;
                    SetParent(_player.gameObject, true);
                    _lastSetParentTime = Time.time;
                }
            }
            else
            {
                if (_player.transform.parent == transform)
                {
                    Debug.LogWarning("Setting player outside yacht");
                    _targetPosition = _player.transform.position;
                    SetParent(_player.gameObject, false);
                    _lastSetParentTime = Time.time;
                }
            }
        }

        private void SetParent(GameObject other, bool isEnter)
        {
            var network = other.GetComponent<NetworkObject>();
            if (network == null || !network.IsOwner)
            {
                return;
            }

            var player = other.GetComponent<CharacterController>();

            if (player != null && network != null && network.IsOwner  && !IsServer)
            {
                SetPlayerParentServerRpc(NetworkManager.LocalClientId, isEnter);
            }

            if (player != null && _UIBinder != null)
            {
                _UIBinder.transform.parent = isEnter ? transform : null;
            }
        }


        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerParentServerRpc(ulong localClientId, bool isEnter)
        {

            if (NetworkManager.ConnectedClients.ContainsKey(localClientId))
            {

                if (isEnter)
                    NetworkManager.ConnectedClients[localClientId].PlayerObject.TrySetParent(transform);
                else
                    NetworkManager.ConnectedClients[localClientId].PlayerObject.TryRemoveParent();
            }
        }
    }
}