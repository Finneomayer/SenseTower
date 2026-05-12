using System;
using System.Collections;
using API.Models.Registration;
using Assets.Localization;
using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.API.RegistrationService;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Assets.Scripts.Transactions;
using Cysharp.Threading.Tasks;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public enum GuestState
    {
        Resident = 0,
        GuestNotInformed = 1,
        GuestInformed = 2,
        GuestIsResident = 3
    }
    public class RegistrationPlayerUi : MonoBehaviour
    {
        [SerializeField] private UIBinder _uiBinder;
        [SerializeField] private KeyboardScript _keyboard;
        [SerializeField] private ModalWindow _modalWindow;
        [Space] 
        [SerializeField] private Button _registrationInSwitcherMenuBtn;
        [SerializeField] private Button _registrationBtn;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _closeSuccessBtn;

        [SerializeField] private GameObject _notificationText;
        [SerializeField] private GameObject _inputFields;
        [SerializeField] private GameObject _successText;

        [SerializeField] private TMP_InputField _loginInput;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TMP_InputField _confirmPasswordInput;
        [Space]
        [SerializeField] private LocalizationVariant _fillAllFieldsLocalizationVariant;
        [SerializeField] private LocalizationVariant _passwordsMissmatchLocalizationVariant;
        [SerializeField] private LocalizationVariant _sceneNeedsToRestartLocalizationVariant;
        [SerializeField] private LocalizationVariant _authErrorLocalizationVariant;
        [SerializeField] private LocalizationVariant _endMainingTWRLocalizationVariant;
        [SerializeField] private LocalizationVariant _registrationCongratulationLocalizationVariant;
        [SerializeField] private LocalizationVariant _currentEarnedLocalizationVariant;


        public static GuestState CurrentGuestState;
        private IRegistrationService _registrationService;
        private IClientData _clientData;
        private ISpaceManager _spaceManager;
        private IApiService _apiService;
        private ITransactionsService _transactionService;

        private Coroutine _startCheckingCoroutine;
        private bool _disableShowing = false;
        private decimal _currentTwrAmount;
        private decimal _editorTwrAmount;

        [Inject]
        private void Construct(IRegistrationService registrationService, IClientData clientData, 
            ISpaceManager spaceManager, IApiService apiService,ITransactionsService transactionsService)
        {
            _transactionService = transactionsService;
            _registrationService = registrationService;
            _clientData = clientData;
            _spaceManager = spaceManager;
            _apiService = apiService;
        }

        private void OnEnable()
        {
            if (_clientData.IsGuest)
            {
                _closeBtn.onClick.AddListener(RegistrationPanelClose);
                _registrationBtn.onClick.AddListener(OnRegistrationButtonPressedAsync);
                _closeSuccessBtn.onClick.AddListener(RegistrationPanelClose);

                _registrationInSwitcherMenuBtn.gameObject.SetActive(true);
                _registrationInSwitcherMenuBtn.onClick.AddListener(RegistrationPanelOpen);

#if UNITY_ANDROID
                if (_startCheckingCoroutine != null)
                {
                    StopCoroutine(_startCheckingCoroutine);
                    _startCheckingCoroutine = null;
                }
                _startCheckingCoroutine = StartCoroutine(StarCheckingTWRCoroutine());
#endif
                _loginInput.onSelect.AddListener(delegate { ShowKeyboard(_loginInput); });
                _emailInput.onSelect.AddListener(delegate { ShowKeyboard(_emailInput); });
                _passwordInput.onSelect.AddListener(delegate { ShowKeyboard(_passwordInput); });
                _confirmPasswordInput.onSelect.AddListener(delegate { ShowKeyboard(_confirmPasswordInput); });

            }
            else
            {
                _registrationInSwitcherMenuBtn.gameObject.SetActive(false);
                if (CurrentGuestState == GuestState.GuestIsResident)
                {
                    RegistrationPanelOpenForSuccessNotification();
                    _closeBtn.onClick.AddListener(RegistrationPanelClose);
                    _closeSuccessBtn.gameObject.SetActive(true);
                    _closeSuccessBtn.onClick.AddListener(RegistrationPanelClose);
                    _registrationBtn.gameObject.SetActive(false);
                    CurrentGuestState = GuestState.Resident;
                }
            }
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(RegistrationPanelClose);
            _registrationInSwitcherMenuBtn.onClick.RemoveListener(RegistrationPanelOpen);
#if UNITY_ANDROID
            if (_startCheckingCoroutine != null)
            {
                StopCoroutine(_startCheckingCoroutine);
                _startCheckingCoroutine = null;
            }
#endif
            _registrationBtn.onClick.RemoveListener(OnRegistrationButtonPressedAsync);
            _closeSuccessBtn.onClick.RemoveListener(RegistrationPanelClose);
            _loginInput.onSelect.RemoveAllListeners();
            _emailInput.onSelect.RemoveAllListeners();
            _passwordInput.onSelect.RemoveAllListeners();
            _confirmPasswordInput.onSelect.RemoveAllListeners();
        }

        [ContextMenu("10TWR")]
        private void Set10twrs()
        {
            _editorTwrAmount = 10;
        }

        [ContextMenu("50TWR")]
        private void Set50twrs()
        {
            _editorTwrAmount = 50;
        }

        private async void StartCheckingEarnedTwr()
        {
            Enumenators.TransactionPurposeTypeDto[] filter = 
                new Enumenators.TransactionPurposeTypeDto[] { Enumenators.TransactionPurposeTypeDto.TimeMining };
            TransactionDto[] result = await _transactionService.GetTransactionsByType(filter, 10,0);

            _currentTwrAmount = 0;

            foreach (TransactionDto transactionDto in result)
            {
                if (transactionDto.Amount != null) _currentTwrAmount += transactionDto.Amount.Value;
            }
            
            if (_editorTwrAmount + _currentTwrAmount >= 10m 
                && _editorTwrAmount + _currentTwrAmount < 11m 
                && CurrentGuestState != GuestState.GuestInformed)
            {
                CurrentGuestState = GuestState.GuestInformed;
                RegistrationPanelOpen();
            }

            if (_editorTwrAmount + _currentTwrAmount >= 11m)
            {
                if (_notificationText.TryGetComponent(out TMP_Text notificationText))
                {
                    notificationText.text = _currentEarnedLocalizationVariant.Localize().Replace($"{{0}}", _currentTwrAmount.ToString());
                }
            }

            if (_editorTwrAmount + _currentTwrAmount >= 50m)
            {
                if(_notificationText.TryGetComponent(out TMP_Text notificationText))
                {
                    notificationText.text = _endMainingTWRLocalizationVariant.Localize();
                }

                CurrentGuestState = GuestState.GuestInformed;
                RegistrationPanelOpen();
                StopCoroutine(_startCheckingCoroutine);
            }
        }

        private void ShowKeyboard(TMP_InputField inputField)
        {
            _keyboard.OpenKeyboard(inputField);
        }

        private async void OnRegistrationButtonPressedAsync()
        {
            if (_inputFields.activeInHierarchy)
            {
                if (!String.IsNullOrEmpty(_loginInput.text) &&
                    !String.IsNullOrEmpty(_emailInput.text) &&
                    !String.IsNullOrEmpty(_passwordInput.text) &&
                    !String.IsNullOrEmpty(_confirmPasswordInput.text))
                {
                    if (_passwordInput.text.Equals(_confirmPasswordInput.text))
                    {
                        RegisterResult result = await _registrationService.MakeGuestResident(_loginInput.text, _passwordInput.text, _emailInput.text);

                        if (!result.success)
                        {
                            if (result.ValidationError.Login != null) await _modalWindow.Show($"{result.ValidationError.Login}", "Ok");
                            if (result.ValidationError.Email != null) await _modalWindow.Show($"{result.ValidationError.Email}", "Ok");
                            if (result.ValidationError.Password != null) await _modalWindow.Show($"{result.ValidationError.Password}", "Ok");
                        }
                        else
                        {
                            CurrentGuestState = GuestState.GuestIsResident;

                            var form = new WWWForm();
                            form.AddField("login", _loginInput.text);
                            form.AddField("password", _passwordInput.text);

                            bool resultAuth = await _apiService.Auth(form);

                            if (resultAuth)
                            {
                                await _modalWindow.Show(_sceneNeedsToRestartLocalizationVariant.Localize(), "Ok");

                                var sceneChanger = FindObjectOfType<SceneChangerView>();
                                if (sceneChanger != null)
                                {
                                    sceneChanger.ReloadCurrentSpace(keepPlayerPosition: true);
                                }
                            }
                            else
                            {
                                await _modalWindow.Show(_authErrorLocalizationVariant.Localize(), "Ok");
                            }
                        }
                    }
                    else
                    {
                        bool modalResult = await _modalWindow.Show(_passwordsMissmatchLocalizationVariant.Localize(), "Ok");
                    }
                }
                else
                {
                    bool modalResult = await _modalWindow.Show(_fillAllFieldsLocalizationVariant.Localize(), "Ok");
                }
            }
            else
            {
                ShowInputFields();
            }
        }

        private async  void RegistrationPanelOpen()
        {
            if (_inputFields.activeInHierarchy) return;

            await _uiBinder.SetUiVisibleForRegistrationAsync(true);

            if (CurrentGuestState == GuestState.GuestInformed)
            {
                ShowInfoNotification();
            }
            else
            {
                ShowInputFields();
            }
        }

        public async void RegistrationPanelOpenForSuccessNotification()
        {
            await _uiBinder.SetUiVisibleForRegistrationAsync(true);
            ShowSuccessText();
        }

        private async void RegistrationPanelClose()
        {
            Debug.LogWarning("!! Close panel");
            await _uiBinder.SetUiVisibleForRegistrationAsync(false);
            CloseAllWindows();
        }

        private void ShowInputFields()
        {
            _notificationText.SetActive(false);
            _successText.SetActive(false);
            _inputFields.SetActive(true);
            _passwordInput.text = "";
            _confirmPasswordInput.text = "";
            ShowKeyboard(_loginInput);
        }

        private void ShowInfoNotification()
        {
            _notificationText.SetActive(true);
            _inputFields.SetActive(false);
            _successText.SetActive(false);
        }

        private void ShowSuccessText()
        {
            _successText.SetActive(true);
            _notificationText.SetActive(false);
            _inputFields.SetActive(false);
        }

        private void CloseAllWindows()
        {
            _successText.SetActive(false);
            _notificationText.SetActive(false);
            _inputFields.SetActive(false);
        }

        private IEnumerator StarCheckingTWRCoroutine()
        {
            while (true)
            {
                StartCheckingEarnedTwr();
                yield return new WaitForSeconds(30f);
            }
        }
    }
}
