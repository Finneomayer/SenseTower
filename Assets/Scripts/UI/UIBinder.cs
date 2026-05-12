using System;
using Assets.Scripts.Client;
using Assets.Scripts.Player;
using Cysharp.Threading.Tasks;
using Data;
using Mechanics.SpaceStaticObjectEditing.UI;
using UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

public class UIBinder : MonoBehaviour
{
    #region Inspector

    public UISwitcherService Switcher;
    public MenuService Menu;
    [SerializeField] private bool HidePreviousPanel = true;
    [SerializeField] private InventoryViewPanel InventoryPanel;
    [SerializeField] private SpaceEditingPanel SpaceEditingPanel;
    [SerializeField] private ModalWindow _universalModalWindow;
    [SerializeField] private LookAtPlayer _universalModalWindowRotator;
    
    #endregion
    public ModalWindow UniversalModalWindow => _universalModalWindow;

    private bool _isVisible;
    private bool _uiBinderIsBlocked = false;
    private ViewPanel _switcherViewPanel;
    private LookAtPlayer[] _lookAtPlayerObjects;
    private IClientData _clientData;
    private bool _playerIsSet;

    [Inject]
    private void Construct(IClientData clientData)
    {
        _clientData = clientData;
    }

    void Awake()
    {
        _lookAtPlayerObjects = GetComponentsInChildren<LookAtPlayer>();

        _switcherViewPanel = Switcher.GetComponent<ViewPanel>();

        Menu.HideAllPanels();

        if (_universalModalWindow != null && _universalModalWindowRotator != null)
            _universalModalWindow.SetRotator(_universalModalWindowRotator);
    }

    private void Start()
    {
        if (_clientData.IsGuest || string.IsNullOrEmpty(_clientData.AccessToken))
        {
            Switcher.SetActiveButton(Enumenators.PanelType.LoginPanel);
            Menu.OpenPanel(Enumenators.PanelType.LoginPanel, HidePreviousPanel);
        }
    }

    private void OnEnable()
    {
        Switcher.PanelTypeClick += OpenPanel;
        Switcher.HidePreviousPanel(HidePreviousPanel);
    }

    private void OnDisable()
    {
        Switcher.PanelTypeClick -= OpenPanel;
    }

    public void SetPlayer(PlayerLogic player)
    {
        foreach (var lookAtPlayerObject in _lookAtPlayerObjects)
        {
            lookAtPlayerObject.SetPlayer(player.CameraTransform);
        }

        IXRInteractor[] interactors = new IXRInteractor[2];
        interactors[0] = player.GetLeftArm().RayInteractor;
        interactors[1] = player.GetRightArm().RayInteractor;
        InventoryPanel.Init(interactors, player.CameraTransform);

        if (SpaceEditingPanel != null)
        {
            SpaceEditingPanel.Init(interactors);
        }

        _playerIsSet = true;
    }

    public void OpenPanel(Enumenators.PanelType panelType)
    {
        if (!_uiBinderIsBlocked) Menu.OpenPanel(panelType, HidePreviousPanel);
    }

    public async UniTask SetUiVisibleForRegistrationAsync(bool isVisible)
    {
        await UniTask.WaitUntil(() => _playerIsSet);

        _switcherViewPanel.HidePanel();
        Switcher.SetButtonInactive();
        Menu.HideAllPanels();

        if (isVisible)
        {
            foreach (var lookAtPlayerObject in _lookAtPlayerObjects)
            {
                lookAtPlayerObject.SetFirstPosition();
                lookAtPlayerObject.SetPlayerFollow(true);
            }

            OpenPanel(Enumenators.PanelType.RegistrationPanel);
            _uiBinderIsBlocked = true;
        }
        else
        {
            _uiBinderIsBlocked = false;
            foreach (var lookAtPlayerObject in _lookAtPlayerObjects)
            {
                lookAtPlayerObject.SetPlayerFollow(false);
            }

            if (_isVisible)
            {
                _isVisible = !_isVisible;
                ToogleUIVisible();
            }
        }
    }

    public void ToogleUIVisible() //switched from PlayerLogic
    {
        _isVisible = !_isVisible;
        if (_uiBinderIsBlocked) return;

#if UNITY_STANDALONE_WIN
        ////moved to EditorMovementSystem.cs
        //Cursor.visible = _isVisible;
        //Cursor.lockState = _isVisible ? CursorLockMode.None : CursorLockMode.Confined; 
#endif

        if (_isVisible)
        {
            foreach (var lookAtPlayerObject in _lookAtPlayerObjects)
            {
                lookAtPlayerObject.SetFirstPosition();
                lookAtPlayerObject.SetPlayerFollow(true);
            }

            _switcherViewPanel.ShowPanel();
            Switcher.EmojiSwitcherObject.ShowEmojiButtons();
        }
        else
        {
            foreach (var lookAtPlayerObject in _lookAtPlayerObjects)
            {
                lookAtPlayerObject.SetPlayerFollow(false);
            }

            _switcherViewPanel.HidePanel();
            Switcher.SetButtonInactive();
            Menu.HideAllPanels();
        }
    }
}