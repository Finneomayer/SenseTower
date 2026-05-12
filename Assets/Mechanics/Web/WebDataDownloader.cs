using UnityEngine;
using System.Net;
using System;

namespace WebUtils
{
    public class WebDataDownloader
    {
        public event Action<byte[]> DownloadCompleted;
        public event Action DownloadFailed;

        private WebClient _webClient;

        public WebDataDownloader()
        {
            _webClient = new WebClient();
        }

        public void DownloadData(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri path))
            {
                _webClient.DownloadDataCompleted += OnDownloadDataCompleted;
                _webClient.DownloadDataAsync(path);
            }
            else
            {
                Debug.LogWarning($"{typeof(WebDataDownloader).Name}. Wrong url: {url}");
                DownloadFailed?.Invoke();
            }
        }

        private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            _webClient.DownloadDataCompleted -= OnDownloadDataCompleted;
            if (e.Error == null)
            {
                DownloadCompleted?.Invoke(e.Result);
            }
            else
            {
                Debug.LogWarning($"{typeof(WebDataDownloader).Name}. {e.Error.Message}");
            }
        }
    }
}
