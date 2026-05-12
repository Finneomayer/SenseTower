using System;
using System.Collections.Generic;
using System.IO;
using Assets.Mechanics.MetaAvatars.Scripts;
using UnityEngine;

namespace UI
{
    public class AvatarSetView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup AvatarIconContent;
        [SerializeField]
        private AvatarIcon AvatarIconPrefab;

        private Dictionary<int, AvatarIcon> _avatarAssetIdIconMap;

        public event Action<int> AvatarSelectRequested;

        private void Awake()
        {
            _avatarAssetIdIconMap = new();

            Sprite[] loadedIcons = Resources.LoadAll<Sprite>("PresetAvatarsIcons");

           foreach (var item in loadedIcons)
            {
                if (!int.TryParse(Path.GetFileNameWithoutExtension(item.name), out int assetId))
                {
                    continue;
                }

                //if (assetId == AvatarSessionData.DefaultAvatarId)
                //{
                //    continue;
                //}
                AvatarIcon newItem = Instantiate(AvatarIconPrefab, AvatarIconContent.transform);
                newItem.Init(assetId, item);
                newItem.Clicked += () => OnAvatarIconClicked(assetId);
                _avatarAssetIdIconMap[assetId] = newItem;
            }
        }

        public void SetInteractable(bool interactable)
        {
            AvatarIconContent.alpha = interactable ? 1 : 0.05f;
            AvatarIconContent.interactable = interactable;
        }

        public void SelectNone()
        {
            foreach (var item in _avatarAssetIdIconMap)
            {
                item.Value.SetSelected(false);
            }
        }

        public void Select(int assetId)
        {
            SelectNone();

            if (_avatarAssetIdIconMap.TryGetValue(assetId, out AvatarIcon avatarIcon))
            {
                avatarIcon.SetSelected(true);
            }
            else
            {               
                Debug.LogWarning($"<color=red>{typeof(AvatarSetView).Name}. Select. Icon for asset id does not exists</color>");
            }
        }

        private void OnAvatarIconClicked(int assetId)
        {
            if (AvatarIconContent.interactable)
            {
                AvatarSelectRequested?.Invoke(assetId);
            }           
        }
    }
}
