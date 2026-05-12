using System;
using System.Text.RegularExpressions;
using API.Models.Registration;
using Assets.Localization;
using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.API.RegistrationService;
using Assets.Scripts.Client;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Panels
{
    public class RegisterPanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private LoginPanel _loginPanel;

        [Space] [SerializeField] private TMP_InputField _loginInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_InputField _requestPasswordInputField;
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_Text _errorText;

        [SerializeField] private KeyboardScript _keyboard;

        [SerializeField] private ButtonUI _registerButton;
        [SerializeField] private LocalizationVariant _emptyInputField;
        [SerializeField] private LocalizationVariant _passwordNotEquals;

        #endregion

        private IRegistrationService _registrationService;
        private IApiService _apiService;
        private IClientData _clientData;

        private Regex
            _emailRegex = new Regex(@"^[-\w.]+@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,4}$");

        [Inject]
        private void Construct(IClientData clientData, IRegistrationService registrationService, IApiService apiService)
        {
            _clientData = clientData;
            _apiService = apiService;
            _registrationService = registrationService;
        }

        private void OnEnable()
        {
            _registerButton.InteractElement.onClick.AddListener(TryRegistration);

            _loginInputField.onSelect.AddListener(delegate { ShowKeyboard(_loginInputField); });
            _requestPasswordInputField.onSelect.AddListener(delegate { ShowKeyboard(_requestPasswordInputField); });
            _passwordInputField.onSelect.AddListener(delegate { ShowKeyboard(_passwordInputField); });
            _emailInputField.onSelect.AddListener(delegate { ShowKeyboard(_emailInputField); });
        }

        private void OnDisable()
        {
            _registerButton.InteractElement.onClick.RemoveListener(TryRegistration);
            _requestPasswordInputField.onSelect.RemoveListener(delegate { ShowKeyboard(_requestPasswordInputField); });
            _loginInputField.onSelect.RemoveListener(delegate { ShowKeyboard(_loginInputField); });
            _passwordInputField.onSelect.RemoveListener(delegate { ShowKeyboard(_passwordInputField); });
            _emailInputField.onSelect.RemoveListener(delegate { ShowKeyboard(_emailInputField); });
        }

        private void ShowKeyboard(TMP_InputField inputField)
        {
            _keyboard.OpenKeyboard(inputField);
        }

        private void DisableKeyboard()
        {
            _keyboard.CloseKeyboard();
        }

        private async void TryRegistration()
        {
            float defaultTimer = NotificationPanel.instance.timer;
            NotificationPanel.instance.timer = 5f;
            DisableKeyboard();
            if (!ValidateInputField(out string errorText))
            {
                NotificationPanel.instance.SetInfo(errorText);
                NotificationPanel.instance.ShowPanel();
                NotificationPanel.instance.timer = defaultTimer;
                return;
            }

            string login = _loginInputField.text;
            string password = _passwordInputField.text;
            string email = _emailInputField.text;

            NotificationPanel.instance.SetDefaultInfo();
            NotificationPanel.instance.ShowPanel();
            RegisterResult registerResult = null;

            if (_clientData.IsGuest)
                registerResult = await _registrationService.MakeGuestResident(login, password, email);
            else if (string.IsNullOrEmpty(_clientData.AccessToken))
                registerResult = await _registrationService.Register(login, password, email);

            if (registerResult != null && registerResult.success)
            {
                NotificationPanel.instance.SetDefaultInfo();
                NotificationPanel.instance.ShowPanel();

                var form = new WWWForm();
                form.AddField("login", login);
                form.AddField("password", password);

                bool result = await _apiService.Auth(form);

                _loginInputField.text = string.Empty;
                _emailInputField.text = string.Empty;
                _passwordInputField.text = string.Empty;
                _requestPasswordInputField.text = string.Empty;

                _loginPanel.OnAuthResult(result);
            }
            else
            {
                if (registerResult != null)
                    NotificationPanel.instance.SetInfo(GetErrorText(registerResult));
                NotificationPanel.instance.ShowPanel();
            }

            NotificationPanel.instance.timer = defaultTimer;
        }

        private bool ValidateInputField(out string errorText)
        {
            if (string.IsNullOrEmpty(_loginInputField.text)
                || string.IsNullOrEmpty(_passwordInputField.text)
                || string.IsNullOrEmpty(_emailInputField.text))
            {
                errorText = _emptyInputField.Localize();
                return false;
            }

            if (!_requestPasswordInputField.text.Equals(_passwordInputField.text))
            {
                errorText = _passwordNotEquals.Localize();
                return false;
            }

            if (!_emailRegex.IsMatch(_emailInputField.text))
            {
                errorText = "Email не корректный";
                return false;
            }

            if (Regex.Match(_emailInputField.text, "[А-Яа-яЁё]").Success)
            {
                errorText = "Email не может содержать кириллицу";
                return false;
            }

            errorText = string.Empty;
            return true;
        }

        private string GetErrorText(RegisterResult registerResult)
        {
            string result = string.Empty;

            if (registerResult.ValidationError == null)
                return result;

            if (registerResult.success)
                return result;

            if (!string.IsNullOrEmpty(registerResult.ValidationError.Email))
                result = registerResult.ValidationError.Email;

            if (!string.IsNullOrEmpty(registerResult.ValidationError.Password))
                result = registerResult.ValidationError.Password;

            if (!string.IsNullOrEmpty(registerResult.ValidationError.Login))
                result = registerResult.ValidationError.Login;

            return result;
        }
    }
}