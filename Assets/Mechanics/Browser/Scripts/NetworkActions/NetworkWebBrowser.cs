using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;
using System.Linq;
using Assets.Mechanics.Network.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.Browser
{
    public enum BrowserNetworkMode
    {
        Local = 0,
        Network = 1,
    }

    public class NetworkWebBrowser : NetworkBehaviour
    {
        private const float MaxWaitingTimeForPageLoad = 3;

        [SerializeField]
        private NetworkVariable<BrowserNetworkMode> _currentNetworkMode = new(BrowserNetworkMode.Local);

        [SerializeField] private CanvasWebViewPrefab CanvasWebViewPrefab;
        [SerializeField] private GraphicRaycaster BrowserRaycaster;
        [SerializeField] private RawImage LoadingPageImage;
        [SerializeField] private RawImage ActiveBrowserImage;
        [SerializeField] private CanvasPointerInputDetector AdminPointerInputDetector;
        [SerializeField] private ControllableCanvasPointerInputDetector RemotePointerInputDetector;

        public String LastUrl => _lastUrl;
        private Queue<WebPageInputAction> _adminPageActionsToSend = new();
        private Queue<WebPageInputAction> _clientActionsToApply = new();
        private Queue<WebPageInputAction> _serverAllPageActions = new();
        private Stack<string> _adminUrlHistory = new();
        private int _browserNumber;

        private Coroutine _fullPageReloadingRoutine;
        private Coroutine _processingActionsRoutine;
        private Coroutine _adminInputTransmittingRoutine;

        private ClientRpcParams _serverClientRpcParams;
        private string _serverCurrentUrl = string.Empty;

        private AdminPlace _adminPlace;
        private BrowserConfig _browserConfig;
        private bool _pageLoadingInProgress;
        private bool _backPerforming;
        private string _lastUrl;
        private string[] _siteBlackList = {"okko.tv"};
        public CanvasWebViewPrefab WebViewPrefab => CanvasWebViewPrefab;
        public ClientRpcParams ServerClientRpcParams => _serverClientRpcParams;
        public BrowserNetworkMode NetworkMode => _currentNetworkMode.Value;

        public event Action<ulong> AdminChanged;
        public event Action<string> AdminUrlChanged;
        public event Action NetworkModeChanged;

        public void Init(AdminPlace adminPlace, BrowserConfig browserConfig, int browserNumber = 0)
        {
            _adminPlace = adminPlace;
            _browserConfig = browserConfig;
            _browserNumber = browserNumber;
#if UNITY_SERVER
            _adminPlace.AdminChange += OnServerAdminChange;
#endif
        }

        public bool IsUserAdmin(ulong clientId)
        {
            if (_adminPlace == null)
            {
                return false;
            }

            return _adminPlace.IsUserAdmin(clientId);
        }

        public void SetNetworkMode(BrowserNetworkMode mode)
        {
            SetNetworkModeServerRpc(mode, NetworkManager.LocalClientId);
        }

        public void OpenPage(string url)
        {
            AdminLoadPage(url);
        }

        public async void RefreshPage()
        {
            await CanvasWebViewPrefab.WaitUntilInitialized();
            await CanvasWebViewPrefab.WebView.ExecuteJavaScript(
                "window.onbeforeunload = function () {window.scrollTo(0, 0);};");
            if (!ExistSiteInBlackList(CanvasWebViewPrefab.WebView.Url))
            {
                CanvasWebViewPrefab.WebView.LoadUrl(CanvasWebViewPrefab.WebView.Url);
                ProcessNewUrl(CanvasWebViewPrefab.WebView.Url);
            }
        }

        public async void GoBack()
        {
            if (_adminUrlHistory.TryPop(out string previousUrl))
            {
                if (_adminUrlHistory.TryPeek(out string predPrevUrl))
                {
                    _lastUrl = predPrevUrl;
                }
                else
                {
                    _lastUrl = _browserConfig.InitialUrl;
                }

                AdminLoadPage(previousUrl);
            }
        }

        public void SendInputAction(WebPageInputAction webPageInputAction)
        {
            EnqueuePageAction(webPageInputAction);
        }

        private void OnServerAdminChange(ulong oldAdminClientId, ulong newAdminClientId)
        {
            _serverCurrentUrl = _browserConfig.InitialUrl;
            _serverAllPageActions.Clear();

            SetAdminClientRpc(newAdminClientId, _serverCurrentUrl);

            List<ulong> newClientIds = new();
            foreach (var id in NetworkManager.ConnectedClientsIds)
            {
                if (id != newAdminClientId)
                {
                    newClientIds.Add(id);
                }
            }

            SetClientRpcParams(newClientIds, ref _serverClientRpcParams);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientConnectedCallback += OnNetworkManagerClientConnectedCallback;
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerClientDisconnectCallback;
                _serverClientRpcParams.Send.TargetClientIds = new ulong[0];
            }

            if (IsClient)
            {
                CanvasWebViewPrefab.SetPointerInputDetector(RemotePointerInputDetector);
                RegisterBrowserEventsListeners();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientConnectedCallback -= OnNetworkManagerClientConnectedCallback;
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnNetworkManagerClientDisconnectCallback;
            }

            if (IsClient)
            {
                UnregisterBrowserEventsListeners();
            }

            base.OnNetworkDespawn();
        }

        private void OnNetworkManagerClientConnectedCallback(ulong clientId)
        {
            List<ulong> clientIds = GetClientIds(_serverClientRpcParams);
            clientIds.Add(clientId);
            SetClientRpcParams(clientIds, ref _serverClientRpcParams);

            if (string.IsNullOrEmpty(_serverCurrentUrl))
            {
                return;
            }

            clientIds = new() {clientId};
            ClientRpcParams newClientRpcParams = new();
            SetClientRpcParams(clientIds, ref newClientRpcParams);

            TransferNewPageClientRpc(_serverCurrentUrl, newClientRpcParams);

            // TODO: Applying accumulated page actions to just connected client
            //SerializableWebPageInputData data = new();
            //data.ActionsQueue = _serverAllPageActions;
            //TransferPageActionsClientRpc(data, newClientRpcParams);
        }

        private void OnNetworkManagerClientDisconnectCallback(ulong clientId)
        {
            List<ulong> clientIds = GetClientIds(_serverClientRpcParams);
            clientIds.Remove(clientId);

            SetClientRpcParams(clientIds, ref _serverClientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetNetworkModeServerRpc(BrowserNetworkMode mode, ulong requestingClientId)
        {
            if (_adminPlace != null && _adminPlace.IsUserAdmin(requestingClientId))
            {
                _currentNetworkMode.Value = mode;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void TransferNewPageServerRpc(string url, ulong requestingClientId)
        {
            if (!_adminPlace.IsUserAdmin(requestingClientId))
            {
                return;
            }

            _serverCurrentUrl = url;
            _serverAllPageActions.Clear();
            TransferNewPageClientRpc(_serverCurrentUrl, _serverClientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TransferPageActionsServerRpc(SerializableWebPageInputData data, ulong requestingClientId)
        {
            if (!_adminPlace.IsUserAdmin(requestingClientId))
            {
                return;
            }

            foreach (var item in data.ActionsQueue)
            {
                _serverAllPageActions.Enqueue(item);
            }

            TransferPageActionsClientRpc(data, _serverClientRpcParams);
        }

        [ClientRpc]
        private void TransferNewPageClientRpc(string url, ClientRpcParams clientRpcParams = default)
        {
            if (_currentNetworkMode.Value == BrowserNetworkMode.Network)
            {
                ClientLoadPage(url);
            }
            else
            {
                ClientLoadPage(_browserConfig.InitialUrl);
            }
        }

        [ClientRpc]
        private void TransferPageActionsClientRpc(SerializableWebPageInputData data,
            ClientRpcParams clientRpcParams = default)
        {
            foreach (var item in data.ActionsQueue)
            {
                _clientActionsToApply.Enqueue(item);
            }
        }

        [ClientRpc]
        private void SetAdminClientRpc(ulong newAdminId, string initialUrl, ClientRpcParams clientRpcParams = default)
        {
            if (_fullPageReloadingRoutine != null)
            {
                StopCoroutine(_fullPageReloadingRoutine);
                _fullPageReloadingRoutine = null;
            }

            if (_adminInputTransmittingRoutine != null)
            {
                StopCoroutine(_adminInputTransmittingRoutine);
                _adminInputTransmittingRoutine = null;
            }

            if (_processingActionsRoutine != null)
            {
                StopCoroutine(_processingActionsRoutine);
                _processingActionsRoutine = null;
            }

            SetLoadingStubActive(false);

            AdminChanged?.Invoke(newAdminId);

            if (NetworkManager.LocalClientId == newAdminId)
            {
                _adminUrlHistory.Clear();
                _adminUrlHistory.Push(initialUrl);

                SetInteractable(true);
                CanvasWebViewPrefab.SetPointerInputDetector(AdminPointerInputDetector);

                _adminInputTransmittingRoutine = StartCoroutine(AdminInputTransmittingRoutine());
                if (_browserConfig != null && !string.IsNullOrEmpty(_browserConfig.LastUrlPlayerPrefsKey)
                                           && PlayerPrefs.HasKey(_browserConfig.LastUrlPlayerPrefsKey + _browserNumber))
                {
                    initialUrl = PlayerPrefs.GetString(_browserConfig.LastUrlPlayerPrefsKey + _browserNumber);
                }
                else
                {
                    if (_browserConfig != null)
                        initialUrl = _browserConfig.InitialUrl;
                }

                if (_browserConfig != null)
                    _lastUrl = initialUrl;

                AdminLoadPage(initialUrl);
            }
            else
            {
                SetInteractable(false);
                CanvasWebViewPrefab.SetPointerInputDetector(RemotePointerInputDetector);
                if (_browserConfig != null && !ExistSiteInBlackList(_browserConfig.InitialUrl))
                    _processingActionsRoutine =
                        StartCoroutine(ClientProcessingPageActionsRoutine(_browserConfig.InitialUrl));
            }
        }

        private async void RegisterBrowserEventsListeners()
        {
            _currentNetworkMode.OnValueChanged += OnNetworkModeChanged;

            await CanvasWebViewPrefab.WaitUntilInitialized();
            CanvasWebViewPrefab.WebView.LoadProgressChanged += OnWebViewLoadProgressChanged;
            CanvasWebViewPrefab.WebView.UrlChanged += OnWebViewUrlChanged;

            AdminPointerInputDetector.BeganDrag += OnLocalInputBeganDrag;
            AdminPointerInputDetector.Dragged += OnLocalInputDragged;
            AdminPointerInputDetector.PointerDown += OnLocalInputPointerDown;
            AdminPointerInputDetector.PointerExited += OnLocalInputPointerExited;
            AdminPointerInputDetector.PointerUp += OnLocalInputPointerUp;
            AdminPointerInputDetector.Scrolled += OnLocalInputScrolled;
        }

        private void UnregisterBrowserEventsListeners()
        {
            _currentNetworkMode.OnValueChanged -= OnNetworkModeChanged;

            if (CanvasWebViewPrefab.WebView != null)
            {
                CanvasWebViewPrefab.WebView.LoadProgressChanged -= OnWebViewLoadProgressChanged;
                CanvasWebViewPrefab.WebView.UrlChanged -= OnWebViewUrlChanged;
            }

            AdminPointerInputDetector.BeganDrag -= OnLocalInputBeganDrag;
            AdminPointerInputDetector.Dragged -= OnLocalInputDragged;
            AdminPointerInputDetector.PointerDown -= OnLocalInputPointerDown;
            AdminPointerInputDetector.PointerExited -= OnLocalInputPointerExited;
            AdminPointerInputDetector.PointerUp -= OnLocalInputPointerUp;
            AdminPointerInputDetector.Scrolled -= OnLocalInputScrolled;
        }

        private void OnWebViewLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.Type)
            {
                case ProgressChangeType.Started:
                    _pageLoadingInProgress = true;
                    break;
                case ProgressChangeType.Finished:
                case ProgressChangeType.Failed:
                    _pageLoadingInProgress = false;
                    break;
                default:
                    break;
            }
        }

        private void OnNetworkModeChanged(BrowserNetworkMode previousValue, BrowserNetworkMode newValue)
        {
            if (!IsClient)
            {
                return;
            }

            NetworkModeChanged?.Invoke();

            if (_adminPlace == null)
            {
                return;
            }

            if (_adminPlace.IsUserAdmin(NetworkManager.LocalClientId))
            {
                if (newValue == BrowserNetworkMode.Network)
                {
                    RefreshPage();
                }
            }
            else
            {
                if (newValue != BrowserNetworkMode.Network)
                {
                    ClientLoadPage(_browserConfig.InitialUrl);
                }
            }
        }

        private void OnWebViewUrlChanged(object sender, UrlChangedEventArgs e)
        {
            if (ExistSiteInBlackList(e.Url))
            {
                CanvasWebViewPrefab.WebView.StopLoad();
            }

            if (IsUserAdmin(NetworkManager.LocalClientId)
                && !string.IsNullOrEmpty(_lastUrl) && _lastUrl != e.Url)
            {
                if (!_adminUrlHistory.TryPeek(out string lastHistoryUrl)
                    || lastHistoryUrl != _lastUrl)
                {
                    _adminUrlHistory.Push(_lastUrl);
                }
            }

            _lastUrl = e.Url;
            ProcessNewUrl(e.Url);
        }

        private void OnLocalInputScrolled(object sender, ScrolledEventArgs e)
        {
            EnqueuePageAction(new ScrolledWebPageInputAction(e));
        }

        private void OnLocalInputPointerUp(object sender, PointerEventArgs e)
        {
            EnqueuePageAction(new PointerUpWebPageInputAction(e));
        }

        private void OnLocalInputPointerExited(object sender, EventArgs e)
        {
            EnqueuePageAction(new PointerExitedWebPageInputAction(e));
        }

        private void OnLocalInputPointerDown(object sender, PointerEventArgs e)
        {
            EnqueuePageAction(new PointerDownWebPageInputAction(e));
        }

        private void OnLocalInputDragged(object sender, EventArgs<Vector2> e)
        {
            EnqueuePageAction(new DraggedWebPageInputAction(e));
        }

        private void OnLocalInputBeganDrag(object sender, EventArgs<Vector2> e)
        {
            EnqueuePageAction(new BeganDragWebPageInputAction(e));
        }

        private void EnqueuePageAction(WebPageInputAction webPageInputAction)
        {
            if (_currentNetworkMode.Value != BrowserNetworkMode.Network || !IsUserAdmin(NetworkManager.LocalClientId))
            {
                return;
            }

            _adminPageActionsToSend.Enqueue(webPageInputAction);
        }

        //TODO: Trying of sum dragging actions. Not working. Delete or remake.
        private void EnqueueDraggedAction(EventArgs<Vector2> draggedEventArgs)
        {
            bool valueAccumulated = false;
            if (_adminPageActionsToSend.Count > 0)
            {
                WebPageInputAction inputAction = _adminPageActionsToSend.Last();
                if (inputAction.WebPageInputActionType == WebPageInputActionType.Dragged)
                {
                    DraggedWebPageInputAction lastDraggedAction = (DraggedWebPageInputAction) inputAction;
                    Vector2 currentDraggedVector = lastDraggedAction.EventArgs.Value;
                    currentDraggedVector += draggedEventArgs.Value;

                    lastDraggedAction.EventArgs = new EventArgs<Vector2>(currentDraggedVector);

                    valueAccumulated = true;
                }
            }

            if (!valueAccumulated)
            {
                _adminPageActionsToSend.Enqueue(new DraggedWebPageInputAction(draggedEventArgs));
            }
        }

        private void SetClientRpcParams(List<ulong> clientIds, ref ClientRpcParams clientRpcParams)
        {
            clientRpcParams = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = clientIds
                }
            };
        }

        private List<ulong> GetClientIds(in ClientRpcParams clientRpcParams)
        {
            return clientRpcParams.Send.TargetClientIds == null
                ? new()
                : _serverClientRpcParams.Send.TargetClientIds.ToList();
        }

        private async void AdminLoadPage(string url)
        {
            await CanvasWebViewPrefab.WaitUntilInitialized();

            if (CanvasWebViewPrefab.WebView == null)
            {
                Debug.LogError("NetworkWebBrowser. Cannot load page: WebView == null");
                return;
            }

            await CanvasWebViewPrefab.WebView.ExecuteJavaScript(
                "window.onbeforeunload = function () {window.scrollTo(0, 0);};");

            if (CanvasWebViewPrefab.WebView == null)
            {
                Debug.LogError("NetworkWebBrowser. Cannot load page: WebView == null");
                return;
            }

            if (!CanvasWebViewPrefab.WebView.IsDisposed && !ExistSiteInBlackList(url))
            {
                await Task.Delay(300);
                CanvasWebViewPrefab.WebView.LoadUrl(url);
            }
        }

        private void ClientLoadPage(string url)
        {
            if (_processingActionsRoutine != null)
            {
                StopCoroutine(_processingActionsRoutine);
            }

            if (!ExistSiteInBlackList(url))
                _processingActionsRoutine = StartCoroutine(ClientProcessingPageActionsRoutine(url));
        }

        private void ProcessNewUrl(string url)
        {
            if (!IsClient || _adminPlace == null)
            {
                return;
            }

            if (_fullPageReloadingRoutine != null || !_adminPlace.IsUserAdmin(NetworkManager.LocalClientId))
            {
                return;
            }

            if (!string.IsNullOrEmpty(_browserConfig.LastUrlPlayerPrefsKey))
            {
                PlayerPrefs.SetString(_browserConfig.LastUrlPlayerPrefsKey + _browserNumber, url);
            }

            _adminPageActionsToSend.Clear();

            if (_currentNetworkMode.Value == BrowserNetworkMode.Network)
            {
                TransferNewPageServerRpc(url, NetworkManager.LocalClientId);
            }

            AdminUrlChanged?.Invoke(url);
        }

        private void SetInteractable(bool active)
        {
            BrowserRaycaster.enabled = active;
        }

        private void SetLoadingStubActive(bool active)
        {
            LoadingPageImage.gameObject.SetActive(active);
            ActiveBrowserImage.gameObject.SetActive(!active);
        }

        private IEnumerator AdminInputTransmittingRoutine()
        {
            Task task = CanvasWebViewPrefab.WaitUntilInitialized();
            yield return new WaitUntil(() => task.IsCompleted);

            while (true)
            {
                if (_adminPageActionsToSend.Count == 0)
                {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(0.1f);

                if (_adminPageActionsToSend.Count == 0)
                {
                    continue;
                }

                SerializableWebPageInputData data = new();
                data.ActionsQueue = _adminPageActionsToSend;

                TransferPageActionsServerRpc(data, NetworkManager.LocalClientId);

                _adminPageActionsToSend.Clear();
            }
        }

        private IEnumerator ClientProcessingPageActionsRoutine(string url)
        {
            _clientActionsToApply.Clear();

            Task task = CanvasWebViewPrefab.WaitUntilInitialized();
            yield return new WaitUntil(() => task.IsCompleted);
            CanvasWebViewPrefab.WebView.LoadUrl(url);

            Task pageLoadTask = CanvasWebViewPrefab.WebView.WaitForNextPageLoadToFinish();
            float tryingLoadEndTime = Time.unscaledTime + MaxWaitingTimeForPageLoad;
            yield return new WaitUntil(() => pageLoadTask.IsCompleted || tryingLoadEndTime < Time.unscaledTime);

            yield return new WaitForSeconds(1f);

            while (true)
            {
                while (_clientActionsToApply.Count > 0)
                {
                    WebPageInputActionType currentActionType = _clientActionsToApply.Peek().WebPageInputActionType;
                    if (currentActionType != WebPageInputActionType.Dragged
                        && currentActionType != WebPageInputActionType.Scrolled)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }

                    if (_clientActionsToApply.Count == 0)
                    {
                        break;
                    }

                    _clientActionsToApply.Dequeue().ProcessAction(RemotePointerInputDetector);
                }

                yield return null;
            }
        }

        private bool ExistSiteInBlackList(string url)
        {
            for (int i = 0; i < _siteBlackList.Length; i++)
            {
                if (url.Contains(_siteBlackList[i]))
                {
                    return true;
                }
            }

            return false;
        }

        //TODO: Remove if it is will not needed
        private IEnumerator AdminFullLoadingRoutine(string url)
        {
            _adminPageActionsToSend.Clear();

            SetInteractable(false);
            SetLoadingStubActive(false);

            Task task = CanvasWebViewPrefab.WaitUntilInitialized();
            yield return new WaitUntil(() => task.IsCompleted);

            Task<byte[]> screenTask = CanvasWebViewPrefab.WebView.CaptureScreenshot();
            while (!screenTask.IsCompleted)
            {
                yield return null;
            }

            Texture2D texture2D = new(1, 1);
            texture2D.LoadImage(screenTask.Result);
            texture2D.Apply();

            LoadingPageImage.texture = texture2D;
            SetLoadingStubActive(true);

            Task pageLoadTask = CanvasWebViewPrefab.WebView.WaitForNextPageLoadToFinish();
            float tryingLoadEndTime = Time.unscaledTime + MaxWaitingTimeForPageLoad;
            yield return new WaitUntil(() => pageLoadTask.IsCompleted || tryingLoadEndTime < Time.unscaledTime);

            yield return new WaitForSeconds(1f);
            SetInteractable(true);
            SetLoadingStubActive(false);

            _fullPageReloadingRoutine = null;
        }
    }
}