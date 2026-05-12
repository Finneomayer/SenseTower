using Assets.Scripts.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Space;
using UnityEngine;
using UnityEngine.UI;
using static Data.Enumenators;
using Zenject;
using Assets.Localization;
using Client;

public class UISwitcherService : MonoBehaviour
{
    #region Inspector
    [SerializeField] private ModalWindow _modalWindow;
    public ButtonUI QuitButton;
    public SceneChangerView sceneChanger;
    public EmojiSwitcher EmojiSwitcherObject;
    [Space]
    [SerializeField] private List<ButtonBinder> _buttons = new List<ButtonBinder>();

    [Space]
    public ParticleSystem VFX;
    public AudioSource audioSource;

    [SerializeField] private LocalizationVariant _notAuthorizedMessageLocalizationVariant;
    [SerializeField] private LocalizationVariant GuestAccountQuitWarningLocalizationVariant;
    [SerializeField] private LocalizationVariant ConfirmQuitLocalizationVariant;
    [SerializeField] private LocalizationVariant CancelQuitLocalizationVariant;
    #endregion

    #region Public Variables

    public event Action<PanelType> PanelTypeClick;

    #endregion

    private bool _hidePreviousButton;

    #region InnerClass
    [Serializable]
    public class ButtonBinder
    {
        public ButtonUI button;
        public PanelType panelType;
    }
    #endregion

    #region Private Variables
    private ButtonUI currentActiveButton;

    private IClientData _clientData;
    private ISpaceManager _spaceManager;
    private ISpaceModeData _spaceModeData;

    #endregion

    #region UnityMethods

    [Inject]
    public void Construct(IClientData clientData, ISpaceManager spaceManager, ISpaceModeData spaceModeData)
    {
        _spaceModeData = spaceModeData;
        _clientData = clientData;
        _spaceManager = spaceManager;
    }

    void Start()
    {
        if (sceneChanger == null)
            sceneChanger = FindObjectOfType<SceneChangerView>();

        SubscribeToEvents();

        SetButtonsInteractable(!string.IsNullOrEmpty(_clientData.AccessToken));
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }
    #endregion

    #region PublicMethods

    public void SetActiveButton(PanelType panelType)
    {
        if(panelType == PanelType.Unknown)
        {
            SetButtonInactive();
            return;
        }

        for (int i = 0; i < _buttons.Count; i++)
        {
            var tempButton = _buttons[i];
            if (tempButton.panelType == panelType)
            {
                currentActiveButton = tempButton.button;
                ChangeBackgroundAlpha(tempButton.button.Background, true);
                break;
            }
        }
    }

    public void SetButtonInactive()
    {
        if (currentActiveButton != null)
            ChangeBackgroundAlpha(currentActiveButton.Background, false);
        currentActiveButton = null;
    }

    public void HidePreviousPanel(bool isVisiblePreviousPanel)
    {
        _hidePreviousButton = isVisiblePreviousPanel;
    }

    public void SetButtonsInteractable(bool isInteract)
    {
        foreach (var item in _buttons)
        {
            if (ReferenceEquals(item.button.PreviewImage, null)) continue;

            if (IsForAutenicatedUsersOnly(item.panelType))
                item.button.PreviewImage.color = isInteract ? Color.white : Color.gray;
            else
                item.button.PreviewImage.color = Color.white;
        }
    }
    #endregion

    #region PrivateMethods

    private bool IsForAutenicatedUsersOnly(PanelType panelType)
    {
        return (panelType != PanelType.LoginPanel && panelType != PanelType.SettingsPanel);
    }

    private void SubscribeToEvents()
    {
        foreach (var item in _buttons)
        {
            item.button.InteractElement.onClick.AddListener(delegate { OpenPanel(item); });
        }
        
        if(QuitButton != null)
            QuitButton.InteractElement.onClick.AddListener(OnQuitButtonClick);
    }

    private void UnsubscribeToEvents()
    {
        foreach (var item in _buttons)
        {
            item.button.InteractElement.onClick.RemoveAllListeners();
        }
        if(QuitButton != null)
            QuitButton.InteractElement.onClick.RemoveAllListeners();
    }
    
    private void OpenPanel(ButtonBinder sender)
    {
        //ShowVXF(sender.transform.position);
        PlayAudio();
		if(sender.panelType != PanelType.TipsPanel)
            EmojiSwitcherObject?.HideEmojiButtons();

        if (string.IsNullOrEmpty(_clientData.AccessToken) 
            && IsForAutenicatedUsersOnly(sender.panelType))
        {
            NotificationPanel.instance.SetInfo(_notAuthorizedMessageLocalizationVariant.Localize());
            NotificationPanel.instance.ShowPanel();
            return;
        }        
        PanelTypeClick?.Invoke(sender.panelType);


        if (currentActiveButton == sender.button)
        {
            if (_hidePreviousButton)
            {
                SetButtonInactive();
                EmojiSwitcherObject?.ShowEmojiButtons();
            }
            return;
        }

        ChangeBackgroundAlpha(sender.button.Background, true);
        SetButtonInactive();
        currentActiveButton = sender.button;
    }

    private void ChangeBackgroundAlpha(Image img, bool isVisible)
    {
        var tempColor = img.color;
        tempColor.a = isVisible ? 1f : 0f;
        img.color = tempColor;
    }

    private void ShowVXF(Vector3 position)
    {
        if (ReferenceEquals(VFX, null)) return;

        VFX.transform.position = position - Vector3.forward * 0.04f;
        VFX.Play();

        StartCoroutine(HideParticle());
    }

    private void PlayAudio()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    private bool IsEnterScene()
    {
        return _spaceManager.CurrentTransitionTarget == null
            || _spaceManager.CurrentTransitionTarget.SpaceType == SpaceType.EnterScene;
    }

    private void ExitApplication()
    {
        if (!IsEnterScene())
        {
            if (sceneChanger != null)
            {
                sceneChanger.ChangeSpace(SpaceType.EnterScene, sceneName: "Начальная сцена");
            }
            else
            {
                _spaceManager.ChangeSpace(SpaceType.EnterScene);
            }
            ClientDataInSpace clientDataInSpace = new();
            clientDataInSpace.Clear();

        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private async void OnQuitButtonClick()
    {
        if (!IsEnterScene() || _clientData == null || !_clientData.IsGuest || _modalWindow == null)
        {
            ExitApplication();
            return;
        }

        bool modalResult = await _modalWindow.Show(
            GuestAccountQuitWarningLocalizationVariant.Localize(), 
            ConfirmQuitLocalizationVariant.Localize(), 
            CancelQuitLocalizationVariant.Localize());
        if (modalResult)
        {
            ExitApplication();
        }
    }

    #endregion

    #region Coroutines
    private IEnumerator HideParticle()
    {
        yield return new WaitForSeconds(2f);
        VFX.Stop();
    }

    #endregion
}
