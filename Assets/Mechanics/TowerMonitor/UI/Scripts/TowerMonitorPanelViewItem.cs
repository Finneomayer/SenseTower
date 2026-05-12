using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WebUtils;
using System;
using Assets.Localization;

namespace UI
{
    public abstract class TowerMonitorPanelViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        protected GameObject HoveredBackground;
        [SerializeField]
        protected Image Image;

        [SerializeField]
        protected LocalizationVariant TodayLocalizationVariant;

        protected WebDataDownloader _webDataDownloader = new();

        public event Action<bool> ImageSetFinished;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (HoveredBackground != null)
            {
                HoveredBackground.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (HoveredBackground != null)
            {
                HoveredBackground.SetActive(false);
            }
        }

        protected void InitImage(string imageUrl)
        {
            Image.enabled = false;

            _webDataDownloader.DownloadCompleted -= OnImageDownloadCompleted;
            _webDataDownloader.DownloadCompleted += OnImageDownloadCompleted;

            _webDataDownloader.DownloadFailed -= OnImageDownloadFailed;
            _webDataDownloader.DownloadFailed += OnImageDownloadFailed;

            _webDataDownloader.DownloadData(imageUrl);
        }

        protected string GetEventDateTimeString(DateTimeOffset dateTimeOffset)
        {
            DateTimeOffset dateTime = dateTimeOffset.ToLocalTime();

            string dateString;
            DateTime now = DateTime.Now;
            if (dateTimeOffset.Year == now.Year
                && dateTimeOffset.Month == now.Month
                && dateTimeOffset.Day == now.Day)
            {
                dateString = TodayLocalizationVariant.Localize();
            }
            else
            {
                dateString = dateTime.ToString("dd.MM.yyyy");
            }

            string timeString = dateTime.ToString("HH:mm");

            return $"{dateString}, {timeString}";
        }

        protected void OnImageDownloadCompleted(byte[] rawData)
        {
            _webDataDownloader.DownloadCompleted -= OnImageDownloadCompleted;

            Texture2D texture2D = new Texture2D(1, 1);

            if (texture2D.LoadImage(rawData))
            {
                Image.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one / 2);
                ImageSetFinished?.Invoke(true);
            }
            Image.enabled = true;
        }

        protected void OnImageDownloadFailed()
        {
            _webDataDownloader.DownloadFailed -= OnImageDownloadFailed;
            ImageSetFinished?.Invoke(false);
        }
    }
}
