using Assets.Localization;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Assets.Mechanics.Keyboard.Scripts;
using Mono.CSharp;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyPlaceAccessTypeView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup ToggleContent;
    [SerializeField]
    private MyPlaceAccessTypeToggle AccessTypeTogglePrefab;
    [SerializeField]
    private MyPlaceAccessTypeToggle AccessTypePaidTogglePrefab;
    [SerializeField]
    private LocalizationVariant PublicTextLocalizationVariant;
    [SerializeField]
    private LocalizationVariant LiveTextLocalizationVariant;
    [SerializeField]
    private LocalizationVariant WhoIsInsideLocalizationVariant;
    [SerializeField]
    private LocalizationVariant OnlyFriendstLocalizationVariant;
    [SerializeField]
    private LocalizationVariant PrivateTextLocalizationVariant;
    [SerializeField]
    private LocalizationVariant PaidLocalizationVariant;
    [SerializeField]
    private LocalizationVariant PaidLocalizationExtendedLocalizationVariant;

    public decimal Tax {
        get
        {
            if (decimal.TryParse(_inputField.text, out decimal tax))
            {           
                return tax;
            }

            return 0;
        }
    }
    private Dictionary<SpaceAccessType, MyPlaceAccessTypeToggle> _accessTypePrefabMap = new();
    private KeyboardScript _keyboard;
    private TMP_InputField _inputField;
    private Button _applyButton;

    public Action<SpaceAccessType> AccessTypeChanged;

    public void Init(LocalSpace myPlace, KeyboardScript keyboard)
    {
        _keyboard = keyboard;

        foreach (var item in _accessTypePrefabMap)
        {
            Destroy(item.Value.gameObject);
        }

        var accessTypeTextMap = GetAccessTypeTextMap();
        foreach (var item in accessTypeTextMap)
        {
            var prefab = AccessTypeTogglePrefab;

            if (item.Key == SpaceAccessType.Paid) prefab = AccessTypePaidTogglePrefab;

            //for base prefab
            MyPlaceAccessTypeToggle newItem = Instantiate(prefab, ToggleContent.transform);
            newItem.Init(item.Value, item.Key == myPlace.PublicAccessType);
            newItem.ToggledOn += () => AccessTypeToggledOn(item.Key);
            _accessTypePrefabMap[item.Key] = newItem;

            //for custom paid prefab
            if (item.Key == SpaceAccessType.Paid)
            {
                _applyButton = newItem.ApplyButton;
                _applyButton.onClick.AddListener(SendCostOfPaidEnter);

                //newItem.ToggledOn += SendCostOfPaidEnter;
                
                if (myPlace.PublicAccessModeSettings != null &&
                    myPlace.PublicAccessModeSettings.PaymentAccessModeSettings != null &&
                    myPlace.PublicAccessModeSettings.PaymentAccessModeSettings.DailyTax > 0)
                {
                    newItem.TwrInput.text =
                        myPlace.PublicAccessModeSettings.PaymentAccessModeSettings.DailyTax.ToString(CultureInfo.CurrentCulture);
                }

                newItem.TextLefT.text = PaidLocalizationExtendedLocalizationVariant.Localize().
                    Remove(PaidLocalizationExtendedLocalizationVariant.Localize().IndexOf('{'));

                newItem.TextRight.text = PaidLocalizationExtendedLocalizationVariant.Localize().
                    Substring(PaidLocalizationExtendedLocalizationVariant.Localize().IndexOf('}') + 1);
                
                newItem.TwrInput.onSelect.AddListener((e) => TwrInputOnSelect());
                _inputField = newItem.TwrInput;
            }
        }
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
    }

    
    private void TwrInputOnSelect()
    {
        _applyButton.gameObject.SetActive(true);
        ShowKeyboard();
    }

    private void FixedUpdate()
    {
        if (_inputField != null)
        {
            if (_inputField.text.Length > 3)
            {
                _inputField.text = _inputField.text.Substring(0, 3);
            }
        }
    }

    private async void SendCostOfPaidEnter()
    {
        Debug.LogWarning($"OnPressed {_inputField.text}");
        AccessTypeChanged?.Invoke(SpaceAccessType.Paid);

        await Task.Delay(200);
        _applyButton.gameObject.SetActive(false);
    }

    public void SetInteractable(bool interactable)
    {
        ToggleContent.alpha = interactable ? 1 : 0.05f;
        ToggleContent.interactable = interactable;
    }

    public void SetAccessType(SpaceAccessType accessType)
    {
        foreach (var item in _accessTypePrefabMap)
        {
            item.Value.SetIsOnWithoutNotify(item.Key == accessType);
        }
    }

    private void AccessTypeToggledOn(SpaceAccessType newAccessType)
    {
        if (newAccessType == SpaceAccessType.Paid)
        {
            ShowKeyboard();
        }
        else
        {
            AccessTypeChanged?.Invoke(newAccessType);
            HideKeyboard();
        }
    }

    private Dictionary<SpaceAccessType, string> GetAccessTypeTextMap()
    {
         return new()
         {
             [SpaceAccessType.Public] = PublicTextLocalizationVariant.Localize(),
             [SpaceAccessType.Live] = LiveTextLocalizationVariant.Localize(),
             [SpaceAccessType.WhoInside] = WhoIsInsideLocalizationVariant.Localize(),
             [SpaceAccessType.OnlyFriends] = OnlyFriendstLocalizationVariant.Localize(),
             [SpaceAccessType.Private] = PrivateTextLocalizationVariant.Localize(),
             [SpaceAccessType.Paid] = PaidLocalizationVariant.Localize(),
         };
    }

    private void ShowKeyboard()
    {
        _keyboard.OpenKeyboard(_inputField, integerFilterOn:true);
    }

    private void HideKeyboard()
    {
        _keyboard.CloseKeyboard();
    }
}
