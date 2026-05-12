using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Mechanics.PadKeyboard
{
    public class RaycastButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _buttonVisual;

        public event Action OnClick;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHoverEnter?.Invoke();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            OnHoverExit?.Invoke();
        }

        public void Show()
        {
            _button.enabled = true;
            if (_buttonVisual != null)
            {
                _buttonVisual.enabled = true;
            }
        }

        public void Hide()
        {
            _button.enabled = false;
            if (_buttonVisual != null)
            {
                _buttonVisual.enabled = false;
            }
        }

        private void OnButtonClick()
        {
            OnClick?.Invoke();
        }

        
    }
}
