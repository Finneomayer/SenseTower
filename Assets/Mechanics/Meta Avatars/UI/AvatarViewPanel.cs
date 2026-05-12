using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace UI
{
    public class AvatarViewPanel : ViewPanel
    {
        [SerializeField]
        private AvatarSetView AvatarSetView;

        private int? _currentAvatarAssetId;
        private IApiService _apiService;

        [Inject]
        public void Init(IApiService apiService)
        {
            _apiService = apiService;

            AvatarSetView.SetInteractable(true);
            AvatarSetView.AvatarSelectRequested += OnAvatarSelectRequested;
        }

        public override void ShowPanel()
        {
            base.ShowPanel();

            _currentAvatarAssetId = AvatarSessionData.AvatarAssetId;
            if (_currentAvatarAssetId.HasValue)
            {
                AvatarSetView.Select(_currentAvatarAssetId.Value);
            }
            else
            {
                AvatarSetView.SelectNone();
            }
        }

        private async void OnAvatarSelectRequested(int avatarAssetId)
        {
            if (_currentAvatarAssetId.HasValue && _currentAvatarAssetId.Value == avatarAssetId)
            {
                return;
            }
            _currentAvatarAssetId = avatarAssetId;

            ClientData clientData = new();
            if (!clientData.UserId.HasValue)
            {
                Debug.LogWarning($"<color=red>{typeof(AvatarViewPanel).Name}.</color> clientData.UserId == null");
                return;
            }

            AvatarSetView.SetInteractable(false);
            bool avatarSetted = await _apiService.SetAvatar(clientData.UserId.Value, avatarAssetId);

            if (avatarSetted)
            {
                AvatarSetView.Select(avatarAssetId);
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }

            AvatarSetView.SetInteractable(true);
        }
    }
}
