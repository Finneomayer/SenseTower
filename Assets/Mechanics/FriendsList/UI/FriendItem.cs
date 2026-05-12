using Mechanics.FriendsList;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Mechanics.FriendsList.UI
{
    public class FriendItem : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private TMP_Text _userName;
        [SerializeField] private TMP_Text _spaceOrOnlineText;
        [SerializeField] private TMP_Text _confirmationSendedText;
        [SerializeField] private Image _avatarImage;
        [Space] [Header("Buttons Panel")] [SerializeField]
        private ViewPanel _firstStateButtonPanel;

        [SerializeField] private ViewPanel _requestAddFriendsButtonPanel;
        [SerializeField] private ViewPanel _confirmDeleteFriendsButtonPanel;
        [SerializeField] private Button _confirmRequestButton;

        [SerializeField] private Button[] _confirmButtons;
        [SerializeField] private Button[] _cancelButtons;
        [SerializeField] private Button[] _deleteButtons;

        #endregion

        public string FriendUserId
        {
            private set => _friendUserId = value;
            get => _friendUserId;
        }

        public bool IsRequest => _isRequest;
        private string _friendUserId;
        private NetworkFriendListService _networkFriendListService;
        private bool _isRequest = false;
        private bool _confirmed = false;

        public Action<string> FriendRequestConfirmationSended;

        [Inject]
        private void Construct(NetworkFriendListService networkFriendListService)
        {
            _networkFriendListService = networkFriendListService;
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnSubscribeToEvents();
        }

        public void MakeInstanceAsRequest(bool confirmed)
        {
            _isRequest = true;
            _confirmed = confirmed;
        }

        public void SetAvatarImage(Sprite sprite)
        {
            _avatarImage.sprite = sprite;
        }

        public void InitData(string userName, bool status, string spaceName, string userId)
        {
            _userName.text = userName;
            if (!_isRequest)
            {
                _spaceOrOnlineText.text = status ? spaceName : "Offline";
                _spaceOrOnlineText.color = !status ? Color.red : Color.white;
            }

            _friendUserId = userId;
            InitState();
        }

        private void InitState()
        {
            if (_isRequest)
            {                
                _requestAddFriendsButtonPanel.ShowPanel();
                _firstStateButtonPanel.HidePanel();
                _confirmDeleteFriendsButtonPanel.HidePanel();

                _confirmRequestButton.gameObject.SetActive(!_confirmed);
                _confirmationSendedText.gameObject.SetActive(_confirmed);
            }
            else
            {
                _firstStateButtonPanel.ShowPanel();
                _requestAddFriendsButtonPanel.HidePanel();
                _confirmDeleteFriendsButtonPanel.HidePanel();

            }
        }

        private void ConfirmRequestButtonClick()
        {
            if (_isRequest)
            {
                _confirmed = true;
                InitState();
                _networkFriendListService.AddRequestMakeFriendToServer(_friendUserId);
                FriendRequestConfirmationSended?.Invoke(_friendUserId);
            }
            else
                _networkFriendListService.RemoveFriendFromFriendList(_friendUserId);
        }

        private void OnDeleteButtonClick()
        {
            if (_isRequest)
                _networkFriendListService.RemoveFriendRequest(_friendUserId);
            else
            {
                _confirmDeleteFriendsButtonPanel.ShowPanel();
                _firstStateButtonPanel.HidePanel();
                _requestAddFriendsButtonPanel.HidePanel();
            }
        }

        private void SubscribeToEvents()
        {
            foreach (Button button in _confirmButtons)
            {
                button.onClick.AddListener(ConfirmRequestButtonClick);
            }

            foreach (Button button in _cancelButtons)
            {
                button.onClick.AddListener(InitState);
            }

            foreach (Button button in _deleteButtons)
            {
                button.onClick.AddListener(OnDeleteButtonClick);
            }
        }

        private void UnSubscribeToEvents()
        {
            foreach (Button button in _confirmButtons)
            {
                button.onClick.RemoveListener(ConfirmRequestButtonClick);
            }

            foreach (Button button in _cancelButtons)
            {
                button.onClick.RemoveListener(InitState);
            }

            foreach (Button button in _deleteButtons)
            {
                button.onClick.RemoveListener(OnDeleteButtonClick);
            }
        }
    }
}