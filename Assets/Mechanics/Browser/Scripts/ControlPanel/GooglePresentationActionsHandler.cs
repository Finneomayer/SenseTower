using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public class GooglePresentationActionsHandler : WebPageActionsHandler
    {
        private const string UrlStartSubstring = "docs.google.com/presentation";
        private const string UrlMinimizedSubstring = "/edit";
        private const string UrlMaximizedSubstring = "/present";
        private const string UrlSlideShowSubstring = "/localpresent";

        public override WebPageAction GetAvailableActions(string url)
        {
            if (!IsGooglePresentationUrl(url))
            {
                return WebPageAction.None;
            }

            if (IsUrlHasMinMaxPageState(url, UrlMaximizedSubstring))
            {
                return WebPageAction.Minimize;
            }
            if (IsUrlHasMinMaxPageState(url, UrlMinimizedSubstring)
                || IsUrlHasMinMaxPageState(url, UrlSlideShowSubstring))
            {
                return WebPageAction.Maximize;
            }
            return WebPageAction.None;
        }

        public override void ProcessAction(WebPageAction webPageAction)
        {
            switch (webPageAction)
            {
                case WebPageAction.Maximize:
                    if (!ReloadPageWithUrlChange(UrlMinimizedSubstring, UrlMaximizedSubstring))
                    {
                        ReloadPageWithUrlChange(UrlSlideShowSubstring, UrlMaximizedSubstring);
                    }
                    break;
                case WebPageAction.Minimize:
                    ReloadPageWithUrlChange(UrlMaximizedSubstring, UrlMinimizedSubstring);
                    break;
                default:
                    break;
            }
        }

        public override async void ExecutePeriodicJavascripts()
        {
            await _canvasWebViewPrefab.WebView.ExecuteJavaScript("vuplex.postMessage({messageType: 'fullscreen_check', message: (document.fullScreenElement && document.fullScreenElement !== null) || (document.mozFullScreen || document.webkitIsFullScreen) || document.body.classList.contains('punch-viewer-body')})");
        }

        public override void HandleWebViewJavascriptResponse(WebViewResponse response)
        {
            if (response == null || response.MessageType == null || response.Message == null)
            {
                return;
            }

            if (response.MessageType == "fullscreen_check")
            {
                if (bool.TryParse(response.Message, out bool isFullscreen) && isFullscreen)
                {
                    if (GetAvailableActions(_canvasWebViewPrefab.WebView.Url).HasFlag(WebPageAction.Maximize))
                    {
                        ProcessAction(WebPageAction.Maximize);
                    }                    
                }
            }
            // For debugging
            //else if (response.MessageType == "html_check")
            //{
            //    Debug.LogWarning($"Html length = {response.Message.Length}");

            //    int lengthForOneLog = 900;
            //    int interations = 1 + (response.Message.Length / lengthForOneLog);
            //    for (int i = 0; i < interations; i++)
            //    {
            //        int startIndex = i * lengthForOneLog;
            //        if (startIndex + lengthForOneLog >= response.Message.Length)
            //        {
            //            Debug.Log(response.Message.Substring(startIndex));
            //            break;
            //        }
            //        Debug.Log(response.Message.Substring(i * lengthForOneLog, lengthForOneLog));
            //    }
            //}
        }

        protected override bool ReloadPageWithUrlChange(string substringToChange, string newSubstring)
        {
            string url = _canvasWebViewPrefab.WebView.Url;
            int lastSlashIndex = url.LastIndexOf('/');
            if (lastSlashIndex == -1)
            {
                return false;
            }

            string keyWordSubString = url.Substring(lastSlashIndex);
            if (keyWordSubString.Contains(substringToChange, System.StringComparison.OrdinalIgnoreCase))
            {
                string newUrl = url.Substring(0, lastSlashIndex) + keyWordSubString.Replace(substringToChange, newSubstring, 
                    System.StringComparison.OrdinalIgnoreCase);
                _canvasWebViewPrefab.WebView.LoadUrl(newUrl);
                return true;
            }

            return false;
        }

        private static bool IsGooglePresentationUrl(string input)
        {
            string cleanInput = GetCleanGooglePresentationUrl(input);

            return cleanInput.StartsWith(UrlStartSubstring, System.StringComparison.OrdinalIgnoreCase);
        }

        private static string GetCleanGooglePresentationUrl(string input)
        {
            string cleanInput = input;

            if (cleanInput.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanInput = input.Replace("http://", "", System.StringComparison.OrdinalIgnoreCase);
            }
            else if (cleanInput.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanInput = input.Replace("https://", "", System.StringComparison.OrdinalIgnoreCase);
            }

            if (cleanInput.StartsWith("www.", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanInput = cleanInput.Replace("www.", "", System.StringComparison.OrdinalIgnoreCase);
            }
            return cleanInput;
        }

        private bool IsUrlHasMinMaxPageState(string url, string minMaxPageStateKeyWord)
        {
            string cleanInput = GetCleanGooglePresentationUrl(url);
            int lastSlashIndex = cleanInput.LastIndexOf('/');
            if (lastSlashIndex == -1)
            {
                return false;
            }

            string keyWordSubString = cleanInput.Substring(lastSlashIndex);

            return keyWordSubString.StartsWith(minMaxPageStateKeyWord, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}