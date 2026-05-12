using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.Browser;
using Assets.Mechanics.Keyboard.Scripts;
using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Vuplex.WebView;

public class PresentationPad : NetworkBehaviour
{
    #region Inspector
    [Header("Connection to browser")]
    [SerializeField] private Button _toFirstBrowser;
    [SerializeField] private Button _toLastBrowser;
    [SerializeField] private TMP_Text _currentBrowserNumText;
    [Space]
    [SerializeField] private AdminPlace _adminPlace;
    [SerializeField] private Browser _padBrowser;
    [SerializeField] private BrowserAdminControlPanel _adminControlPanel;
    [SerializeField] private List<WebPageActionsSlot> _webPageActionSlots = new List<WebPageActionsSlot>();
    [SerializeField] private XRGrabInteractable _padGrabbable;
    [SerializeField] private PadViewPermission _padViewPermission;

    [Header("Visual elements")] [SerializeField]
    private GameObject _stubWhenMainBrowserIsActive;

    [SerializeField] private GameObject _tabModel;
    [SerializeField] private CanvasGroup _tabPhantom;

    #endregion

    private TrackedDeviceGraphicRaycaster _adminUIGraphicRaycaster;
    private ViewPanel _browserView;
    private PresentationControlPanel _presentationControlPanel;
    private KeyboardWebAdapter _canvasKeyboard;
    private CanvasWebViewPrefab _webViewPrefab;
    private NetworkPresentationLaserPointMover _networkLaserPointMover;
    private PresentationLaserActivator _laserActivator;

    //private MainPresentationHolder _mainPresentationHolder;
    private PadSwitcher _padSwitcher;
    private MainPresentationForPad _currentPresentation;
    private int _currentBrowserNum = 1;

    public XRGrabInteractable PadGrabbable => _padGrabbable;

    public event Action PadCloseRequested;
    //public event Action PadConnected;
    //public event Action PadDisconnected;

    //for local pad (not this)
    private PadButtons _padButtons;
    private GameObject _localPad;
    private bool _isConnected;

    private void Awake()
    {
        //_mainPresentationHolder = FindObjectOfType<MainPresentationHolder>();

        //if (_mainPresentationHolder != null)
        //{
        //    if (_mainPresentationHolder.PadSwitcher != null)
        //    {
        //        _mainPresentationHolder.PadSwitcher.SearchFieldIsVisible = false;
        //    }
        //}

        _padSwitcher = FindObjectOfType<PadSwitcher>();
        if (_padSwitcher != null)
        {
            _padSwitcher.SearchFieldIsVisible = false;
        }
    }


    private void Start()
    {
        if (IsClient && IsOwner)
        {
            //if (_padSwitcher == null)
            _adminPlace.SetAdmin(NetworkManager.LocalClientId); //need for scenes without big browser to init the pad
            _padGrabbable.enabled = true;
        }
        else
        {
            _padGrabbable.enabled = false;
        }

        SetCurrentBrowser(_currentBrowserNum);

        if (_padSwitcher != null && _padSwitcher.Browsers.Count > 1)
        {
            _toFirstBrowser.gameObject.SetActive(true);
            _toLastBrowser.gameObject.SetActive(true);
            _currentBrowserNumText.gameObject.SetActive(true);

            _toFirstBrowser.onClick.AddListener(() => ChangeCurrentBrowser(-1));
            _toLastBrowser.onClick.AddListener(() => ChangeCurrentBrowser(1));
        }

        if (_padBrowser == null || _adminPlace == null) return;

        SetBrowser(_padBrowser);
        Init();
    }

    private void LateUpdate()
    {
        _padBrowser.transform.position = _padGrabbable.transform.position;
        _padBrowser.transform.rotation = _padGrabbable.transform.rotation;
        _padBrowser.transform.localScale = _padGrabbable.transform.localScale;
    }

