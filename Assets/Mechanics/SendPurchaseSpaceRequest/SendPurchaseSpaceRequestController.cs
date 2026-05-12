using Assets.Scripts.Space;
using Unity.Netcode;
using UnityEngine;

namespace Mechanics.SendPurchaseSpaceRequest
{
    public class SendPurchaseSpaceRequestController : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private SpacePurchaseRequestViewPanel _spacePurchaseRequestViewPanel;
        [SerializeField] private DoorPlayerSensor _doorPlayerSensor;

        #endregion

        private LocalSpace _localSpace;

        private void OnEnable()
        {
            _spacePurchaseRequestViewPanel.gameObject.SetActive(false);
            _doorPlayerSensor.OnDoorNearEnter += OnDoorPlayerEnter;
            _doorPlayerSensor.OnDoorNearExit += OnDoorPlayerExit;
        }

        private void OnDisable()
        {
            _doorPlayerSensor.OnDoorNearEnter -= OnDoorPlayerEnter;
            _doorPlayerSensor.OnDoorNearExit -= OnDoorPlayerExit;
        }

        public void SetLocalSpace(LocalSpace localSpace)
        {
            _localSpace = localSpace;
        }

        private void OnDoorPlayerEnter(Collider collider)
        {
            if (collider.transform.root.TryGetComponent(out NetworkObject networkObject))
            {
                if (networkObject != null && networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                    return;

                if (_localSpace == null
                    || _localSpace.IsForSale == null)
                    return;

                if (_localSpace.IsForSale.Value)
                {
                    _spacePurchaseRequestViewPanel.gameObject.SetActive(true);
                    _spacePurchaseRequestViewPanel.SetLocalSpace(_localSpace);
                    _spacePurchaseRequestViewPanel.ShowPanel();
                }
            }
        }

        private void OnDoorPlayerExit(Collider collider)
        {
            if (_localSpace == null)
                return;
            if (collider.transform.root.TryGetComponent(out NetworkObject networkObject))
            {
                if (networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                    return;
                _spacePurchaseRequestViewPanel.HidePanel();
                _spacePurchaseRequestViewPanel.gameObject.SetActive(false);
            }
        }
    }
}