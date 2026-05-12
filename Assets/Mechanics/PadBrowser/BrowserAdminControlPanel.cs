using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using Vuplex.WebView;
using Zenject;

public class BrowserAdminControlPanel : ViewPanel
{
    #region Inspector
    [SerializeField] private ViewPanel _searchPanel;
    [SerializeField] private CustomSlider _volumeSlider;
    [SerializeField] private ViewPanel _volumeSliderPanel;

    [Space] [SerializeField] private GameObject _bottomButtonPanel;
    [SerializeField] private ButtonUI _exitButton;
    [SerializeField] private ButtonUI _searchButton;
    [SerializeField] private ButtonUI _refreshButton;
    [SerializeField] private ButtonUI _pageDownButton;
    [SerializeField] private ButtonUI _volumeButton;
    [SerializeField] private ButtonUI _screenSharingButton;
    [SerializeField] private ButtonUI _lightButton;
    [SerializeField] private ButtonUI _collapseButton;
    [SerializeField] private ButtonUI _padVisibleButton;
    [SerializeField] private ButtonUI _padConnectToBrowserButton;

    [SerializeField] private TMP_InputField _searchPanelTextField;
    #endregion

    public CustomSlider VolumeSlider { get; private set; }

    public event Action CollapseMenu;
    public event Action<bool> ToogleLight;
    public event Action<bool> PadClickVisible;
    public event Action ConnectToBrowserRequested;
    public event Action SearchOpen;
    public event Action SearchClosed;

    private Browser _browser;
    private AdminPlace _adminPlace;
    private KeyboardWebAdapter _canvasKeyboard;
    private CanvasWebViewPrefab _webViewPrefab;
    private ViewPanel _keyboardPanel;
    private DefaultPointerInputDetector _inputDetector;
    private XrWebViewTest _xrWebViewTest;
    private PadSwitcher _padSwitcher; //used for connection to local Pad

    private bool isSearchViewPanelVisible = false;
    private bool _isSceneDark = false;
    private bool _isVolumeViewPanelVisible = false;
    private bool isBrowserdFocused = false;
    private bool _isPadVisibleToOthers;

    private List<ButtonUI> _buttons = new();

    private IClientData _clientData;

    public event Action CloseRequested;

    [Inject]
    private void Construct(IClientData clientData)
    {
        _clientData = clientData;
    }

    private void Awake()
    {
        if (_searchPanelTextField != null)
        {
            _searchPanelTextField.shouldHideSoftKeyboard = true;
        }

        _buttons.Add(_exitButton);
        _buttons.Add(_searchButton);
        _buttons.Add(_refreshButton);
        _buttons.Add(_pageDownButton);
        _buttons.Add(_volumeButton);
        _buttons.Add(_screenSharingButton);
        _buttons.Add(_lightButton);
        _buttons.Add(_collapseButton);
        _buttons.Add(_padVisibleButton);
        _buttons.Add(_padConnectToBrowserButton);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (_searchPanelTextField.isFocused && isBrowserdFocused)
        {
            isBrowserdFocused = false;
            _xrWebViewTest.IsAllowSendKeyToBrowser = false;
        }
    }

    public string GetInputFieldValue()
    {
        return _searchPanelTextField.text;
    }

    public void SetInputFieldFocused()
    {
        isBrowserdFocused = false;
        _searchPanelTextField.Select();
        _xrWebViewTest.IsAllowSendKeyToBrowser = false;
    }

    public void SetBrowserFocused()
    {
        isBrowserdFocused = true;
        _xrWebViewTest.IsAllowSendKeyToBrowser = true;
    }

    public void SetLaserPointer(DefaultPointerInputDetector inputDetector)
    {
        _inputDetector = inputDetector;
    }

    public void SetControlKeyboardInput(XrWebViewTest xrWebViewTest)
    {
        _xrWebViewTest = xrWebViewTest;
    }

    public void Init(Browser browser, AdminPlace adminPlace, KeyboardWebAdapter canvasKeyboard,
        CanvasWebViewPrefab webViewPrefab)
    {
        UnsubscribeEvents();

        if (_bottomButtonPanel != null) _bottomButtonPanel.SetActive(true);

        _adminPlace = adminPlace;
        _browser = browser;
        _canvasKeyboard = canvasKeyboard;

        if (_canvasKeyboard.TryGetComponent(out ViewPanel viewPanel))
            _keyboardPanel = viewPanel;

        _webViewPrefab = webViewPrefab;

        _adminPlace.AdminChange += AdminPlaceOnAdminChange;

        if (_webViewPrefab.WebView != null)
        {
            OnBrowserInitialized(null, null);
        }
        else
        {
            _webViewPrefab.Initialized += OnBrowserInitialized;
        }
    }

    public void SearchInit(PadSwitcher padSwitcher)
    {
        if (padSwitcher != null) _padSwitcher = padSwitcher;
    }

    public void AllowCloseButtonOnly()
    {
        SetButtonsInteractable(false, _exitButton);
    }

