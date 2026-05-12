using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public class YoutubeActionsHandler : WebPageActionsHandler
    {
        private const string UrlStartSubstring = "https://www.youtube.com";
        private const string UrlMinimizedSubstring = "/watch";
        private const string UrlMaximizedSubstring = "/embed";

        public override WebPageAction GetAvailableActions(string url)
        {
            string videoId = YoutubeLinkFunctions.GetVideoId(_canvasWebViewPrefab.WebView.Url);


            if (url.Contains(UrlMaximizedSubstring))
            {
                return WebPageAction.Minimize;
            }
            if (url.Contains(UrlMinimizedSubstring))
            {
                return WebPageAction.Maximize;
            }
            return WebPageAction.None;
        }

        public override void ProcessAction(WebPageAction webPageAction)
        {
            string videoId = YoutubeLinkFunctions.GetVideoId(_canvasWebViewPrefab.WebView.Url);
            if (string.IsNullOrEmpty(videoId))
            {
                return;
            }

            switch (webPageAction)
            {
                case WebPageAction.Maximize:
                    Maximize(videoId);
                    break;
                case WebPageAction.Minimize:
                    Minimize(videoId);
                    break;
                default:
                    break;
            }
        }

        public override void ProcessPageLoadedActions(string previousUrl, string newUrl)
        {
            if (newUrl.Contains(UrlMaximizedSubstring)
                || previousUrl.Contains(UrlMaximizedSubstring))
            {
                _canvasWebViewPrefab.WebView.Click(new Vector2(0.5f, 0.5f));
            }
        }

        private void Minimize(string videoId)
        {
            string newUrl = $"{UrlStartSubstring}{UrlMinimizedSubstring}?v={videoId}";
            _canvasWebViewPrefab.WebView.LoadUrl(newUrl);
        }

        private void Maximize(string videoId)
        {
            string newUrl = $"{UrlStartSubstring}{UrlMaximizedSubstring}/{videoId}?start=0&autoplay=1";
            _canvasWebViewPrefab.WebView.LoadUrl(newUrl);
        }
    }
}