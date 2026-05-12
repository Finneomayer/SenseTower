using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class WatchIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image SelectedBackground;

        public int WatchAssetId { get; private set; }

        public event Action Clicked;

        public void Init(int watchId)
        {
            WatchAssetId = watchId;
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
