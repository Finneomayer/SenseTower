using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSelectInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private XRBaseInteractable Interactable;

        public event Action HoverEntered;
        public event Action HoverExited;
        public event Action Selected;

        private Coroutine _selection;
        private bool _selectionBlocked = false;

        private void OnEnable()
        {
            Interactable.firstHoverEntered.AddListener(OnHoverEnter);
            Interactable.lastHoverExited.AddListener(OnHoverExit);
            Interactable.selectExited.AddListener(OnSelect);
        }

        private void OnDisable()
        {
            Interactable.firstHoverEntered.RemoveListener(OnHoverEnter);
            Interactable.lastHoverExited.RemoveListener(OnHoverExit);
            Interactable.selectExited.RemoveListener(OnSelect);
        }

        public void SetActive(bool active)
        {
            Interactable.gameObject.SetActive(active);
        }

        private void OnHoverEnter(HoverEnterEventArgs hoverEnterEventArgs)
        {
            HoverEntered?.Invoke();
        }

        private void OnHoverExit(HoverExitEventArgs hoverExitEventArgs)
        {
            HoverExited?.Invoke();
        }

        private void OnSelect(SelectExitEventArgs selectExitEventArgs)
        {
            if (_selectionBlocked) return;
            if (_selection != null)
            {
                StopCoroutine(_selection);
            }
            _selection = StartCoroutine(SelectCoroutine());
            Selected?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsXrInitialized())
            {
                return;
            }
            HoverEntered?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsXrInitialized())
            {
                return;
            }
            HoverExited?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsXrInitialized())
            {
                return;
            }
            Selected?.Invoke();
        }

        private bool IsXrInitialized()
        {
            return XRGeneralSettings.Instance != null
                && XRGeneralSettings.Instance.Manager != null
                && XRGeneralSettings.Instance.Manager.isInitializationComplete;
        }

        private IEnumerator SelectCoroutine()
        {
            _selectionBlocked = true;
            yield return new WaitForSeconds(2f);
            _selectionBlocked = false;
            _selection = null;
        }
    }
}
