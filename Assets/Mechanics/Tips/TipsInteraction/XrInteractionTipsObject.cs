using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.Tips.TipsInteraction
{
    public class XrInteractionTipsObject : TipsObject, IPointerEnterHandler, IPointerExitHandler
    {
        #region Inspector
        [SerializeField] private XRBaseInteractable _interactable;
        
        #endregion

        private void Start()
        {
            if (_interactable == null)
            {
                _interactable = GetComponent<XRBaseInteractable>();
            }

            if (_interactable == null)
                return;

            _interactable.hoverEntered.AddListener(OnHoverEnter);
            _interactable.hoverExited.AddListener(OnHoverExit);
        }

        private void OnDestroy()
        {
            if (_interactable == null)
                return;
            
            _interactable.hoverEntered.RemoveListener(OnHoverEnter);
            _interactable.hoverExited.RemoveListener(OnHoverExit);
        }

        private void OnHoverEnter(HoverEnterEventArgs hoverEnterEventArgs)
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                ShowTips();
        }

        private void OnHoverExit(HoverExitEventArgs hoverExitEventArgs)
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                HideTips();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                ShowTips();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                HideTips();
        }
    }
}