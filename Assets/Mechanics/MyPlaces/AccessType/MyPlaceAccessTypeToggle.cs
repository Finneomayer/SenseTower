using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPlaceAccessTypeToggle : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Text;
    [SerializeField]
    private Toggle Toggle;

    [SerializeField] private TMP_InputField _twrInput;
    [SerializeField] private TMP_Text _textPrimary;
    [SerializeField] private TMP_Text _textSecondary;
    [SerializeField] private Button _applyButton;
    [SerializeField] private GameObject _extendedContent;

    public event Action ToggledOn;

    //for custom paid toggle:
    public TMP_InputField TwrInput
    {
        get => _twrInput;
        set => _twrInput = value;
    }
    public TMP_Text TextLefT 
    { 
        get => _textPrimary;
        set => _textPrimary = value;
    }
    public TMP_Text TextRight
    {
        get => _textSecondary;
        set => _textSecondary = value;
    }
    public Button ApplyButton
    {
        get => _applyButton;
        set => _applyButton = value;
    }

    public void Init(string text, bool isOn = false)
    {
        Text.text = text;
        Toggle.isOn = isOn;
        Toggle.onValueChanged.AddListener(OnToggled);
    }

    public void SetIsOnWithoutNotify(bool isOn)
    {
        Toggle.SetIsOnWithoutNotify(isOn);
        SetContentExtended(isOn);
    }

    private void OnToggled(bool value)
    {
        if (value)
        {
            ToggledOn?.Invoke();
            SetContentExtended(value);
        }
        else
        {
            Toggle.SetIsOnWithoutNotify(true);
        }
    }

    private void SetContentExtended(bool isExtended)
    {
        if (_extendedContent != null)
        {
            _extendedContent.SetActive(isExtended);
            Text.gameObject.SetActive(!isExtended);
        }
    }
}
