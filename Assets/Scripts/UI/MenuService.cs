using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static Data.Enumenators;

public class MenuService : MonoBehaviour
{
    #region Inspector
    [SerializeField] private KeyboardScript _keyboard;
    [SerializeField] private Image _maskingObject; //now uses to prevent browser raycasting through menu
    public List<ViewPanelBinder> viewPanels = new List<ViewPanelBinder>();
    #endregion

    protected PanelType currentPanelType = PanelType.Unknown;

    private void Awake()
    {
        foreach (var panel in viewPanels)
        {
            panel.ViewPanel.Init(_keyboard);
        }
    }

    public void HideAllPanels() 
    {
        for (int i = 0; i < viewPanels.Count; i++)
        {
            viewPanels[i].ViewPanel.HidePanel();
        }
        HidePreviousPanel();

        if (_maskingObject != null) _maskingObject.enabled = false;
    }

    public virtual void OpenPanel(PanelType panelType, bool hidePreviousPanel = true)
    {
        if(currentPanelType == panelType && !hidePreviousPanel)
        {
            return;
        }

        if (currentPanelType == panelType && hidePreviousPanel)
        {
            HidePreviousPanel();
            return;
        }

        if (currentPanelType != PanelType.Unknown)
            HidePreviousPanel();

        for (int i = 0; i < viewPanels.Count; i++)
        {
            if (viewPanels[i].PanelType == panelType) 
            {
                viewPanels[i].ViewPanel.ShowPanel();
                break;
            }
        }

        currentPanelType = panelType;
        if (_maskingObject != null) _maskingObject.enabled = true;
    }

    private void HidePreviousPanel()
    {
        for (int i = 0; i < viewPanels.Count; i++)
        {
            if (viewPanels[i].PanelType == currentPanelType)
            {
                viewPanels[i].ViewPanel.HidePanel();
                break;
            }
        }

        currentPanelType = PanelType.Unknown;
        if (_maskingObject != null) _maskingObject.enabled = false;
    }

    #region InnerClass
    
    [Serializable]
    public struct ViewPanelBinder
    {
        public ViewPanel ViewPanel;
        public PanelType PanelType;
    }
    
    #endregion
}