    public void SetCloseButtonInteractable(bool interactable)
    {
        SetButtonInteractable(_exitButton, interactable);
    }

    public void SetConnectButtonInteractable(bool interactable)
    {
        SetButtonInteractable(_padConnectToBrowserButton, interactable);
    }

    private void SetButtonInteractable(ButtonUI button, bool interactable)
    {
        if (button == null)
        {
            return;
        }
        button.SetButtonInteractable(interactable);
    }

    public void SetButtonsInteractable(bool interactable)
    {
        SetButtonsInteractable(interactable, null);
    }

    private void SetPadVisibleButtonActive(bool active)
    {
        if (_padVisibleButton != null)
        {
            _padVisibleButton.SetButtonActive(active);
        }
    }

    private void SetSearchButtonActive(bool active)
    {
        if (_searchButton != null)
        {
            _searchButton.SetButtonActive(active);
        }
    }

    private void SetVolumeButtonActive(bool active)
    {
        if (_volumeButton != null)
        {
            _volumeButton.SetButtonActive(active);
        }
    }

    private void SetButtonsInteractable(bool interactable, params ButtonUI[] excludeButtons)
    {
        foreach (var button in _buttons)
        {
            if (button == null)
            {
                continue;
            }
            if (excludeButtons != null && excludeButtons.Contains(button))
            {
                continue;
            }
            button.SetButtonInteractable(interactable);
        }
    }

    private void SubscribeEvents()
    {
        _inputDetector.PointerDown += InputDetectorOnPointerDown;
        _canvasKeyboard.KeyPressed += OnKeyboardInputHandler;

        _searchButton.InteractElement.onClick.AddListener(ToogleSearchViewPanel);
        _exitButton.InteractElement.onClick.AddListener(OnCloseRequested);
        _refreshButton.InteractElement.onClick.AddListener(delegate { _browser.Refresh(); });
        _pageDownButton.InteractElement.onClick.AddListener(delegate { _browser.GoBack(); });

        if (_screenSharingButton != null)
            _screenSharingButton.InteractElement.onClick.AddListener(()=>
            {
                OpenScreenSharingUrl(_clientData.UserId.ToString()); 
            });

        if (_volumeButton != null)
            _volumeButton.InteractElement.onClick.AddListener(ToogleVolumeSliderViewPanel);
        if (_collapseButton != null)
            _collapseButton.InteractElement.onClick.AddListener(CollapseAdminMenu);
        if (_lightButton != null)
            _lightButton.InteractElement.onClick.AddListener(ToogleSceneLight);

        if (_padConnectToBrowserButton != null)
            _padConnectToBrowserButton.InteractElement.onClick.AddListener(() => ConnectToBrowserRequested?.Invoke());

        if (_padVisibleButton != null)
            _padVisibleButton.InteractElement.onClick.AddListener(OnSetPadVisibleToOthersButtonClick);
    }

    private void OnSetPadVisibleToOthersButtonClick()
    {
        _isPadVisibleToOthers = !_isPadVisibleToOthers;
        SetPadVisibleButtonActive(_isPadVisibleToOthers);
        PadClickVisible?.Invoke(_isPadVisibleToOthers);
    }

    private void UnsubscribeEvents()
    {
        if (_adminPlace != null)
            _adminPlace.AdminChange -= AdminPlaceOnAdminChange;
        if (_webViewPrefab != null)
            _webViewPrefab.Initialized -= OnBrowserInitialized;
        if (_canvasKeyboard != null)
            _canvasKeyboard.KeyPressed -= OnKeyboardInputHandler;
        if (_inputDetector != null)
            _inputDetector.PointerDown -= InputDetectorOnPointerDown;

        _searchButton.InteractElement.onClick.RemoveAllListeners();
        _exitButton.InteractElement.onClick.RemoveAllListeners();
        _refreshButton.InteractElement.onClick.RemoveAllListeners();
        _pageDownButton.InteractElement.onClick.RemoveAllListeners();

        //ScreenSharing on meetting small scene
        if (_screenSharingButton != null)
            _screenSharingButton.InteractElement.onClick.RemoveAllListeners();
        // CinemaScene buttons
        if (_volumeButton != null)
            _volumeButton.InteractElement.onClick.RemoveAllListeners();
        if (_lightButton != null)
            _lightButton.InteractElement.onClick.RemoveAllListeners();
        if (_collapseButton != null)
            _collapseButton.InteractElement.onClick.RemoveAllListeners();
        if (_padVisibleButton != null)
            _padVisibleButton.InteractElement.onClick.RemoveAllListeners();
        if (_padConnectToBrowserButton != null)
            _padConnectToBrowserButton.InteractElement.onClick.RemoveAllListeners();
    }

