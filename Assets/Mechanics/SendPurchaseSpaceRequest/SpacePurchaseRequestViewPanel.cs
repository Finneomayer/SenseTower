using System;
using Assets.Localization;
using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.Space;
using Mechanics.SendPurchaseSpaceRequest.Models;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mechanics.SendPurchaseSpaceRequest
{
    public class SpacePurchaseRequestViewPanel : ViewPanel
    {
        #region Inspect

        [SerializeField] private bool _isIndependent = false;
        [SerializeField] private ViewPanel _sendBuyRequestPanel;
        [SerializeField] private ModalWindow _modalWindow;
        [SerializeField] private KeyboardScript _keyboard;
        [Space]
        [Header("Buttons")]
        //[SerializeField] private Button _openSendBuyRequestPanelButton;
        [SerializeField] private HandButton _handButton;
        [SerializeField] private Button _sendRequestButton;
        [SerializeField] private Button _openSendBuyRequestPanelButton;
        [SerializeField] private Button _exitButton;

        [Space]
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField _loginInput;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _numberInput;
        [SerializeField] private TMP_InputField _commentsInput;
        
        [SerializeField] private LocalizationVariant _closeButtonText;
        [SerializeField] private LocalizationVariant _successText;
        [SerializeField] private LocalizationVariant _failNameValidation;
        [SerializeField] private LocalizationVariant _emptyNameError;
        [SerializeField] private LocalizationVariant _emptyEmailError;
        [SerializeField] private LocalizationVariant _failPhoneNumberError;
        [SerializeField] private LocalizationVariant _failText;
        #endregion
        
        private ISpacePurchaseService _spacePurchaseService;
        private ISpaceManager _spaceManager;
        private LocalSpace _localSpace;

        [Inject]
        private void Construct(ISpacePurchaseService spacePurchaseService, ISpaceManager spaceManager)
        {
            _spaceManager = spaceManager;
            _spacePurchaseService = spacePurchaseService;
        }
        
        private void OnEnable()
        {
            _loginInput.onSelect.AddListener(delegate { ShowKeyboard(_loginInput); });
            _emailInput.onSelect.AddListener(delegate { ShowKeyboard(_emailInput); });
            _numberInput.onSelect.AddListener(delegate { ShowKeyboard(_numberInput, true); });
            _commentsInput.onSelect.AddListener(delegate { ShowKeyboard(_commentsInput); });
            _handButton.OnPressPhysicsButton += OpenSendBuyRequestPanel;
            _sendRequestButton.onClick.AddListener(OnSendButtonClick);
            _openSendBuyRequestPanelButton.onClick.AddListener(OpenSendBuyRequestPanel);
            _exitButton.onClick.AddListener(OnExitButtonClick);
            _handButton.gameObject.SetActive(_isIndependent);
        }

        private void OnDisable()
        {
            _loginInput.onSelect.RemoveListener(delegate { ShowKeyboard(_loginInput); });
            _emailInput.onSelect.RemoveListener(delegate { ShowKeyboard(_emailInput); });
            _numberInput.onSelect.RemoveListener(delegate { ShowKeyboard(_numberInput, true); });
            _commentsInput.onSelect.RemoveListener(delegate { ShowKeyboard(_commentsInput); });
            
            _sendRequestButton.onClick.RemoveListener(OnSendButtonClick);
            _handButton.OnPressPhysicsButton -= OpenSendBuyRequestPanel;

            _openSendBuyRequestPanelButton.onClick.RemoveListener(OpenSendBuyRequestPanel);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void Start()
        {
            if (_isIndependent)
            {
                SetLocalSpace(_spaceManager.CurrentTransitionTarget);
                ShowPanel();
            }
        }

        public override void ShowPanel()
        {
            if (IsVisible())
                return;
            
            base.ShowPanel();
            _sendBuyRequestPanel.HidePanel();
            _handButton.gameObject.SetActive(true);
            _openSendBuyRequestPanelButton.gameObject.SetActive(true);
        }

        public override void HidePanel()
        {
            _handButton.gameObject.SetActive(_isIndependent);
            _openSendBuyRequestPanelButton.gameObject.SetActive(_isIndependent);
            base.HidePanel();
        }

        public void SetLocalSpace(LocalSpace localSpace)
        {
            _localSpace = localSpace;
        }

        private async void OnSendButtonClick()
        {
            if (GetValidationError(out string validateError))
            {
                HidePanel();
                await _modalWindow.Show(validateError, _closeButtonText.Localize());
                ShowPanel();
            }
            else
            {
                HidePanel();
                SendPurchaseResult request = await _spacePurchaseService.SendPurchaseRequest(FillPurchaseRequest());
                if (request == null)
                {
                    await _modalWindow.Show(_failText.Localize(), _closeButtonText.Localize());
                    ShowPanel();
                    _sendBuyRequestPanel.HidePanel();
                    _handButton.gameObject.SetActive(true);
                    _openSendBuyRequestPanelButton.gameObject.SetActive(true);

                }
                else
                {
                    if (request.success)
                    {
                        await _modalWindow.Show(_successText.Localize(), _closeButtonText.Localize());
                    }
                    else
                    {
                        if (request.ValidationError.Name != null) await _modalWindow.Show($"{request.ValidationError.Name}", _closeButtonText.Localize());
                        if (request.ValidationError.Email != null) await _modalWindow.Show($"{request.ValidationError.Email}", _closeButtonText.Localize());
                        if (request.ValidationError.PhoneNumber != null) await _modalWindow.Show($"{request.ValidationError.PhoneNumber}", _closeButtonText.Localize());
                        if (request.ValidationError.Comment != null) await _modalWindow.Show($"{request.ValidationError.Comment}", _closeButtonText.Localize());
                        if (request.ValidationError.Space != null) await _modalWindow.Show($"{request.ValidationError.Space}", _closeButtonText.Localize());
                        ShowPanel();
                        _sendBuyRequestPanel.ShowPanel();
                        _handButton.gameObject.SetActive(_isIndependent);
                        _openSendBuyRequestPanelButton.gameObject.SetActive(_isIndependent);

                    }
                }
            }
        }

        private void ShowKeyboard(TMP_InputField inputField, bool isNumericInputField = false)
        {
            if (_keyboard.IsOpened() && inputField == _keyboard.AttachedInputField)
            {
                return;
            }

            _keyboard.OpenKeyboard(inputField, isNumericInputField);
            _keyboard.SetMode(isNumericInputField ? KeyboardMode.Digits : KeyboardMode.EngLowerCase);
        }

        private void OpenSendBuyRequestPanel()
        {
            _sendBuyRequestPanel.ShowPanel();
            _handButton.gameObject.SetActive(false);
            _openSendBuyRequestPanelButton.gameObject.SetActive(false);

        }
        
        private void OnExitButtonClick()
        {
            _sendBuyRequestPanel.HidePanel();
            _handButton.gameObject.SetActive(true);
            _openSendBuyRequestPanelButton.gameObject.SetActive(true);

        }
        
        private PurchaseRequestDTO FillPurchaseRequest()
        {
            PurchaseRequestDTO purchaseRequestDto = new();
            //purchaseRequestDto.Email = _emailInput.text;
            //purchaseRequestDto.Name = _loginInput.text;
            purchaseRequestDto.Comment = _commentsInput.text;
            purchaseRequestDto.PhoneNumber = $"+{_numberInput.text}";
            purchaseRequestDto.SpaceId = _isIndependent ? "81000000-0000-0000-0000-000000000000":_localSpace.Id.ToString();
            return purchaseRequestDto;
        }

        private bool GetValidationError(out string ValidateError)
        {
           ValidateError = string.Empty;
           ////login validation
           //if (string.IsNullOrEmpty(_loginInput.text))
           //{
           //    ValidateError = _emptyNameError.Localize();
           //    return true;
           //}
           //
           //if (_loginInput.text.Length < 2)
           //{
           //    ValidateError = _failNameValidation.Localize();
           //    return true;
           //}
           ////email validation
           //if (string.IsNullOrEmpty(_emailInput.text))
           //{
           //    ValidateError = _emptyEmailError.Localize();
           //    return true;
           //}
            // Phone Number Validation
            if (string.IsNullOrEmpty(_numberInput.text) || _numberInput.text.Length < 5 || _numberInput.text.Length > 25)
            {
                ValidateError = _failPhoneNumberError.Localize();
                return true;
            }

            return false;
        }
    }
}