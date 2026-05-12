using Assets.Mechanics.MetaAvatars.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class WatchViewPanel : ViewPanel
    {
        [SerializeField]
        private WatchSetView _watchSetView;
        [SerializeField]
        private AvatarSetView _avatarSetView;
        [SerializeField]
        private GameObject _ignoreText;

        private void Start()
        {
            _watchSetView.SetInteractable(true);
            _watchSetView.OnWatchSelectRequested += OnWatchSelectRequested;
            _watchSetView.OnIgnoreAvatarWarnong += ShowIgnoreText;
            _avatarSetView.AvatarSelectRequested += (x) => _watchSetView.OnWatchIconClicked(-1);
        }


        private void OnDestroy()
        {
            _watchSetView.OnWatchSelectRequested -= OnWatchSelectRequested;
            _watchSetView.OnIgnoreAvatarWarnong -= ShowIgnoreText;
            _avatarSetView.AvatarSelectRequested -= (x) => _watchSetView.OnWatchIconClicked(-1);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            if (AvatarSessionData.UserId != null && AvatarSessionData.UserId != 0)
            {
                _watchSetView.Select(WatchSessionData.WatchPlayerIdOculus);
                _watchSetView.OnWatchIconClicked(WatchSessionData.WatchPlayerIdOculus);
            }
            else
            {
                _watchSetView.Select(WatchSessionData.WatchPlayerId);
                _watchSetView.OnWatchIconClicked(WatchSessionData.WatchPlayerId);
            }
        }

        private void ShowIgnoreText(bool canShow)
        {
            _ignoreText.SetActive(canShow);
        }

        private void OnWatchSelectRequested()
        {         
            _watchSetView.SetInteractable(false);
            if (AvatarSessionData.UserId != null && AvatarSessionData.UserId != 0)
            {
                _watchSetView.Select(WatchSessionData.WatchPlayerIdOculus);
            }
            else
            {
                _watchSetView.Select(WatchSessionData.WatchPlayerId);
            }
            _watchSetView.SetInteractable(true);
        }
    }
}