    private void AdminPlaceOnAdminChange(ulong previousValue, ulong clientId)
    {
        if (!_adminPlace.IsAdminSet())
        {
            if (_lightButton != null)
                _lightButton.SetStartStateButton();

            _isSceneDark = false;
            _isVolumeViewPanelVisible = false;
            isSearchViewPanelVisible = false;
            SetSearchButtonActive(false);
            SetVolumeButtonActive(false);

            _isPadVisibleToOthers = false;
            SetPadVisibleButtonActive(false);
        }

        if (clientId != NetworkManager.Singleton.LocalClientId)
            _keyboardPanel.HidePanel();
        else
            _keyboardPanel.ShowPanel();

        SetActiveViewPanel(_volumeSliderPanel, false);

        _searchPanel.HidePanel();
    }

    private async void OnCloseRequested()
    {
        await UniTask.Delay(100);
        _adminPlace.ClearAdmin();
        CloseRequested?.Invoke();
    }

    private void OnBrowserInitialized(object sender, EventArgs e)
    {
        SubscribeEvents();
    }

    private void InputDetectorOnPointerDown(object sender, PointerEventArgs e)
    {
        isBrowserdFocused = true;
        _xrWebViewTest.IsAllowSendKeyToBrowser = true;
    }

    private void OnKeyboardInputHandler(object sender, EventArgs<string> e)
    {
        if (isBrowserdFocused) return;

        string tempValue = string.Empty;

        if (e.Value.Equals("Backspace"))
        {
            if (_searchPanelTextField.text.Length > 0)
            {
                _searchPanelTextField.text = _searchPanelTextField.text.Remove(_searchPanelTextField.text.Length - 1);
            }
        }
        else if (e.Value.Equals("Enter"))
        {
            _browser.SetUrl(_searchPanelTextField.text);
        }
        else if (e.Value.Equals("ArrowUp") ||
                e.Value.Equals("ArrowLeft") ||
                e.Value.Equals("ArrowDown") ||
                e.Value.Equals("ArrowRight"))
        {
        }
        else if (e.Value.Equals(" "))
        {
            _searchPanelTextField.text += " ";
        }
        else
        {
            tempValue = e.Value.Split(" ")[0];
            _searchPanelTextField.text += tempValue;
        }
    }

    private void OpenScreenSharingUrl(string clientId)
    {
        string url = $"{APIService.GetScreenSharingUrl}{clientId}";
        _browser.SetUrl(url);
    }

    private void ToogleSearchViewPanel()
    {
        if (isSearchViewPanelVisible)
        {
            _searchPanel.HidePanel();
            SearchClosed?.Invoke();
        }
        else
        {
            _isVolumeViewPanelVisible = false;
            SetActiveViewPanel(_volumeSliderPanel, false);
            _searchPanel.ShowPanel();
            _searchPanelTextField.Select();
            _xrWebViewTest.IsAllowSendKeyToBrowser = false;
            SearchOpen?.Invoke();
            SetVolumeButtonActive(false);
        }

        isSearchViewPanelVisible = !isSearchViewPanelVisible;

        SetSearchButtonActive(isSearchViewPanelVisible);
    }

    public void SetSearchViewPanelAtStart()
    {
        if (_padSwitcher != null)
        {
            if (_padSwitcher.SearchFieldIsVisible) _searchPanel.ShowPanel();
            else _searchPanel.HidePanel();

            SetSearchButtonActive(_padSwitcher.SearchFieldIsVisible);
        }
    }

    private void ToogleVolumeSliderViewPanel()
    {
        if (_isVolumeViewPanelVisible)
        {
            SetActiveViewPanel(_volumeSliderPanel, false);
        }
        else
        {
            isSearchViewPanelVisible = false;
            SetSearchButtonActive(false);
            _searchPanel.HidePanel();
            SetActiveViewPanel(_volumeSliderPanel, true);
        }
        
        _isVolumeViewPanelVisible = !_isVolumeViewPanelVisible;

        SetVolumeButtonActive(_isVolumeViewPanelVisible);
    }

    private void ToogleSceneLight()
    {
        _isSceneDark = !_isSceneDark;

        ToogleLight?.Invoke(_isSceneDark);
    }

    private async void CollapseAdminMenu()
    {
        await UniTask.Delay(400);

        HidePanel();
        _keyboardPanel.HidePanel();

        SetActiveViewPanel(_volumeSliderPanel, false);

        CollapseMenu?.Invoke();
    }

    private void SetActiveViewPanel(ViewPanel viewPanel, bool active)
    {
        if (viewPanel == null)
        {
            return;
        }

        if (active)
        {
            viewPanel.ShowPanel();
        }
        else
        {
            viewPanel.HidePanel();
        }
    }

    public void SearchButtonClick()
    {
        _searchButton.InteractElement.onClick.Invoke();
    }
    public void DownButtonClick()
    {
        _pageDownButton.InteractElement.onClick.Invoke();
    }
    public void RefreshButtonClick()
    {
        _refreshButton.InteractElement.onClick.Invoke();
    }
    public void ScreenSharinghButtonClick()
    {
        if (_screenSharingButton != null)
        {
            _screenSharingButton.InteractElement.onClick.Invoke();
        }

    }
    public void CloseButtonClick()
    {
        _exitButton.InteractElement.onClick.Invoke();
    }
}
