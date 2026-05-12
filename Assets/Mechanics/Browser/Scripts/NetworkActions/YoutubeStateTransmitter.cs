using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Globalization;

namespace Assets.Mechanics.Browser
{
    [Serializable]
    public class SerializableYoutubeStateData : INetworkSerializable
    {
        public float CurrentTime;
        public float CurrentVolume;
        public bool IsPaused;
        public bool IsMuted;
        public int ActualVideoNumber;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CurrentTime);
            serializer.SerializeValue(ref CurrentVolume);
            serializer.SerializeValue(ref IsPaused);
            serializer.SerializeValue(ref IsMuted);
            serializer.SerializeValue(ref ActualVideoNumber);
        }
    }

    public class YoutubeStateTransmitter : NetworkBehaviour
    {
        [SerializeField] private NetworkWebBrowser NetworkWebBrowser;
        [SerializeField] private CinemaEnviromentControl _cinemaEnviroment;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (NetworkManager.IsClient)
            {
                NetworkWebBrowser.AdminChanged += OnBrowserAdminChanged;
                NetworkWebBrowser.AdminUrlChanged += OnBrowserAdminUrlChanged;
                NetworkWebBrowser.NetworkModeChanged += OnBrowserNetworkModeChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.IsClient)
            {
                NetworkWebBrowser.AdminChanged -= OnBrowserAdminChanged;
                NetworkWebBrowser.AdminUrlChanged -= OnBrowserAdminUrlChanged;
                NetworkWebBrowser.NetworkModeChanged -= OnBrowserNetworkModeChanged;
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

            if (NetworkWebBrowser.NetworkMode != BrowserNetworkMode.Network)
            {
                return;
            }

            if (string.IsNullOrEmpty(YoutubeLinkFunctions.GetVideoId(url)))
            {
                return;
            }

            Debug.LogError("StartTransmission");

            StartTransmission();

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

        public void StopDoubleTransmission()
        {
            StopTransmission();
        }

        private void StopTransmission()
        {
            StopAllCoroutines();
        }

        private IEnumerator YoutubeTransmittingRoutine()
        {
            Debug.LogWarning("START TRANSMITTION");
            Task<SerializableYoutubeStateData> getStateTask;
            while (true)
            {
                if (NetworkWebBrowser.WebViewPrefab.WebView == null 
                    || NetworkWebBrowser.NetworkMode != BrowserNetworkMode.Network)
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

        private async UniTask<SerializableYoutubeStateData> GetCurrentStateDataAsync()
        {
            int currentVideoNum = 0;
            await NetworkWebBrowser.WebViewPrefab.WaitUntilInitialized();

            var utcs = new UniTaskCompletionSource<SerializableYoutubeStateData>();

            SerializableYoutubeStateData stateData = new();

            string response;
            //response = await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("document.querySelector('video').currentTime");

            string video1 = await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("document.querySelectorAll('video')[0].currentTime");
            string video2 = await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript("document.querySelectorAll('video')[1].currentTime");

            var video1float = StringResponseToFloat(video1);
            var video2float = StringResponseToFloat(video2);

            if (video1float != 0)
            {
                stateData.CurrentTime = video1float;
                stateData.ActualVideoNumber = 0;
                currentVideoNum = 0;
            }
            if (video2float != 0)
            {
                stateData.CurrentTime = video2float;
                stateData.ActualVideoNumber = 1;
                currentVideoNum = 1;
            }

            response = await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video')[{currentVideoNum}].volume");
            stateData.CurrentVolume = StringResponseToFloat(response);

            response = await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video')[{currentVideoNum}].paused");
            bool.TryParse(response, out stateData.IsPaused);

            response = await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video')[{currentVideoNum}].muted");
            bool.TryParse(response, out stateData.IsMuted);

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
            await NetworkWebBrowser.WebViewPrefab.WaitUntilInitialized();

            SerializableYoutubeStateData currentStateData = await GetCurrentStateDataAsync();
            var currentVideoNum = newStateData.ActualVideoNumber;

            Debug.LogWarning($"{newStateData.ActualVideoNumber} -- {newStateData.CurrentTime} -- mute:{newStateData.IsMuted} -- pause{newStateData.IsPaused}");

            if (newStateData.IsPaused != currentStateData.IsPaused)
            {
                if (newStateData.IsPaused)
                {
                    await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video')[{currentVideoNum}].pause()");
                }
                else
                {
                    await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video')[{currentVideoNum}].play()");
                }
            }

            if (newStateData.IsPaused || Mathf.Abs(newStateData.CurrentTime - currentStateData.CurrentTime) > 5)
            {
                Debug.LogError($"Execute script {newStateData.CurrentTime}");
                await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelectorAll('video')[{currentVideoNum}].currentTime = {FloatToStringRequest(newStateData.CurrentTime)}");
            }

            if (Mathf.Abs(newStateData.CurrentVolume - currentStateData.CurrentVolume) > 0.05f)
            {
                await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelectorAll('video')[{currentVideoNum}].volume = {FloatToStringRequest(newStateData.CurrentVolume)}");
            }

            if (newStateData.IsMuted != currentStateData.IsMuted)
            {
                await NetworkWebBrowser.WebViewPrefab.WebView.ExecuteJavaScript(
                    $"document.querySelectorAll('video')[{currentVideoNum}].muted = {newStateData.IsMuted.ToString().ToLower()}");
            }
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
        private void TransferStateDataServerRpc(SerializableYoutubeStateData stateData, ulong requestingClientId)
        {
            Debug.Log($"Admin {requestingClientId} id {!NetworkWebBrowser.IsUserAdmin(requestingClientId)}");
            if (!NetworkWebBrowser.IsUserAdmin(requestingClientId))
            {
                return;
            }
            Debug.Log($"Transfer client data {stateData.CurrentTime}");
            TransferStateDataClientRpc(stateData, NetworkWebBrowser.ServerClientRpcParams);
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void TransferStateDataClientRpc(SerializableYoutubeStateData stateData, ClientRpcParams clientRpcParams = default)
        {
            SetStateDataAsync(stateData).Forget();
        }
    }
}
