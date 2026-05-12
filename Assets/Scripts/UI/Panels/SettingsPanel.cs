using System;
using System.Linq;
using Assets.Localization;
using Assets.Scripts.Client;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(ToggleGroup))]
public class SettingsPanel : MainPanel
{
    [SerializeField] private LoginPanel LoginPanel;
    public TMPro.TMP_Text userName;
    [SerializeField] 
    private TMPro.TMP_Text _contourType;
    [SerializeField]
    private Toggle[] _toggles;
    [SerializeField]
    private ApplicationRunner _applicationRunner;

    private ToggleGroup _toggleGroup;
    private IClientData _clientData;

    [Inject]
    private void Construct(IClientData clientData)
    {
        _clientData = clientData;
    }

    private void Awake()
    {
        SetUserName(_clientData.UserName);
        LoginPanel.UserLogInSuccess += OnAuthorization;
        LoginPanel.UserLogOut += OnDeleteData;
        _clientData.DeleteAllAuthData += OnDeleteData;
        _toggleGroup = GetComponent<ToggleGroup>();
    }
    private void Start()
    {
        foreach (Toggle toggle in _toggles)
        {
            if (toggle.name.Equals(LocalizationManager.CurrentLanguageCode)) toggle.isOn = true;
        }

        if (_applicationRunner != null)
            _contourType.text = _applicationRunner.BootstrapperPrefab.DiscoveryService.Assembly.AssemblyType.ToString();
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (Toggle toggle in _toggles)
        {
            if (toggle.group != null && toggle.group != _toggleGroup)
            {
                Debug.LogError($"EventToggleGroup is trying to register a Toggle that is a member of another group.");
            }
            toggle.group = _toggleGroup; 

            toggle.onValueChanged.AddListener((args) => LanguageToggleOnPressed(args, toggle.name));
        }
    }

    private void LanguageToggleOnPressed(bool args, string name)
    {
        if (args)
        {            
            LocalizationManager.SetLanguage(name);
        }        
    }

    void OnDisable()
    {
        LoginPanel.UserLogInSuccess -= OnAuthorization;
        LoginPanel.UserLogOut -= OnDeleteData;
        _clientData.DeleteAllAuthData -= OnDeleteData;

        foreach (Toggle toggle in _toggleGroup.ActiveToggles())
        {
            toggle.group = null;
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
    
    private void OnAuthorization(string name)
    {
        SetUserName(name);
    }

    private void OnDeleteData()
    {
        SetUserName("");
    }

    private void SetUserName(string name)
    {
        userName.text = name;
    }
}
