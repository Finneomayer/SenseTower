using Assets.Scripts.Player;
using Assets.Scripts.TowerObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

namespace UI
{
    public class InventoryPlaceView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private BoxCollider ViewCollider;
        [SerializeField]
        private GameObject TextContent;
        [SerializeField]
        private TMP_Text ItemNameText;
        [SerializeField] private GameObject _countContent; 
        [SerializeField] private TMP_Text _countText;

        private InventoryPlace _inventoryPlace;
        private IXRInteractor[] _interactors;
        private Coroutine _delayedDisabling = null;

        private void Awake()
        {
            SetActiveText(false);
        }

        private void FixedUpdate()
        {
            if (XRGeneralSettings.Instance != null &&
                !XRGeneralSettings.Instance.Manager.isInitializationComplete) return;

            if (_inventoryPlace == null)
            {
                return;
            }

            if (_inventoryPlace.UserItem == null)
            {
                return;
            }

            if (_inventoryPlace.PlaceGrabbable.AreHandsInPlace)
            {
                SetActiveText(true);
                return;
            }

            SetActiveText(XrColliderHovering.IsHovering(_interactors, ViewCollider));
        }

        public void Init(InventoryPlace inventoryPlace, IXRInteractor[] interactors)
        {
            _inventoryPlace = inventoryPlace;
            _interactors = interactors;
            SetCountText(true);
        }

        public void DeInit()
        {
            _inventoryPlace = null;
            _interactors = null;
            SetActiveText(false);
            SetCountText(false);
        }

        private void SetActiveText(bool active)
        {
            if (_inventoryPlace == null || _inventoryPlace.UserItem == null)
            {
                TextContent.SetActive(false);
                return;
            }

            if (!active)
            {
                if (!TextContent.activeInHierarchy) return;
                if (_delayedDisabling == null) 
                    _delayedDisabling = StartCoroutine(DelayedDisabling());

                return;
            }

            string itemCount = _inventoryPlace.ItemCount > 1 ? _inventoryPlace.ItemCount.ToString() + "x ": "";

            ItemNameText.text = itemCount + _inventoryPlace.UserItem.GetLocalizedName();
            TextContent.SetActive(true);
        }

        private void SetCountText(bool active)
        {
            _countContent.SetActive(active && _inventoryPlace != null && (_inventoryPlace.ItemCount > 1));

            if (_inventoryPlace != null) _countText.text = _inventoryPlace.ItemCount > 1 ? _inventoryPlace.ItemCount.ToString() + "x " : "";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetActiveText(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetActiveText(false);
        }

        private IEnumerator DelayedDisabling()
        {
            yield return new WaitForSeconds(0.5f);
            TextContent.SetActive(false);
            _delayedDisabling = null;
        }
    }
}
