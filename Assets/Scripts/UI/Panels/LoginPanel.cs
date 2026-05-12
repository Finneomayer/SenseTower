using Assets.Scripts.API;
using Assets.Scripts.Client;
using UI;
using UnityEngine;
using TMPro;
using Zenject;
using System;
using System.Collections.Generic;
using Assets.Mechanics.Keyboard.Scripts;
using Assets.Localization;
using Assets.Scripts;
using Assets.Scripts.API.RegistrationService;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;

public class LoginPanel : MainPanel
{
    #region Inspector
    [SerializeField] private ModalWindow _modalWindow;
    [SerializeField] private CompositionRootEnterScene _compositionRootEnterScene;
    public UISwitcherService switcher;
    public KeyboardScript Keyboard;
    [Space] public ViewPanel EntryPointPanel;
    [Space] public TMP_InputField LoginInputField;
    public TMP_InputField PasswordInputField;

    [Space] public ButtonUI AuthButton;
    public ButtonUI GuestAuthButton;
    public ButtonUI OpenRegistrationPanelButton;
    public List<ButtonUI> LogOutButtons;
    public List<TMP_Text> UserLoginTexts;

    [SerializeField] private LocalizationVariant AuthorizationFailedLocalizationVariant;
    [SerializeField] private LocalizationVariant GuestAccountLogoutWarningLocalizationVariant;
    [SerializeField] private LocalizationVariant ConfirmLogoutLocalizationVariant;
    [SerializeField] private LocalizationVariant CancelLogoutLocalizationVariant;
    #endregion

    #region Events

    public event Action UserLogOut;
    public event Action<string> UserLogInSuccess;

    #endregion

    #region PrivateVariables

    private IApiService _apiService;
    private IClientData _clientData;
    private IRegistrationService _registerService;
    private ISpaceService _spaceService;
    private IHallsService _hallService;

    #endregion

    [Inject]
    public void Construct(IClientData clientData, IApiService apiService, IRegistrationService registrationService,
        ISpaceService spaceService, ISpaceService spaceManager, IHallsService hallsService)
    {
        _hallService = hallsService;
        _spaceService = spaceService;
        _registerService = registrationService;
        _clientData = clientData;
        _apiService = apiService;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void Start()
    {
        Application.runInBackground = true; //need to prevent freezing on Android when select InputFields
        ShowContentPanel();
    }

    public void OnAuthResult(bool success)
    {
        if (success)
        {
            switcher.SetButtonsInteractable(true);
            UserLogInSuccess?.Invoke(_clientData.UserName);
        }
        else
        {
            NotificationPanel.instance.SetInfo(AuthorizationFailedLocalizationVariant.Localize());
            NotificationPanel.instance.ShowPanel();
        }

        ShowContentPanel();
    }

    private void SubscribeToEvents()
    {
        _clientData.DeleteAllAuthData += OnDeleteAllAuth;

        AuthButton.InteractElement.onClick.AddListener(delegate { Authorization(); });
        GuestAuthButton.InteractElement.onClick.AddListener(delegate { GuestAuthorization(); });
        foreach (ButtonUI outButton in LogOutButtons)
        {
            outButton.InteractElement.onClick.AddListener(OnLogoutButtonClick);
        }

        LoginInputField.onSelect.AddListener(delegate { ShowKeyboard(LoginInputField); });

        PasswordInputField.onSelect.AddListener(delegate { ShowKeyboard(PasswordInputField); });
    }

    private void ShowKeyboard(TMP_InputField inputField)
    {
        Keyboard.OpenKeyboard(inputField);
    }

    public override void ShowPanel()
    {
        ShowContentPanel();
        base.ShowPanel();
    }

    private void DisableKeyboard()
    {
        Keyboard.CloseKeyboard();
    }

    private void UnsubscribeToEvents()
    {
        _clientData.DeleteAllAuthData -= OnDeleteAllAuth;

        AuthButton.InteractElement.onClick.RemoveAllListeners();
        foreach (ButtonUI outButton in LogOutButtons)
        {
            outButton.InteractElement.onClick.RemoveListener(OnLogoutButtonClick);
        }

        GuestAuthButton.InteractElement.onClick.RemoveListener(delegate { GuestAuthorization(); });

        LoginInputField.onSelect.RemoveAllListeners();

        PasswordInputField.onSelect.RemoveAllListeners();
    }


    private void ShowContentPanel()
    {
        ShowViewPanel(EntryPointPanel);
        if (!string.IsNullOrEmpty(_clientData.AccessToken))
        {
            foreach (TMP_Text loginText in UserLoginTexts)
            {
                loginText.text = _clientData.UserName;
            }

            if (!_clientData.IsGuest)
                OpenRegistrationPanelButton.SetButtonInteractable(false);
        }
        else
        {
            OpenRegistrationPanelButton.SetButtonInteractable(true);
        }
    }

    private void GuestAuthorization()
    {
        Authorization(true);
    }

    private async void Authorization(bool isGuest = false)
    {
        NotificationPanel.instance.SetDefaultInfo();
        NotificationPanel.instance.ShowPanel();

        DisableKeyboard();

        string login = LoginInputField.text;
        string password = PasswordInputField.text;

        var form = new WWWForm();
        form.AddField("login", login);
        form.AddField("password", password);

        bool success;
        if (isGuest)
        {
            success = await _registerService.RegisterAsGuest(SystemInfo.deviceUniqueIdentifier);
            if (success)
                ChangeSpaceAfterInitForGuest();
        }
        else
        {
            success = await _apiService.Auth(form);
        }

        OnAuthResult(success);
    }

    private void LogOut()
    {
        _clientData.DeleteAllData();
        OnDeleteAllAuth();
    }

    private async void OnLogoutButtonClick()
    {
        if (_clientData == null || !_clientData.IsGuest || _modalWindow == null)
        {
            LogOut();
            return;
        }

        bool modalResult = await _modalWindow.Show(
            GuestAccountLogoutWarningLocalizationVariant.Localize(),
            ConfirmLogoutLocalizationVariant.Localize(),
            CancelLogoutLocalizationVariant.Localize());
        if (modalResult)
        {
            LogOut();
        }
    }

    private void OnDeleteAllAuth()
    {
        LoginInputField.text = "";
        PasswordInputField.text = "";
        ShowContentPanel();
        switcher.SetButtonsInteractable(false);
        UserLogOut?.Invoke();
    }

    private async void ChangeSpaceAfterInitForGuest()
    {
        await UniTask.WaitUntil(() => _spaceService.GetAllSpaces().Length > 2);

        var halls = await _hallService.GetHalls();
        foreach (LocalSpace space in halls[0].Spaces)
        {
            if (space.SpaceType == SpaceType.HallScene)
            {
                _compositionRootEnterScene.LoadingSceneChangerView.LoadScene(space.SpaceType, space.Id.ToString());
                break;
            }
        }
    }
}