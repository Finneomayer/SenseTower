using Assets.Mechanics.Browser;
using Assets.Mechanics.Keyboard.Scripts;
using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PadButtons : MonoBehaviour
{
    [Header("Main browser buttons")]
    [SerializeField] private BrowserAdminControlPanel _mainPanel;
    //[SerializeField] private WebPageActionControl _maximizeButton;
    //[SerializeField] private WebPageActionControl _minimizeButton;
    [Header("Pad browser buttons")]
    [SerializeField] private ButtonUI _searchButton;
    [SerializeField] private ButtonUI _downButton;
    [SerializeField] private ButtonUI _refreshButton;
    [SerializeField] private ButtonUI _screenSharingButton;
    [SerializeField] private ButtonUI _expandButton;
    [SerializeField] private Image _maximize;
    [SerializeField] private Image _minimize;
    [SerializeField] private ButtonUI _closeButton;
    [SerializeField] private ButtonUI _disconnectFromBrowserButton;

    public event Action<bool> SearchOpenRequest;
    public event Action CloseButtonClicked;
    public event Action DisconnectButtonClicked;
    public event Action<WebPageAction> WebPageActionRequested;

    private PadSwitcher _padSwitcher;

    private void Awake()
    {
        if (_mainPanel == null)
        {
            _mainPanel = FindObjectOfType<BrowserAdminControlPanel>();
        }
        _padSwitcher = GetComponent<PadSwitcher>();
    }

    private void OnDisable()
    {
        UnsubscribeButtons();
    }

    public void InitButtons()
    {
        _searchButton.InteractElement.onClick.AddListener(OnSearchButtonClicked);
        _downButton.InteractElement.onClick.AddListener(OnBackButtonClick);
        _refreshButton.InteractElement.onClick.AddListener(OnRefreshButtonClick);
        _screenSharingButton.InteractElement.onClick.AddListener(OnScreenSharinghButtonClick);
        //_expandButton.onClick.AddListener(OnExpandButtonClicked);
        _closeButton.InteractElement.onClick.AddListener(OnCloseButtonClick);
        _disconnectFromBrowserButton.InteractElement.onClick.AddListener(OnDisconnectButtonClick);

        _searchButton.SetButtonActive(_padSwitcher != null && _padSwitcher.SearchFieldIsVisible);

        _mainPanel.SetBrowserFocused();
    }    

    public void UnsubscribeButtons()
    {
        _searchButton.InteractElement.onClick.RemoveListener(OnSearchButtonClicked);
        _downButton.InteractElement.onClick.RemoveListener(OnBackButtonClick);
        _refreshButton.InteractElement.onClick.RemoveListener(OnRefreshButtonClick);
        _screenSharingButton.InteractElement.onClick.RemoveListener(OnScreenSharinghButtonClick);
        //_expandButton.onClick.AddListener(OnExpandButtonClicked);
        _closeButton.InteractElement.onClick.RemoveListener(OnCloseButtonClick);
        _disconnectFromBrowserButton.InteractElement.onClick.RemoveListener(OnDisconnectButtonClick);
    }

    private void OnSearchButtonClicked()
    {
        SearchOpenRequest?.Invoke(!_padSwitcher.SearchFieldIsVisible);
        _padSwitcher.SearchFieldIsVisible = !_padSwitcher.SearchFieldIsVisible;
        _searchButton.SetButtonActive(_padSwitcher.SearchFieldIsVisible);
    }

    private void OnBackButtonClick()
    {
        _mainPanel.DownButtonClick();
    }

    private void OnRefreshButtonClick()
    {
        _mainPanel.RefreshButtonClick();
    }

    private void OnScreenSharinghButtonClick()
    {
        _mainPanel.ScreenSharinghButtonClick();
    }

    private void OnCloseButtonClick()
    {
        CloseButtonClicked?.Invoke();
        if (_padSwitcher != null) _padSwitcher.Hide();
    }

    private void OnDisconnectButtonClick()
    {
        _mainPanel.CloseButtonClick();
        DisconnectButtonClicked?.Invoke();
    }

    private void OnExpandButtonClicked()
    {
        if (_maximize.enabled)
        {
            WebPageActionRequested?.Invoke(WebPageAction.Maximize);
            //_maximizeButton.ClickRemote();
            _maximize.enabled = false;
            _minimize.enabled = true;
        }
        else if (_minimize.enabled)
        {
            WebPageActionRequested?.Invoke(WebPageAction.Minimize);
            //_minimizeButton.ClickRemote();
            _maximize.enabled = true;
            _minimize.enabled = false;
        }        
    }     
}
