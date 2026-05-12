using Cysharp.Threading.Tasks;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.VectorGraphics;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Browser.Scripts.NetworkActions
{
    public class Mp4StateTransmitter : NetworkBehaviour
    {
        [SerializeField]
        private NetworkWebBrowser _networkWebBrowser;

        [SerializeField] private YoutubeStateTransmitter _youtubeStateTransmitter;
        [SerializeField] private string[] _videoLinks;
        [SerializeField] private CinemaEnviromentControl _cinemaEnviroment;

        private string _javaScriptResponse;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (NetworkManager.IsClient)
            {
                _networkWebBrowser.AdminChanged += OnBrowserAdminChanged;
                _networkWebBrowser.AdminUrlChanged += OnBrowserAdminUrlChanged;
                _networkWebBrowser.NetworkModeChanged += OnBrowserNetworkModeChanged;
                
                WaitBrowserInitialized().Forget();
            }
        }

        private async UniTask WaitBrowserInitialized()
        {
            await _networkWebBrowser.WebViewPrefab.WaitUntilInitialized();

            _networkWebBrowser.WebViewPrefab.WebView.MessageEmitted += WebViewMessageEmitted;
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.IsClient)
            {
                _networkWebBrowser.AdminChanged -= OnBrowserAdminChanged;
                _networkWebBrowser.AdminUrlChanged -= OnBrowserAdminUrlChanged;
                _networkWebBrowser.NetworkModeChanged -= OnBrowserNetworkModeChanged;
                if (_networkWebBrowser.WebViewPrefab.WebView.IsDisposed) return;
                _networkWebBrowser.WebViewPrefab.WebView.MessageEmitted -= WebViewMessageEmitted;
            }

            base.OnNetworkDespawn();
        }

        private void OnBrowserAdminChanged(ulong adminClientId)
        {
            StopTransmission();
        }

        private void OnBrowserAdminUrlChanged(string url)
        {
            StopTransmission();

            if (_networkWebBrowser.NetworkMode != BrowserNetworkMode.Network)
            {
                return;
            }

            //string pattern = $@"\s*\w*{Format}\b";
            //if (Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase)) StartTransmission();

            //if (url.Contains(Format)) 

            if (_videoLinks != null && _videoLinks.Length > 0)
            {
                if (_videoLinks.Any(link => url.Contains(link)))
                {
                    if (_youtubeStateTransmitter != null) _youtubeStateTransmitter.StopDoubleTransmission();
                    StartTransmission();
                }
            }            

            if (_cinemaEnviroment != null) _cinemaEnviroment.SetSliderDefaultValue();
        }

        private void OnBrowserNetworkModeChanged()
        {
            StopTransmission();
        }

        private void StartTransmission()
        {
            StartCoroutine(YoutubeTransmittingRoutine());
        }

        private void WebViewMessageEmitted(object sender, EventArgs<string> e)
        {
            _javaScriptResponse = e.Value;
        }

        private void StopTransmission()
        {
            StopAllCoroutines();
        }

        private IEnumerator YoutubeTransmittingRoutine()
        {
            Task<SerializableYoutubeStateData> getStateTask;
            while (true)
            {
                if (_networkWebBrowser.WebViewPrefab.WebView == null
                    || _networkWebBrowser.NetworkMode != BrowserNetworkMode.Network)
                {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(1);

                getStateTask = GetCurrentStateDataAsync().AsTask();
                yield return new WaitUntil(() => getStateTask.IsCompleted);

                TransferStateDataServerRpc(getStateTask.Result, NetworkManager.LocalClientId);
            }
        }

        private async UniTask<SerializableYoutubeStateData> GetCurrentStateDataAsync() //For browser Admin getting data
        {
            await _networkWebBrowser.WebViewPrefab.WaitUntilInitialized();

            var utcs = new UniTaskCompletionSource<SerializableYoutubeStateData>();

            SerializableYoutubeStateData stateData = new();

            await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("vuplex.postMessage(document.querySelector('video').currentTime)");
            await Task.Delay(50);
            stateData.CurrentTime = StringResponseToFloat(_javaScriptResponse);

            await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("vuplex.postMessage(document.querySelector('video').volume)");
            await Task.Delay(50);
            stateData.CurrentVolume = StringResponseToFloat(_javaScriptResponse);

            await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"vuplex.postMessage(document.querySelector('video').paused)");
            await Task.Delay(50);
            bool.TryParse(_javaScriptResponse, out stateData.IsPaused);

            await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"vuplex.postMessage(document.querySelector('video').muted)");
            await Task.Delay(50);
            bool.TryParse(_javaScriptResponse, out stateData.IsMuted);

            utcs.TrySetResult(stateData);

            return await utcs.Task;
        }
        
        private float StringResponseToFloat(string response)
        {
            float.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float result);
            return result;
        }

        private string FloatToStringRequest(float request)
        {
            return request.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        private async UniTask SetStateDataAsync(SerializableYoutubeStateData newStateData)
        {
            await _networkWebBrowser.WebViewPrefab.WaitUntilInitialized();

            SerializableYoutubeStateData currentStateData = await GetCurrentStateDataAsync();
            
            if (newStateData.IsPaused != currentStateData.IsPaused)
            {
                if (newStateData.IsPaused)
                {
                    await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("document.querySelector('video').pause()");
                }
                else
                {
                    //_networkWebBrowser.WebViewPrefab.WebView.SetFocused(true);
                    _networkWebBrowser.WebViewPrefab.WebView.Click(new Vector2(0.5f, 0.5f));
                    
                    await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("document.querySelector('video').play()");
                }
            }

            if (newStateData.IsPaused || Mathf.Abs(newStateData.CurrentTime - currentStateData.CurrentTime) > 5)
            {
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelector('video').currentTime = {FloatToStringRequest(newStateData.CurrentTime)}");
            }

            if (Mathf.Abs(newStateData.CurrentVolume - currentStateData.CurrentVolume) > 0.05f)
            {
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelector('video').volume = {FloatToStringRequest(newStateData.CurrentVolume)}");
            }

            if (newStateData.IsMuted != currentStateData.IsMuted)
            {
                await _networkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelector('video').muted = {newStateData.IsMuted.ToString().ToLower()}");
            }
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
        private void TransferStateDataServerRpc(SerializableYoutubeStateData stateData, ulong requestingClientId)
        {
            if (!_networkWebBrowser.IsUserAdmin(requestingClientId))
            {
                return;
            }

            TransferStateDataClientRpc(stateData, _networkWebBrowser.ServerClientRpcParams);
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void TransferStateDataClientRpc(SerializableYoutubeStateData stateData, ClientRpcParams clientRpcParams = default)
        {
            SetStateDataAsync(stateData).Forget();
        }
    }
}
