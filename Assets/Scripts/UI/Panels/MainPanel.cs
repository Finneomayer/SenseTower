using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.UI;

public class MainPanel : ViewPanel
{
    #region InnerClass
    [System.Serializable]
    public class InteractElement
    {
        public ButtonUI button;
        public ViewPanel viewPanel;
    }
    #endregion

    #region PrivateVariables
    [SerializeField] private List<InteractElement> _interactElements = new List<InteractElement>();

    public event Action PanelRequest;
     private ViewPanel _activeViewPanel;
     private ButtonUI _activeButton;
     #endregion

    #region UnityMethods
    private void Awake()
    {
        HidePanel();
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeEvents();
    }
    #endregion

    #region PublicMethods
    public override void ShowPanel()
    {
        base.ShowPanel();

        if (_activeButton != null && _activeViewPanel != null && _activeButton.IsInteractable)
        {
            ShowViewPanel(_activeViewPanel, _activeButton);
        }
        else
        {
            foreach (var item in _interactElements)
            {
                if (item.button.IsInteractable)
                {
                    ShowViewPanel(item.viewPanel, item.button);
                    break;
                }
            }
        }
    }

    public override void HidePanel()
    {
        HideCurrentPanel();

        base.HidePanel();
    }
    #endregion

    #region PrivateMethods
    private void SubscribeEvents()
    {
        for (int i = 0; i < _interactElements.Count; i++)
        {
            var element = _interactElements[i];
            element.button.InteractElement.onClick.AddListener(delegate
            {
                ShowViewPanel(element.viewPanel,element.button);
                PanelRequest?.Invoke();
            });
        }
    }

    private void UnSubscribeEvents()
    {
        for (int i = 0; i < _interactElements.Count; i++)
        {
            _interactElements[i].button.InteractElement.onClick.RemoveAllListeners();
        }
    }

    private void ShowViewPanel(ViewPanel viewPanel, ButtonUI buttonUI) 
    {
        if (!ReferenceEquals(_activeViewPanel, null))
            _activeViewPanel.HidePanel();

        if (!ReferenceEquals(_activeButton, null))
            _activeButton.SetButtonActive(false);

        viewPanel.ShowPanel();
        buttonUI.SetButtonActive(true);
        
        _activeButton = buttonUI;
        _activeViewPanel = viewPanel;
    }

    protected void ShowViewPanel(ViewPanel viewPanel)
    {
        if (!ReferenceEquals(_activeViewPanel, null))
            _activeViewPanel.HidePanel();

        if (!ReferenceEquals(_activeButton, null))
            _activeButton.SetButtonActive(false);

        foreach (var item in _interactElements)
        {
            if (item.viewPanel != null && item.viewPanel == viewPanel)
            {
                item.viewPanel.ShowPanel();
                if (item.button != null)
                {
                    item.button.SetButtonActive(true);
                }
                _activeButton = item.button;
                _activeViewPanel = item.viewPanel;
            }
        }
    }

    private void HideCurrentPanel()
    {
        if (!ReferenceEquals(_activeViewPanel, null))
            _activeViewPanel.HidePanel();

        if (!ReferenceEquals(_activeButton, null))
            _activeButton.SetButtonActive(false);

        _activeButton = null;
        _activeViewPanel = null;
    }
    #endregion
}
