using System;
using UnityEngine;

namespace Assets.Mechanics.Mirror_selfie
{
    public class MirrorControlPanel : MonoBehaviour
    {
        [SerializeField] private ButtonUI _mirrorVisibleButton;
        [SerializeField] private ButtonUI _closeButton;
        private bool _isVisibleToOthers;
        public event Action<bool> MirrorClickVisible;
        public event Action MirrorCloseClick;

        private void OnEnable()
        {
            SubscribeEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        private void SubscribeEvents()
        {
            if (_mirrorVisibleButton != null)
                _mirrorVisibleButton.InteractElement.onClick.AddListener(OnSetVisibleToOthersButtonClick);
            if (_closeButton != null)
                _closeButton.InteractElement.onClick.AddListener(OnCloseButtonClick);
        }

        private void UnsubscribeEvents()
        {
            if (_mirrorVisibleButton != null)
                _mirrorVisibleButton.InteractElement.onClick.RemoveAllListeners();
            if (_closeButton != null)
                _closeButton.InteractElement.onClick.RemoveAllListeners();
        }

        private void OnCloseButtonClick()
        {
            MirrorCloseClick?.Invoke();
        }

        private void OnSetVisibleToOthersButtonClick()
        {
            _isVisibleToOthers = !_isVisibleToOthers;
            SetVisibleButtonActive(_isVisibleToOthers);
            MirrorClickVisible?.Invoke(_isVisibleToOthers);
        }

        public void SetVisibleButtonActive(bool active)
        {
            if (_mirrorVisibleButton != null)
            {
                _mirrorVisibleButton.SetButtonActive(active);
            }
        }
    }
}