    private void OnDisable()
    {
        if (_adminPlace != null)
            _adminPlace.AdminChange -= OnAdminChange;

        _adminControlPanel.CollapseMenu -= OnChangeVisibilityAdminMenu;
        _adminControlPanel.CloseRequested -= OnPadCloseRequested;
        _adminControlPanel.ConnectToBrowserRequested -= ConnectToBrowser;
        _adminControlPanel.SearchOpen -= AdminControlPanel_SearchOpen;
        _adminControlPanel.SearchClosed -= AdminControlPanel_SearchClosed;

        if (_presentationControlPanel != null)
            _presentationControlPanel.DeInit();

        _padViewPermission.VisibilityStateChanged -= OnViewPermissionVisibilityChanged;
    }

    public override void OnDestroy()
    {
        if (_padGrabbable != null)
        {
            Destroy(_padGrabbable.gameObject);
        }
        if (_currentPresentation != null) 
            _currentPresentation.MainPresentation.AdminPlace.AdminChange -= OnMainBrowserAdminChange;

        base.OnDestroy();
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            DisconnectRequested();
        }
        base.OnNetworkDespawn();
    }

    private void ChangeCurrentBrowser(int shift)
    {
        if (!(shift == 1 || shift == -1)) return;

        if (shift == 1)
        {
            if (_currentBrowserNum != _padSwitcher.Browsers.Count) _currentBrowserNum++;
            else _currentBrowserNum = 1;
        }
        else
        {
            if (_currentBrowserNum != 1) _currentBrowserNum--;
            else _currentBrowserNum = _padSwitcher.Browsers.Count;
        }
        SetCurrentBrowser(_currentBrowserNum);
        _currentBrowserNumText.text = _currentBrowserNum.ToString();
    }

    private void SetCurrentBrowser(int number)
    {
        Debug.LogError($"Set current browser {number}");

        if (IsClient)
        {
            if (_currentPresentation != null && _currentPresentation.MainPresentation.AdminPlace != null)
            {
                _currentPresentation.MainPresentation.AdminPlace.AdminChange -= OnMainBrowserAdminChange;
            }

            if (_padSwitcher != null && _padSwitcher.Browsers.Count > 0)
            {
                Debug.LogError($"try init pad switcher");
                foreach (var browser in _padSwitcher.Browsers)
                {
                    if (browser.BrowserNum == number)
                    {
                        _padSwitcher.InitPadBrowserAfterMain(browser);
                        _currentPresentation = browser;
                    }
                }
            }

            if (_currentPresentation != null && _currentPresentation.MainPresentation.AdminPlace != null)
            {
                _currentPresentation.MainPresentation.AdminPlace.AdminChange += OnMainBrowserAdminChange;
            }
        }
    }

    public void SetBrowser(Browser browser)
    {
        _padBrowser = browser;
        _adminUIGraphicRaycaster = browser.AdminUIGraphicRaycaster;
        _browserView = browser.BrowserView;
        _presentationControlPanel = browser.PresentationControlPanel;
        _canvasKeyboard = browser.CanvasKeyboard;
        _webViewPrefab = browser.WebViewPrefab;
        _networkLaserPointMover = browser.NetworkLaserPointMover;
    }
    
    public void Init()
    {
        HideAll();

        if (_laserActivator != null)
            _networkLaserPointMover.StartTransmission(_laserActivator);

        _adminControlPanel.SetLaserPointer(_padBrowser.InputDetector);
        _adminControlPanel.SetControlKeyboardInput(_padBrowser.XrWebViewTest);
        _adminControlPanel.CollapseMenu += OnChangeVisibilityAdminMenu;
        _adminControlPanel.CloseRequested += OnPadCloseRequested;
        _adminControlPanel.ConnectToBrowserRequested += ConnectToBrowser;
        _adminControlPanel.SearchOpen += AdminControlPanel_SearchOpen;
        _adminControlPanel.SearchClosed += AdminControlPanel_SearchClosed;

        _adminControlPanel.Init(_padBrowser, _adminPlace, _canvasKeyboard, _webViewPrefab);
        _presentationControlPanel.Init(_webViewPrefab);
        _networkLaserPointMover.Init(_adminPlace);

        _adminControlPanel.SearchInit(_padSwitcher);

        _padBrowser.Construct(_adminPlace);
        if (_webPageActionSlots.Count > 0)
            _padBrowser.AddWebPageSlots(_webPageActionSlots);
        _adminPlace.AdminChange += OnAdminChange;
        _adminPlace.CheckAdmin();

        _padViewPermission.VisibilityStateChanged += OnViewPermissionVisibilityChanged;
        OnViewPermissionVisibilityChanged();

        if (_padSwitcher != null)
        {
            RefreshPadByMainBrowserState();
        }
    }

    private void AdminControlPanel_SearchClosed()
    {
        if (_padSwitcher != null) 
            _padSwitcher.SearchFieldIsVisible = false;
    }

    private void AdminControlPanel_SearchOpen()
    {
        if (_padSwitcher != null) 
            _padSwitcher.SearchFieldIsVisible = true;
    }

    private void OnViewPermissionVisibilityChanged()
    {
        if (IsOwner)
        {
            _padBrowser.SetNetworkMode(_padViewPermission.IsVisibleToOthers
                ? BrowserNetworkMode.Network
                : BrowserNetworkMode.Local);
        }

        RefreshPadView();
    }

    public void SetLaserActivator(PresentationLaserActivator laserActivator)
    {
        _laserActivator = laserActivator;

        if (_networkLaserPointMover != null)
            _networkLaserPointMover.StartTransmission(_laserActivator);
    }

    private void HideAll()
    {
        if (_browserView == null)
        {
            return;
        }

        _browserView.HidePanel();
        _adminControlPanel.HidePanel();
        _presentationControlPanel.HidePanel();
        _adminUIGraphicRaycaster.enabled = false;
        _stubWhenMainBrowserIsActive.SetActive(false);
        SetPadModelVisible(false);
    }

    private void OnAdminChange(ulong previousAdmin, ulong adminId)
    {
        if (IsOwner)
        {
            _padBrowser.SetNetworkMode(BrowserNetworkMode.Local);
        }

        RefreshPadView();
    }

    private void OnChangeVisibilityAdminMenu()
    {
        _presentationControlPanel.HidePanel();
        _adminPlace.ShowAdminPlace();
    }

    private void OnPadCloseRequested()
    {
        DisconnectRequested();
        PadCloseRequested?.Invoke();
    }

    private void ConnectToBrowser()
    {
        if (_padSwitcher != null && _currentPresentation.MainPresentation.AdminPlace != null && _padSwitcher != null)
        {
            //if (!_mainPresentationHolder.IsPadConnectionAvailable())
            //{
            //    return;
            //}
            _localPad = _padSwitcher.GetPad();
            if (_currentPresentation.MainPresentation.AdminPlace.IsUserAdmin(NetworkManager.LocalClientId))
            {
                _isConnected = true;
                //PadConnected?.Invoke();

                _adminPlace.ClearAdmin();

                _localPad.transform.parent = _padGrabbable.transform;

                _localPad.transform.localPosition = Vector3.zero;
                _localPad.transform.localEulerAngles = Vector3.zero;
                _localPad.transform.localScale = Vector3.one;

                _padButtons = _padSwitcher.GetButtons(); //collect local pad buttons to network pad

                _padSwitcher.CloseBrowser += DisconnectWithMainBrowserDeactivation;
                _padButtons.CloseButtonClicked += OnPadCloseRequested;
                _padButtons.DisconnectButtonClicked += DisconnectWithMainBrowserDeactivation;

                _padSwitcher.ShowWithUrl(_padBrowser.GetCurrentUrl());
            }
            else
            {
                if (!_currentPresentation.MainPresentation.AdminPlace.IsAdminSet())
                {
                    _currentPresentation.MainPresentation.AdminPlace.SetAdmin(NetworkManager.LocalClientId);
                }
            }

            RefreshPadView();
        }
        else
            Debug.LogWarning("No PadSwitcher on Scene");
    }

    private void DisconnectRequested()
    {
        if (_isConnected && _padSwitcher != null && _localPad != null && _padButtons != null)
        {
            _padSwitcher.CloseBrowser -= DisconnectWithMainBrowserDeactivation;
            _padButtons.CloseButtonClicked -= OnPadCloseRequested;
            _padButtons.DisconnectButtonClicked -= DisconnectWithMainBrowserDeactivation;

            _isConnected = false;
            //PadDisconnected?.Invoke();

            _localPad.transform.parent = _padSwitcher.transform;
            _localPad.transform.localPosition = new Vector3(0, 100, 0);

            _adminPlace.SetAdmin(NetworkManager.Singleton.LocalClientId);


            RefreshPadView();
        }
    }

    private void DisconnectWithMainBrowserDeactivation()
    {
        if (_padSwitcher != null && _currentPresentation.MainPresentation != null 
            && _currentPresentation.MainPresentation.AdminPlace != null)
        {
            if (_currentPresentation.MainPresentation.AdminPlace.IsUserAdmin(NetworkManager.Singleton.LocalClientId))
            {
                _currentPresentation.MainPresentation.AdminPlace.ClearAdmin();
            }
        }

        DisconnectRequested();
    }

    private void OnMainBrowserAdminChange(ulong oldAdmin, ulong newAdmin)
    {
        if (!IsClient)
        {
            return;
        }

        RefreshPadByMainBrowserState();
    }

    private void RefreshPadByMainBrowserState()
    {
        if (!IsOwner)
        {
            RefreshPadView();
            return;
        }

        if (_currentPresentation.MainPresentation.AdminPlace != null &&
            _currentPresentation.MainPresentation.AdminPlace.IsAdminSet())
        {
            _adminPlace.ClearAdmin();

            if (!_isConnected &&
                _currentPresentation.MainPresentation.AdminPlace.IsUserAdmin(NetworkManager.LocalClientId))
            {
               ConnectToBrowser();
            }

            _padBrowser.SetNetworkMode(BrowserNetworkMode.Local);
        }
        else
        {
            _adminPlace.SetAdmin(NetworkManager.LocalClientId);
        }

        RefreshPadView();
    }

    private void RefreshPadView()
    {
        HideAll();

        if (IsOwner)
        {
            SetPadModelVisible(true);
            if (_padSwitcher != null && _currentPresentation.MainPresentation.AdminPlace != null &&
                _currentPresentation.MainPresentation.AdminPlace.IsAdminSet())
            {
                if (//!_mainPresentationHolder.IsPadConnectionAvailable()||
                    !_currentPresentation.MainPresentation.AdminPlace.IsUserAdmin(NetworkManager.LocalClientId))
                {
                    _stubWhenMainBrowserIsActive.SetActive(true);
                    _adminControlPanel.ShowPanel();
                    _adminControlPanel.SetSearchViewPanelAtStart();
                    _adminControlPanel.SetButtonsInteractable(false);
                    _adminControlPanel.SetCloseButtonInteractable(true);
                }
            }
            else
            {
                _browserView.ShowPanel();
                _adminUIGraphicRaycaster.enabled = true;
                _adminControlPanel.ShowPanel();
                _adminControlPanel.SetSearchViewPanelAtStart();
                _adminControlPanel.SetButtonsInteractable(true);
                _adminControlPanel.SetConnectButtonInteractable(_padSwitcher != null);
                   //&& _mainPresentationHolder.IsPadConnectionAvailable());

                _presentationControlPanel.ShowPanel();
            }
        }
        else
        {
            if (_padViewPermission.IsVisibleToOthers &&
                (_padSwitcher == null 
                || _currentPresentation.MainPresentation == null
                || _currentPresentation.MainPresentation.AdminPlace == null
                || !_currentPresentation.MainPresentation.AdminPlace.IsAdminSet()))
            {
                _browserView.ShowPanel();
                SetPadModelVisible(true);
            }
        }
    }

    public void SetPadModelVisible(bool visible)
    {
#if UNITY_SERVER
        return;
#endif

        _tabModel.SetActive(visible);
        _tabPhantom.alpha = visible ? 0 : 1;
    }
}