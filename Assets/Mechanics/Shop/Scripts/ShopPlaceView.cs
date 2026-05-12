using Assets.Scripts.TowerObjects;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

namespace Assets.Mechanics.Shop.Scripts
{
    public class ShopPlaceView : MonoBehaviour
    {
        [SerializeField]
        private GameObject TextContent;
        [SerializeField]
        private TMP_Text ItemNameText;
        [SerializeField]
        private TMP_Text ItemBuyPriceText;

        private ShopItemPlace _inventoryPlace;
        private IXRInteractor[] _interactors;
        private BoxCollider _viewCollider;

        private void Awake()
        {
            DeInit();
        }

        private void FixedUpdate()
        {
            if (XRGeneralSettings.Instance != null &&
                !XRGeneralSettings.Instance.Manager.isInitializationComplete) return;

            if (_inventoryPlace == null)
            {
                return;
            }

            if (_inventoryPlace.ShopItemDto == null)
            {
                return;
            }

            if (_inventoryPlace.PlaceGrabbable.AreHandsInPlace)
            {
                SetActiveText(true);
                return;
            }

            SetActiveText(XrColliderHovering.IsHovering(_interactors, _viewCollider));
        }

        public void Init(ShopItemPlace inventoryPlace, BoxCollider collider, IXRInteractor[] interactors)
        {
            _inventoryPlace = inventoryPlace;
            _interactors = interactors;
            _viewCollider = collider;
            enabled = true;
        }

        public void DeInit()
        {
            _inventoryPlace = null;
            _interactors = null;
            _viewCollider = null;
            enabled = false;

            SetActiveText(false);
        }

        public void SetActiveText(bool active)
        {
            if (!active)
            {
                TextContent.SetActive(false);
                return;
            }

            if (_inventoryPlace == null || _inventoryPlace.ShopItemDto == null 
                || _inventoryPlace.ShopItemDto.Item == null || !_inventoryPlace.ShopItemDto.Price.HasValue)
            {
                TextContent.SetActive(false);
                return;
            }
            
            TextContent.SetActive(true);
            ItemNameText.text = _inventoryPlace.ShopItemDto.Item.GetLocalizedName();
            ItemBuyPriceText.text = $"{decimal.ToInt32(_inventoryPlace.ShopItemDto.Price.Value)} TWR";
        }
    }
}