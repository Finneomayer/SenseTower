using Newtonsoft.Json;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Browser
{
    [System.Serializable]
    public class ScrollTransmitter : NetworkBrowserTransmitter
    {
        public override async void CheckSyncData()
        {
            await _networkWebBrowser.WebViewPrefab.WaitUntilInitialized();
            
            if (_networkWebBrowser.WebViewPrefab.WebView.IsDisposed)
                return;
            
            ScrollData scrollData = new ScrollData();
            if (_networkWebBrowser.WebViewPrefab.WebView.IsDisposed)
                return;
            string scrollY =
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    "PDFViewerApplication.pdfViewer.container.scrollTop;");
            string scrollX =
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    "PDFViewerApplication.pdfViewer.container.scrollLeft;");

            bool isPDF = true;

            if (scrollY.Contains("null") || scrollY.Contains("ReferenceError: PDFViewerApplication is not defined"))
            {
                isPDF = false;
                scrollY = await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    "document.querySelector('div[role=document]').parentElement.scrollTop;");

               //if (scrollY.Contains("TypeError: Cannot read properties of null (reading 'parentElement')"))
               //{
               //    scrollY = await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("window.scrollY;");
               //}
            }

            if (scrollX.Contains("null") || scrollX.Contains("ReferenceError: PDFViewerApplication is not defined"))
            {
                scrollX = await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    "document.querySelector('div[role=document]').parentElement.scrollLeft;");

                //if (scrollX.Contains("TypeError: Cannot read properties of null (reading 'parentElement')"))
                //{
                //    scrollX = await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("window.scrollX;");
                //}
            }

            scrollData.isPDF = isPDF;
            scrollData.XScroll = scrollX;
            scrollData.YScroll = scrollY;

            SyncData = JsonConvert.SerializeObject(scrollData);
        }

        public override async void ApplyChanges()
        {
            if (string.IsNullOrEmpty(SyncData))
                return;
            if (!_networkWebBrowser.WebViewPrefab.WebView.IsInitialized)
                return;
            if (_networkWebBrowser.WebViewPrefab.WebView.IsDisposed)
                return;
            
            ScrollData scrollData = JsonConvert.DeserializeObject<ScrollData>(SyncData);
            
            if (scrollData.isPDF)
            {
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"PDFViewerApplication.pdfViewer.ScrollHorizontal({scrollData.XScroll});");
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"PDFViewerApplication.pdfViewer.ScrollVertical({scrollData.YScroll});");
            }
            else
            {
                string result = await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelector('div[role=document]').parentElement.scrollTop={scrollData.YScroll};");

                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelector('div[role=document]').parentElement.scrollLeft={scrollData.XScroll};");

                //if (result.Contains("TypeError: Cannot read properties of null (reading 'parentElement')"))
                //{
                //    await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                //        $"window.scrollTo({scrollData.XScroll},{scrollData.YScroll});");
                //}
            }
        }

        #region InnerClass

        public class ScrollData
        {
            public string YScroll;
            public string XScroll;
            public bool isPDF;
        }

        #endregion
    }
}