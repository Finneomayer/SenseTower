using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.Browser;
using Assets.Mechanics.Keyboard.Scripts;
using Cysharp.Threading.Tasks;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Vuplex.WebView;

public class Presentation : MonoBehaviour
{
    #region Inspector
    [SerializeField] private AdminPlace _adminPlace;
    [SerializeField] private AgoraVoice _agoraVoice;
    [SerializeField] private Browser _browser;
    [SerializeField] private BrowserAdminControlPanel _adminControlPanel;
    [SerializeField] private List<WebPageActionsSlot> _webPageActionSlots = new();
    public XRSimpleInteractable SenseLogo;
    #endregion

    public event Action BrowserShownToAdmin;//used for admin pad 
    public event Action BrowserHiddenToAdmin;//used for admin pad 
    private TrackedDeviceGraphicRaycaster _adminUIGraphicRaycaster;
    private ViewPanel _browserView;
    private PresentationControlPanel _presentationControlPanel;
    private KeyboardWebAdapter _canvasKeyboard;
    private CanvasWebViewPrefab _webViewPrefab;
    private NetworkPresentationLaserPointMover _networkLaserPointMover;
    private Coroutine _excludeSpatialAudio;
    private Coroutine _disableExcludeSpatialAudio;
    private PresentationLaserActivator _laserActivator;
    
    private bool _isUserHavePermissionManage = true;

    public AdminPlace AdminPlace => _adminPlace;
    public Browser Browser => _browser;

    private void Start()
    {
        _agoraVoice = FindObjectOfType<AgoraVoice>();

        HideAll();
        if (_browser == null || _adminPlace == null) return;
        
        SetBrowser(_browser);
        SetAdminPlace(_adminPlace);

        Init();
    }

    private void OnDisable()
    {
        if(_adminPlace != null)
            _adminPlace.AdminChange -= OnAdminChange;
       
        _adminControlPanel.CollapseMenu -= OnChangeVisibilityAdminMenu;
        
        if(_presentationControlPanel != null)
            _presentationControlPanel.DeInit();
    }

    public void SetBrowser(Browser browser) 
    {
        _browser = browser;
        _adminUIGraphicRaycaster = browser.AdminUIGraphicRaycaster;
        _browserView = browser.BrowserView;
        _presentationControlPanel = browser.PresentationControlPanel;
        _canvasKeyboard = browser.CanvasKeyboard;
        _webViewPrefab = browser.WebViewPrefab;
        _networkLaserPointMover = browser.NetworkLaserPointMover;

    }

    public void SetAdminPlace(AdminPlace adminPlace) 
    {
        _adminPlace = adminPlace;
    }

    public void SetAdminPanelPosition()
    {
        Vector3 adminControlPosition =_browser.BottomPointScreen == null ? _browser.transform.position : _browser.BottomPointScreen.transform.position;
        //adminControlPosition.y -= 0.4f;

        _canvasKeyboard.transform.position = adminControlPosition;
        _canvasKeyboard.transform.localPosition = new Vector3(_canvasKeyboard.transform.localPosition.x,
            _canvasKeyboard.transform.localPosition.y - 650f,
            _canvasKeyboard.transform.localPosition.z - 300f);

        //_adminControlPanel.transform.position = adminControlPosition;
        _adminControlPanel.transform.SetParent(_canvasKeyboard.transform.GetChild(0));
        _adminControlPanel.transform.localRotation = Quaternion.identity;
        _adminControlPanel.transform.localPosition = new Vector3(_adminControlPanel.transform.localPosition.x,
            _adminControlPanel.transform.position.y + 260, 0);

        //Vector3 canvasKeyboardPosition = _canvasKeyboard.transform.position;
        //canvasKeyboardPosition.y = adminControlPosition.y - 0.4f;
        //_canvasKeyboard.transform.position = canvasKeyboardPosition;

    }

    public void RaiseWebPageAction(WebPageAction webPageAction)
    {
        if (_presentationControlPanel != null)
        {
            _presentationControlPanel.RaiseWebPageAction(webPageAction);
        }
    }

    public void AddNewWebPageSlot(WebPageActionsSlot webPageActionsSlot)
    {
        if (!_webPageActionSlots.Contains(webPageActionsSlot))
        {
            _webPageActionSlots.Add(webPageActionsSlot);
            _browser.AddWebPageSlot(webPageActionsSlot);
        }
    }

