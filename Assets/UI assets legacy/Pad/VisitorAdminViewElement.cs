using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Pad
{
    public class VisitorAdminViewElement : VisitorViewElement
    {
        public event Action<ulong> AdminButtonClicked;
        public event Action<ulong, bool> MuteButtonClicked;
        public event Action<ulong, Guid> KickButtonClicked;

        [SerializeField] private Button _adminButton;
        [SerializeField] private Button _muteButton;
        [SerializeField] private Button _kickButton;
        [SerializeField] private Image _image;
        
        private bool _isMuted = false;
        private void Start()
        {
            _image.enabled = _isMuted;
            
            _adminButton.onClick.AddListener(() => AdminButtonClicked?.Invoke(Id));
            _muteButton.onClick.AddListener(OnMuteButtonClick);
            _kickButton.onClick.AddListener(() => KickButtonClicked?.Invoke(Id, Guid));
        }

        public void ToogleMuteState(bool state)
        {
            _isMuted = state;
            _image.enabled = _isMuted;
        }

        public void SetActiveAdminButton(bool active)
        {
            _adminButton.gameObject.SetActive(active);
        }

        private void OnMuteButtonClick()
        {
            _isMuted = !_isMuted;

            ToogleMuteState(_isMuted);
            MuteButtonClicked?.Invoke(Id,_isMuted);
        }
    }
}
