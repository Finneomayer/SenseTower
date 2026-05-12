using System;
using System.Collections.Generic;
using Assets.Scripts.API;
using UI;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Advertisement
{
    [Serializable]
    public class AdvertisementView
    {
        public ViewPanel Panel;
        public CanvasWebViewPrefab Canvas;
    }

    public class AdvertisementBrowserPageService : MonoBehaviour, IAdvertisementService
    {
        #region Inspector

        [SerializeField] private List<AdvertisementView> _advertisements; //1 - Left, 2 - Right
        #endregion

        private void Start()
        {
            foreach (var adv in _advertisements)
            {
                adv.Panel.HidePanel();
            }

            Show();
        }
        
        public void Show()
        {
            string leftSideAddress = APIService.LeftSideHallAdvertisementBilboardEndPoint;
            string rightSideAddress = APIService.RightSideHallAdvertisementBilboardEndPoint;

            if (string.IsNullOrEmpty(leftSideAddress) && string.IsNullOrEmpty(rightSideAddress)) return;

           
            for (int i = 0; i < _advertisements.Count; i++)
            {
                string adress = !string.IsNullOrEmpty(leftSideAddress) ? leftSideAddress : rightSideAddress;

                if (i == 1) adress = rightSideAddress;

                ShowAdvertisement(adress, _advertisements[i]);
            }
        }

        private async void ShowAdvertisement(string address, AdvertisementView view) 
        {
            await view.Canvas.WaitUntilInitialized();
            view.Panel.ShowPanel();
            view.Canvas.InitialUrl = address;
        }
    }
}