    public void Init() 
    {
        HideAll();
        
        _adminControlPanel.SetLaserPointer(_browser.InputDetector);
        _adminControlPanel.SetControlKeyboardInput(_browser.XrWebViewTest);
        _adminControlPanel.CollapseMenu += OnChangeVisibilityAdminMenu;

        _adminControlPanel.Init(_browser, _adminPlace, _canvasKeyboard, _webViewPrefab);
        _presentationControlPanel.Init(_webViewPrefab);
        _networkLaserPointMover.Init(_adminPlace);
        
        _browser.Construct(_adminPlace);
        if (_webPageActionSlots.Count > 0)
            _browser.AddWebPageSlots(_webPageActionSlots);
        _adminPlace.ShowAdminPlace();
        _adminPlace.AdminChange += OnAdminChange;
        _adminPlace.CheckAdmin();

        InitLaserActivator().Forget();
    }

    public void ToogleManagePermission(bool value) 
    {
        _isUserHavePermissionManage = value;
    }

    public void SetLaserActivator(PresentationLaserActivator laserActivator)
    {
        _laserActivator = laserActivator;

        if (_networkLaserPointMover != null)
            _networkLaserPointMover.StartTransmission(_laserActivator);
    }

    private async UniTask InitLaserActivator()
    {
        CompositionRootNetworkScene compositionRoot = FindObjectOfType<CompositionRootNetworkScene>();
        if (compositionRoot == null)
        {
            return;
        }

        if (!compositionRoot.Initialized)
        {
            await UniTask.WaitUntil(() => compositionRoot.Initialized);
        }
        compositionRoot.InitPresentationNetwork(this);
    }

    public void HideAll()
    {
        if(_adminPlace != null)
            _adminPlace.HideAdminPlace();
        
        if(_browserView != null)
            _browserView.HidePanel();
        
        if(_adminControlPanel != null)
            _adminControlPanel.HidePanel();          
        
        if(_presentationControlPanel != null)
            _presentationControlPanel.HidePanel();
    }

    private void OnAdminChange(ulong previousAdmin,ulong adminId)
    {
        if (!_adminPlace.IsAdminSet())
        {
            HideAll();
            BrowserHiddenToAdmin?.Invoke();//used for admin pad            
            _disableExcludeSpatialAudio = StartCoroutine(DisableExcludeAdminInSpatialAudio(previousAdmin));
            return;
        }

        _browserView.ShowPanel();
        
        if(_excludeSpatialAudio != null)
            StopCoroutine(_excludeSpatialAudio);
        
        if(_disableExcludeSpatialAudio != null)
            StopCoroutine(_disableExcludeSpatialAudio);
        
        _excludeSpatialAudio = StartCoroutine(ExcludeAdminInSpatialAudio(adminId));
        _disableExcludeSpatialAudio = StartCoroutine(DisableExcludeAdminInSpatialAudio(previousAdmin));

        if (NetworkManager.Singleton.LocalClientId == adminId)
        {
            _browser.SetNetworkMode(BrowserNetworkMode.Network);
            if (_isUserHavePermissionManage)
            {
                _adminControlPanel.ShowPanel();
                BrowserShownToAdmin?.Invoke();//used for admin pad 
            }              

            _adminUIGraphicRaycaster.enabled = true;
            _presentationControlPanel.ShowPanel();
        }
        else
        {
            _adminControlPanel.HidePanel();
            _adminUIGraphicRaycaster.enabled = false;
            _presentationControlPanel.HidePanel();
            BrowserHiddenToAdmin?.Invoke();//used for admin pad 
        }
    }

    private void OnChangeVisibilityAdminMenu()
    {
        _presentationControlPanel.HidePanel();
        _adminPlace.ShowAdminPlace();
    }

    private IEnumerator ExcludeAdminInSpatialAudio(ulong clientId)
    {
        yield return new WaitUntil(()=> _agoraVoice.GetCurrentPlayerSpatialAudio() != null);
        _agoraVoice.GetCurrentPlayerSpatialAudio().MaxVolume(clientId);
    }

    private IEnumerator DisableExcludeAdminInSpatialAudio(ulong clientId)
    {
        yield return new WaitUntil(()=> _agoraVoice.GetCurrentPlayerSpatialAudio() != null);
        _agoraVoice.GetCurrentPlayerSpatialAudio().DisableMaxVolume(clientId);
    }
}