using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Localization;
using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UI_assets_legacy.Pad;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class HallVisitorsViewPanel : ViewPanel
{
    [SerializeField] private ViewPanel _errorPanel;
    [SerializeField] private ViewPanel _loadingPanel;
    [SerializeField] private RectTransform _parentPanel;
    [SerializeField] private float _updatePeriod = 1f;
    [SerializeField] private int _usersCount = 20;
    [SerializeField] private UserInSpaceViewElement _visitorInSpaceViewElement;
    [SerializeField] private TMP_Text _totalUsersText;
    [SerializeField] private KeyboardScript _keyboard;
    [SerializeField] private TMP_InputField _searchField;
    [SerializeField] private Button _startSearch;
    [SerializeField] private Button _stopSearch;
    [SerializeField] private List<UserInSpaceViewElement> _usersInSpaces = new List<UserInSpaceViewElement>();

    [SerializeField]
    private LocalizationVariant TotalUserCountLocalizationVariant;
    [SerializeField]
    private LocalizationVariant UsersFoundLocalizationVariant;

    private Coroutine _updateCoroutine;
    private ISpaceService _spaceService;
    private bool _filterOn;

    [Inject]
    private void Construct(ISpaceService spaceService)
    {
        _spaceService = spaceService;
    }

    void Start()
    {
        for (int i = 0; i < _usersCount + 1; i++)
        {        
            UserInSpaceViewElement tempUserInSpaceViewElement = Instantiate(_visitorInSpaceViewElement,_parentPanel);
            tempUserInSpaceViewElement.gameObject.SetActive(false);
            _usersInSpaces.Add(tempUserInSpaceViewElement);
        }
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        _updateCoroutine = StartCoroutine(UpdateStatisticsCoroutine());
    }

    public override void HidePanel()
    {
        base.HidePanel();
        _errorPanel.HidePanel();
        _loadingPanel.HidePanel();
        if(_updateCoroutine != null) 
            StopCoroutine(_updateCoroutine);
    }
    
    private void InstantiateClients(UsersInSpaceResponse usersInSpaces)
    {
        for (int i = 0; i < usersInSpaces.Users.Length; i++)
        {
            if (usersInSpaces.Users[i].UserName.Equals("camera")) continue;
            
            UserInSpaceInfoDto tempUserInSpace = usersInSpaces.Users[i];
            UserInSpaceViewElement tempUserInSpaceViewElement = _usersInSpaces[i];
            tempUserInSpaceViewElement.gameObject.SetActive(true);
            tempUserInSpaceViewElement.SetNameValue(tempUserInSpace.UserName);
            tempUserInSpaceViewElement.SetSpaceValue(tempUserInSpace.SpaceName);
        }
    }

    private async UniTask<bool> UpdateVisitorsStatistics()
    {
        UsersInSpaceResponse usersInSpaces = await _spaceService.GetUsersInSpaces(_usersCount);
        _loadingPanel.HidePanel();
        if (usersInSpaces == null)
        {
            _errorPanel.ShowPanel();
        }
        else
        {
            HideAllClientsView();
            if (usersInSpaces.Users.Length > 0)
            {
                _errorPanel.HidePanel();

                _totalUsersText.text = TotalUserCountLocalizationVariant.Localize(usersInSpaces.Users.Length);

                if (_filterOn)
                {
                    usersInSpaces = ApplyUserFilter(usersInSpaces, _searchField.text);
                    _totalUsersText.text += $". {UsersFoundLocalizationVariant.Localize(usersInSpaces.Users.Length)}";
                }

                InstantiateClients(usersInSpaces);
            }
        }

        return true;
    }

    private UsersInSpaceResponse ApplyUserFilter(UsersInSpaceResponse users, string filter)
    {
        var result = new UsersInSpaceResponse();
        List<UserInSpaceInfoDto> userList = new List<UserInSpaceInfoDto>();

        foreach (var user in users.Users)
        {
            if (user.UserName.ToLower().Contains(filter.ToLower())) userList.Add(user);
        }

        result.Users = userList.ToArray();
        return result;
    }

    private void HideAllClientsView()
    {
        for (int i = 0; i < _usersInSpaces.Count; i++)
        {
            UserInSpaceViewElement tempUserInSpaceViewElement = _usersInSpaces[i];
            tempUserInSpaceViewElement.gameObject.SetActive(false);
        }
    }

    private IEnumerator UpdateStatisticsCoroutine()
    {
        while (true)
        {
            _loadingPanel.ShowPanel();
            yield return UpdateVisitorsStatistics();

            yield return new WaitForSeconds(_updatePeriod);
        }
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _searchField.onSelect.AddListener(delegate
        {
            _startSearch.gameObject.SetActive(true);
            _stopSearch.gameObject.SetActive(false);
            _filterOn = false;
            ShowKeyboard(_searchField);
        });
        _startSearch.onClick.AddListener(delegate
        {
            if (!String.IsNullOrEmpty(_searchField.text))
            {
                DisableKeyboard();
                _startSearch.gameObject.SetActive(false);
                _stopSearch.gameObject.SetActive(true);
                _filterOn = true;
            }
        });
        _stopSearch.onClick.AddListener(delegate
        {
            _startSearch.gameObject.SetActive(true);
            _stopSearch.gameObject.SetActive(false);
            _searchField.text = String.Empty;
            _filterOn = false;
        });
    }

    private void UnsubscribeToEvents()
    {
        _searchField.onSelect.RemoveAllListeners();
        _startSearch.onClick.RemoveAllListeners();
        _stopSearch.onClick.RemoveAllListeners();
    }

    private void ShowKeyboard(TMP_InputField inputField) 
    {
        _keyboard.OpenKeyboard(inputField);
    }

    private void DisableKeyboard() 
    {
        _keyboard.CloseKeyboard();
    }
}
