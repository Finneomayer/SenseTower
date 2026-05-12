using Assets.Mechanics.Browser;
using Assets.Mechanics.Keyboard.Scripts;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Vuplex.WebView;

public class Browser : NetworkBehaviour
{
    #region Inspector
    [SerializeField] private NetworkWebBrowser _networkBrowser;
    [SerializeField] private CanvasWebViewPrefab _browserPrefab;
    [SerializeField] private BrowserConfig _browserConfig;

    [SerializeField] private ViewPanel _browserView;
    [SerializeField] private TrackedDeviceGraphicRaycaster _adminUIGraphicRaycaster;
    [SerializeField] private KeyboardWebAdapter _canvasKeyboard;
    [SerializeField] private TMP_Text _browserNumText;
    [SerializeField] private CanvasWebViewPrefab _webViewPrefab;
    [SerializeField] private PresentationControlPanel _presentationControlPanel;
    [SerializeField] private NetworkPresentationLaserPointMover _networkLaserPointMover;
    [SerializeField] private DefaultPointerInputDetector _inputDetector;
    [SerializeField] private XrWebViewTest _xrWebViewTest;
    [SerializeField] private GameObject _bottomPoint;
    #endregion

    public NetworkVariable<FixedString128Bytes> LogoTowerObjectId;
    public NetworkVariable<int> BrowserNumber;
    public NetworkVariable<Vector3> BrowserScale;
    public NetworkVariable<Vector3> BrowserAdminPlace;
    public ViewPanel BrowserView { get { return _browserView; } }
    public TrackedDeviceGraphicRaycaster AdminUIGraphicRaycaster { get { return _adminUIGraphicRaycaster; } }
    public KeyboardWebAdapter CanvasKeyboard { get { return _canvasKeyboard; } }
    public CanvasWebViewPrefab WebViewPrefab { get { return _webViewPrefab; } }
    public PresentationControlPanel PresentationControlPanel { get { return _presentationControlPanel; } }
    public NetworkPresentationLaserPointMover NetworkLaserPointMover { get { return _networkLaserPointMover; } }
    public DefaultPointerInputDetector InputDetector { get { return _inputDetector; } }
    public XrWebViewTest XrWebViewTest { get { return _xrWebViewTest; } }
    public NetworkWebBrowser NetworkWebBrowser => _networkBrowser;
    public GameObject BottomPointScreen => _bottomPoint;
    private AdminPlace _adminPlace;
    private BrowserSearch _browserSearch;

    private void Awake()
    {
        _browserPrefab.InitialUrl = _browserConfig.InitialUrl;
        _browserSearch = new CommonBrowserSearch();
    }
    
    #region Public

    public void Construct(AdminPlace adminPlace)
    {
        _adminPlace = adminPlace;
        _networkBrowser.Init(_adminPlace, _browserConfig, BrowserNumber.Value);
        _browserNumText.text = BrowserNumber.Value.ToString();

        SetScreenScale(BrowserScale.Value);
        SetAdminPanelPosition(BrowserAdminPlace.Value);
    }

    private void SetScreenScale(Vector3 scale)
    {
        if (scale == Vector3.zero)
            return;
        
        _webViewPrefab.transform.localScale = scale;
    }

    private void SetAdminPanelPosition(Vector3 position)
    {
        //if (position.magnitude > 0.1f)
        //    _canvasKeyboard.transform.position = new Vector3(
        //        _canvasKeyboard.transform.position.x,
        //        position.y + 0.1f,
        //        _canvasKeyboard.transform.position.z);
    }

    public void SetUrl(string url)
    {
        OpenUrl(url);
    }

    public void AddWebPageSlots(IEnumerable<WebPageActionsSlot> slot) => slot.ToList().ForEach(x => _presentationControlPanel.AddWebPageActions(x));
    public void AddWebPageSlot(WebPageActionsSlot slot) => _presentationControlPanel.AddWebPageActions(slot);

    public void Refresh()
    {
        _networkBrowser.RefreshPage();
    }

    public string GetCurrentUrl()
    {
        return _networkBrowser.LastUrl;
    }

    public void GoBack()
    {
        _networkBrowser.GoBack();
    }

    public void SetNetworkMode(BrowserNetworkMode mode)
    {
        _networkBrowser.SetNetworkMode(mode);
    }

    #endregion

    private void OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        
        bool isUri = _browserSearch.IsUrlValid(url);

        if (!isUri)
        {
            url = _browserSearch.GetUrlForSearch(url);
        }

        _networkBrowser.OpenPage(url);
    }
}
