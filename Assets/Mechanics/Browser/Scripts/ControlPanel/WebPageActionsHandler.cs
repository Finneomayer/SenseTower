using Vuplex.WebView;

namespace Assets.Mechanics.Browser
{
    public class WebViewResponse
    {
        public string MessageType;
        public string Message;
    }

    public abstract class WebPageActionsHandler : IWebPageActionsHandler
    {
        protected CanvasWebViewPrefab _canvasWebViewPrefab;

        public void Init(CanvasWebViewPrefab canvasWebViewPrefab)
        {
            _canvasWebViewPrefab = canvasWebViewPrefab;
        }

        public abstract void ProcessAction(WebPageAction webPageAction);

        public virtual WebPageAction GetAvailableActions(string url)
        {
            return WebPageAction.None;
        }

        public virtual void ProcessPageLoadedActions(string previousUrl, string newUrl)
        {
            return;
        }

        public virtual async void ExecutePeriodicJavascripts()
        {
        }

        public virtual void HandleWebViewJavascriptResponse(WebViewResponse webViewResponse)
        {
        }

        protected virtual bool ReloadPageWithUrlChange(string substringToChange, string newSubstring)
        {
            string url = _canvasWebViewPrefab.WebView.Url;

            if (url.Contains(substringToChange))
            {
                string newUrl = url.Replace(substringToChange, newSubstring);
                _canvasWebViewPrefab.WebView.LoadUrl(newUrl);
                return true;
            }

            return false;
        }
    }
}