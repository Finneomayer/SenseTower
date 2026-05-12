using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Browser
{
    public class PresentationControlPanel : ViewPanel
    {
        private List<WebPageActionsSlot> WebPageActionsSlots = new List<WebPageActionsSlot>();

        private CanvasWebViewPrefab _canvasWebViewPrefab;
        private List<WebPageActionsHandler> _pageActionHandlers = new();
        private WebPageActionsHandler _currentPageActionHandler;

        private string _lastLoadedUrl = string.Empty;
        private Coroutine _processActionsAfterPageLoadedRoutine;

        private void Awake()
        {
            WebPageStateChangingReactionRoutine().Forget();

            if (WebPageActionsSlots.Count > 0)
                foreach (var item in WebPageActionsSlots)
                {
                    item.SetActiveActions(WebPageAction.None);
                }
        }

        private void OnEnable()
        {
            if (WebPageActionsSlots.Count > 0) 
            {
                foreach (var item in WebPageActionsSlots)
                {
                    item.ActionRequested += OnWebPageActionRequested;
                }
            }

        }

        private void OnDisable()
        {
            if (WebPageActionsSlots.Count > 0)
            {
                foreach (var item in WebPageActionsSlots)
                {
                    item.ActionRequested -= OnWebPageActionRequested;
                }
            }
        }

        public void RaiseWebPageAction(WebPageAction webPageAction)
        {
            OnWebPageActionRequested(webPageAction);
        }

        public void AddWebPageActions(WebPageActionsSlot slot)
        {
            if (slot == null)
            {
                return;
            }
            WebPageActionsSlots.Add(slot);
            slot.SetActiveActions(WebPageAction.None);
            slot.ActionRequested += OnWebPageActionRequested;
        }

        public void Init(CanvasWebViewPrefab canvasWebViewPrefab)
        {
            _canvasWebViewPrefab = canvasWebViewPrefab;

            _pageActionHandlers.Add(new GooglePresentationActionsHandler());
            _pageActionHandlers.Add(new YoutubeActionsHandler());

            foreach (var item in _pageActionHandlers)
            {
                item.Init(_canvasWebViewPrefab);
            }

            SubscribeWebViewEvents().Forget();
        }

        public void DeInit()
        {
            UnsubscribeWebViewEvents();
        }

        private async UniTask SubscribeWebViewEvents()
        {
            await _canvasWebViewPrefab.WaitUntilInitialized();
            _canvasWebViewPrefab.WebView.UrlChanged += OnWebViewUrlChanged;
            _canvasWebViewPrefab.WebView.MessageEmitted += OnWebViewMessageEmitted;
        }

        private void UnsubscribeWebViewEvents()
        {
            if (_canvasWebViewPrefab.WebView != null)
            {
                _canvasWebViewPrefab.WebView.UrlChanged -= OnWebViewUrlChanged;
                _canvasWebViewPrefab.WebView.MessageEmitted -= OnWebViewMessageEmitted;
            }              
        }

        private void OnWebViewUrlChanged(object sender, UrlChangedEventArgs e)
        {
            if (_processActionsAfterPageLoadedRoutine != null)
            {
                StopCoroutine(_processActionsAfterPageLoadedRoutine);
                _processActionsAfterPageLoadedRoutine = null;
            }
            if (WebPageActionsSlots.Count > 0)
                foreach (var item in WebPageActionsSlots)
                {
                    item.SetActiveActions(WebPageAction.None);
                }

            _currentPageActionHandler = null;
            foreach (var handler in _pageActionHandlers)
            {
                WebPageAction handlerActions = handler.GetAvailableActions(e.Url);
                if (handlerActions != WebPageAction.None)
                {
                    _currentPageActionHandler = handler;
                    foreach (var actionsSlot in WebPageActionsSlots)
                    {
                        actionsSlot.SetActiveActions(handlerActions);
                    }

                    break;
                }
            }
            _lastLoadedUrl = _canvasWebViewPrefab.WebView.Url;
        }

        private void OnWebPageActionRequested(WebPageAction webPageAction)
        {
            if (_currentPageActionHandler == null)
            {
                return;
            }
            _currentPageActionHandler.ProcessAction(webPageAction);
        }

        private void OnWebViewMessageEmitted(object sender, EventArgs<string> eventArgs)
        {
            if (string.IsNullOrEmpty(eventArgs.Value))
            {
                return;
            }

            WebViewResponse response;
            try
            {
                response = JsonConvert.DeserializeObject<WebViewResponse>(eventArgs.Value);
            }
            catch (Exception)
            {
                return;
            }

            _currentPageActionHandler?.HandleWebViewJavascriptResponse(response);
            //if (response == null || response.MessageType == null || response.Message == null)
            //{
            //    return;
            //}

            //if (response.MessageType == "fullscreen_check")
            //{
            //    if (bool.TryParse(response.Message, out bool isFullscreen) && isFullscreen)
            //    {
            //        if (_currentPageActionHandler == null 
            //            || !_currentPageActionHandler.GetAvailableActions(_canvasWebViewPrefab.WebView.Url).HasFlag(WebPageAction.Maximize))
            //        {
            //            return;
            //        }

            //        _currentPageActionHandler.ProcessAction(WebPageAction.Maximize);
            //    }
            //}
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

        private async UniTask WebPageStateChangingReactionRoutine()
        {
            while (true)
            {
                await UniTask.Delay(1000);

                if (_canvasWebViewPrefab == null || _canvasWebViewPrefab.WebView == null)
                {
                    continue;
                }

                _currentPageActionHandler?.ExecutePeriodicJavascripts();
                
                //if (_currentPageActionHandler is not GooglePresentationActionsHandler)
                //{
                //    continue;
                //}

                //await _canvasWebViewPrefab.WebView.ExecuteJavaScript("vuplex.postMessage({messageType: 'fullscreen_check', message: (document.fullScreenElement && document.fullScreenElement !== null) || (document.mozFullScreen || document.webkitIsFullScreen) || document.body.classList.contains('punch-viewer-body')})");
                //await _canvasWebViewPrefab.WebView.ExecuteJavaScript("vuplex.postMessage({messageType: 'html_check', message: document.documentElement.outerHTML})");
            }
        }

        private IEnumerator ProcessActionsAfterPageLoadedRoutine(string previousUrl, string newUrl)
        {
            yield return new WaitForSeconds(0.5f);

            _currentPageActionHandler.ProcessPageLoadedActions(previousUrl, newUrl);
            _processActionsAfterPageLoadedRoutine = null;
        }
    }
}