using Data;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HallMonitor : MonoBehaviour
{
    [SerializeField] private ViewPanel _zonePanel;
    [SerializeField] private UIBinder _uiBinder;
    [Space]
    [SerializeField] private Enumenators.PanelType _firstPanelType;
    [Space]
    [SerializeField] private XRSimpleInteractable _xrInteractable;
    [SerializeField] private InputActionReference _menuButton = null;
    private bool _isVisible = false;
   
    private void Start()
    {
        _xrInteractable.selectEntered.AddListener(Activate);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        _menuButton.action.started += MenuExecute;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        _menuButton.action.started -= MenuExecute;
#endif
        _xrInteractable.selectEntered.RemoveAllListeners();
    }
    
    private void MenuExecute(InputAction.CallbackContext inputAction)  
    {
        ToggleUI();
    }
    
    public void Activate(SelectEnterEventArgs args)
    {
        ToggleUI();
    }

    public void ToggleUI()
    {
        _isVisible = !_isVisible;

        if (_isVisible)
        {
            _zonePanel.ShowPanel();
            _uiBinder.Switcher.SetActiveButton(_firstPanelType);
            _uiBinder.OpenPanel(_firstPanelType);
        }
        else
        { 
            _uiBinder.OpenPanel(Enumenators.PanelType.Unknown);
            _uiBinder.Switcher.SetActiveButton(Enumenators.PanelType.Unknown);

            _zonePanel.HidePanel();
        }
    }
}
