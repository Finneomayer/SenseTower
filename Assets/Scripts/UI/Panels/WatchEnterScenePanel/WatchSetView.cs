using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{
    public class WatchSetView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _watchIconContent;

        [SerializeField]
        private List< WatchIcon> _watchAssetIdIconMap;

        public event Action OnWatchSelectRequested;
        public event Action<bool> OnIgnoreAvatarWarnong;
        private IApiService _apiService;

        [Inject]
        public void Init(IApiService apiService)
        {
            _apiService = apiService;
        }

        private void Awake()
        {            
            for(int i = 0; i < _watchAssetIdIconMap.Count; i++) 
            {
                WatchIcon item = _watchAssetIdIconMap[i];
                item.Init(i);
                int number = i;
                item.Clicked += () => OnWatchIconClicked(number);
                _watchAssetIdIconMap[i] = item;
            }
        }



        public void SetInteractable(bool interactable)
        {
            _watchIconContent.alpha = interactable ? 1 : 0.05f;
            _watchIconContent.interactable = interactable;
        }

        public void UnSelectAll()
        {
            foreach (var item in _watchAssetIdIconMap)
            {
                item.SetSelected(false);
            }
        }

        public void UnselectItem()
        {
            _watchAssetIdIconMap.ForEach(x =>
            {
                x.SetSelected(false);
            });
            
        }

        public void Select(int watchId)
        {
            UnselectItem();
            _watchAssetIdIconMap[watchId].SetSelected(true);
        }

        public async void OnWatchIconClicked(int assetId)
        {
            //WatchSessionData.WatchPlayerId = assetId;

            if (AvatarSessionData.UserId != null && AvatarSessionData.UserId != 0)
            {
                bool watchSet = await _apiService.SetWatch(true, assetId);
                if (watchSet)
                {
                    Select(WatchSessionData.WatchPlayerIdOculus);
                    OnIgnoreAvatarWarnong?.Invoke(false);
                    OnWatchSelectRequested?.Invoke();
                }
            }
            else
            {
                bool watchSet = await _apiService.SetWatch(false, assetId);
                if (watchSet)
                {
                    Select(WatchSessionData.WatchPlayerId);
                    OnIgnoreAvatarWarnong?.Invoke(WatchSessionData.HasIgnoreAvatar);
                    OnWatchSelectRequested?.Invoke();
                }
            }
        }
    }
}