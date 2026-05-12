using Assets.Scripts.API;
using System.Collections.Generic;
using Assets.Mechanics.MyPlaces;
using UI;
using UnityEngine;
using Zenject;
using Assets.Scripts.Space;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Mechanics.Keyboard.Scripts;
using Cysharp.Threading.Tasks;
using System.Linq;

public class MyPlaceView : MonoBehaviour
{
    [SerializeField] private ViewPanel _viewPanel;
    [SerializeField] private RectTransform _apartmentListUI;
    [SerializeField] private RectTransform _apartmentSettingsContainer;
    [SerializeField] private PlaceItemUI _itemPrefab;
    [SerializeField] private PlaceSettingsUI _settingsPrefab;
    [SerializeField] private ViewPanel _imageChooserPanel;

    private IMyPlaceService _myPlaceService = new MyPlaceService();
    private IApiService _apiService;
    private IUsersInSpacesService _usersInSpacesService;
    private IClientData _clientData;
    private IAccountsService _accountsService;
    private ISpaceManager _spaceManager;

    private LocalSpace[] _mySpaces;
    private List<PlaceItemUI> _placeItems = new List<PlaceItemUI>();
    private PlaceSettingsUI _placeSettingsUi;

    private LocalSpace _currentMyPlace;
    private PlaceItemUI _currentPlaceItem;

    [Inject]
    public async void Init(IMyPlaceService myPlaceService, IApiService apiService,
        IUsersInSpacesService usersInSpacesService, ISpaceService spaceService, IClientData clientData,
        IAccountsService accountsService, ISpaceManager spaceManager)
    {
        _myPlaceService = myPlaceService;
        _apiService = apiService;
        _usersInSpacesService = usersInSpacesService;
        _clientData = clientData;
        _accountsService = accountsService;
        _spaceManager = spaceManager;

        List<LocalSpace> places = (await _myPlaceService.GetAllMySpaces()).ToList();

        foreach (LocalSpace space in spaceService.GetAllSpaces())
        {
            if (places.FirstOrDefault(p => p.Id == space.Id) != null)
            {
                continue;
            }
            if (DataExtensions.AvailableAdmin(space,clientData.UserId.ToString()))
            {
                places.Add(space);
            }
        }
        
        _mySpaces = places.ToArray();
        GetMySpaces();
    }

    public void RefreshMySpaceItems()
    {
        if (_currentMyPlace != null && _currentPlaceItem != null) OnSelectItem(_currentMyPlace, _currentPlaceItem);
    }

    private void GetMySpaces()
    {
        if (_mySpaces != null && _mySpaces.Length > 0)
        {
            foreach (var space in _mySpaces)
            {
                CreateSpaceUi(_apartmentListUI, space);
                //Debug.LogWarning($"CreateSpaceUi {space.Images.Count}");
            }

            SetSelectedFirstItem();
        }
        else
        {
            //show message "No My Places!"
        }
    }

    private void SetSelectedFirstItem()
    {
        if (_placeItems.Count > 0)
        {
            _placeItems[0].SetSelected(true);
            _currentMyPlace = _mySpaces[0];
            _currentPlaceItem = _placeItems[0];
            ShowSpaceSettings(_mySpaces[0]).Forget();
        }
    }

    private void CreateSpaceUi(RectTransform parent, LocalSpace myPlace)
    {
        var item = Instantiate(_itemPrefab, parent);
        item.SetName(myPlace.SpaceName);
        item.SetSpaceId(myPlace.Id.ToString());

        if (myPlace.SpaceOwner != null && myPlace.SpaceOwner.UserId == _clientData.UserId)
        {
            item.CurrentUserIsAdmin(false);
        }
        else
        {
            item.CurrentUserIsAdmin(DataExtensions.AvailableAdmin(myPlace, _clientData.UserId.ToString()));
        }
        
        item.OnSelect += () =>
        {
            _currentMyPlace = myPlace;
            _currentPlaceItem = item;
            OnSelectItem(_currentMyPlace, _currentPlaceItem);
        };
        _placeItems.Add(item);
    }

    private void OnSelectItem(LocalSpace myPlace, PlaceItemUI placeItem)
    {
        foreach (var item in _placeItems)
        {
            item.SetSelected(false);
        }
        placeItem.SetSelected(true);
        ShowSpaceSettings(myPlace).Forget();
    }

    private async UniTask ShowSpaceSettings(LocalSpace myPlace)
    {
        await UniTask.WaitUntil(() => _viewPanel.Keyboard != null);
        if (_placeSettingsUi != null)
        {
            Destroy(_placeSettingsUi.gameObject);
        }

        _placeSettingsUi = Instantiate(_settingsPrefab, _apartmentSettingsContainer);
        _placeSettingsUi.Init(myPlace, _viewPanel.Keyboard, _apiService, _myPlaceService, _usersInSpacesService, _clientData, _accountsService, 
            _spaceManager);
        _placeSettingsUi.SetImages();
        _placeSettingsUi.CallImageChooser += (key, myPlace) =>
        {
            _imageChooserPanel.ShowPanel();
            var placePictureChooser = _imageChooserPanel.GetComponent<PlacePictureChooser>();
            placePictureChooser.SetChangingImage(myPlace, key);
            placePictureChooser.Refresh();
        };
        _placeSettingsUi.CallDoorImageChooser += (myPlace) =>
        {
            _imageChooserPanel.ShowPanel();
            var placePictureChooser = _imageChooserPanel.GetComponent<PlacePictureChooser>();
            placePictureChooser.SetChangingImage(myPlace, 0, true);
            placePictureChooser.Refresh();
        };
    }
}
