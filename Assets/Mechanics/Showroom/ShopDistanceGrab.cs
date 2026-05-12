using Assets.Scripts.Player;
using UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.Showroom
{
    public class ShopDistanceGrab : MonoBehaviour
    {
        [SerializeField] private XRSimpleInteractable _xrInteractable;
        [SerializeField] private InventoryPlaceGrabbable _inventory;
        [SerializeField] private BoxCollider _boxCollider;

        private void OnEnable()
        {
            if (_xrInteractable != null)
            {
                _xrInteractable.selectEntered.AddListener(PlaceDistanceSelected);
            }
            _inventory.HandInPlaceChanged += OnInventoryPlaceHandInPlaceChanged;
        }

        private void OnDisable()
        {
            if (_xrInteractable != null)
            {
                _xrInteractable.selectEntered.RemoveListener(PlaceDistanceSelected);
            }
            _inventory.HandInPlaceChanged -= OnInventoryPlaceHandInPlaceChanged;
        }

        private void PlaceDistanceSelected(SelectEnterEventArgs arg0)
        {
            PlayerController handController = arg0.interactorObject.transform.GetComponentInParent<PlayerController>();
            if (handController == null)
            {
                return;
            }

            _inventory.RaiseGrab(handController.GrabbingHand);
        }

        private void OnInventoryPlaceHandInPlaceChanged()
        {
            _xrInteractable.enabled = !_inventory.AreHandsInPlace;
        }
    }
}
