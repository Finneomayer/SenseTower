using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class AvatarIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image Image;
        [SerializeField]
        private Image SelectedBackground;

        public int AvatarAssetId { get; private set; }

        public event Action Clicked;

        public void Init(int assetId, Sprite sprite)
        {
            AvatarAssetId = assetId;
            Image.sprite = sprite;
        }

        public void SetSelected(bool selected)
        {
            SelectedBackground.gameObject.SetActive(selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }
    }
}
