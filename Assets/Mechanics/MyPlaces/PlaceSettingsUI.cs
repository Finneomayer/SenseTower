using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Audio;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Space;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Linq;
using Assets.Mechanics.Keyboard.Scripts;

public class PlaceSettingsUI : MonoBehaviour
{
    private const int _picturesInPlaceCount = 4;

    //[SerializeField] private Toggle[] _toggles;
    [SerializeField] private RectTransform _accessTypeContent;
    [SerializeField] private MyPlaceAccessTypeView _accessTypeView;
    [SerializeField] private RectTransform _pictureList;
    [SerializeField] private PlacePictureItemUI _placePictureItemPrefab;
    [SerializeField] private PlacePictureItemUI _emptyPictureItemPrefab;
    [SerializeField] private UserPlaceVisitorsUI _placeUsersVisitorsUI;
    [SerializeField] private UserPlaceAdminsUI _placeUsersAdminsUI;
    [SerializeField] private UserPlaceBlockListUI _placeUsersBlockListUI;

    //public Action<SpaceAccessType> AccesTypeChanged;
    public Action<LocalSpace> CallDoorImageChooser;
    public Action<int, LocalSpace> CallImageChooser;

    public LocalSpace Space => _myPlace;

    private LocalSpace _myPlace;
    private IApiService _apiService;
    private IMyPlaceService _myPlaceService;
    private IUsersInSpacesService _usersInSpacesService;
    private IClientData _clientData;
    private IAccountsService _accountsService;
    private ISpaceManager _spaceManager;

    //private MyPlaceAccessTypeController _accessTypeController;
    private RoomUsersWatcher _roomUsersWatcher;
    private RoomUsersKicker _roomUsersKicker;
    private IAudioService _audioService;
    private List<Guid> _connectedUserIdList = new();
    private bool _connectedUserIdListReceived;
    private Coroutine _refreshingSettingsRoutine;

    public void Init(LocalSpace myPlace, KeyboardScript keyboard, IApiService apiService, IMyPlaceService myPlaceService, 
        IUsersInSpacesService usersInSpacesService, IClientData clientData, IAccountsService accountsService, ISpaceManager spaceManager)
    {
        _roomUsersWatcher = FindObjectOfType<RoomUsersWatcher>();
        _roomUsersKicker = FindObjectOfType<RoomUsersKicker>();
        _audioService = FindObjectOfType<AgoraAudioService>();
        _roomUsersWatcher.ConnectedUserIdListReceived -= ConnectedUserIdListReceived;
        _roomUsersWatcher.ConnectedUserIdListReceived += ConnectedUserIdListReceived;

        _myPlace = myPlace;
        _apiService = apiService;
        _myPlaceService = myPlaceService;
        _usersInSpacesService = usersInSpacesService;
        _clientData = clientData;
        _accountsService = accountsService;
        _spaceManager = spaceManager;
        SetAvailabilitySettings(keyboard);
        SetAdminUI(keyboard);
        SetBlockListUI(keyboard);

        if (_refreshingSettingsRoutine != null)
        {
            StopCoroutine(_refreshingSettingsRoutine);
        }

        _refreshingSettingsRoutine = StartCoroutine(RefreshingSettingsRoutine());
    }

    private void OnEnable()
    {
        if (_roomUsersWatcher != null)
        {
            _roomUsersWatcher.ConnectedUserIdListReceived -= ConnectedUserIdListReceived;
            _roomUsersWatcher.ConnectedUserIdListReceived += ConnectedUserIdListReceived;
        }

        _placeUsersVisitorsUI.KickRequested += OnVisitorKickRequested;
        _placeUsersVisitorsUI.MuteRequested += OnVisitorMuteRequested;
    }

    private void OnVisitorKickRequested(UserInSpaceInfoDto visitorData)
    {
        if (_roomUsersKicker == null)
        {
            return;
        }

        _roomUsersKicker.KickUserServerRpc(visitorData.UserId.ToString(), UserKickReason.PrivateSpace);
    }

    private void OnVisitorMuteRequested(UserInSpaceInfoDto visitorData)
    {
        if (_audioService == null)
        {
            return;
        }

        if (_audioService.MutedUsersID.ContainsValue(visitorData.UserId.ToString()))
            _audioService.UnmuteUser(visitorData.UserId.ToString());
        else
            _audioService.MuteUser(visitorData.UserId.ToString());
    }

    private void OnDisable()
    {
        if (_roomUsersWatcher != null)
        {
            _roomUsersWatcher.ConnectedUserIdListReceived -= ConnectedUserIdListReceived;
        }

        _placeUsersVisitorsUI.KickRequested -= OnVisitorKickRequested;
        _placeUsersVisitorsUI.MuteRequested -= OnVisitorMuteRequested;
    }

    private void ConnectedUserIdListReceived(List<string> connectedUserIdList)
    {
        _connectedUserIdList.Clear();
        foreach (var item in connectedUserIdList)
        {
            _connectedUserIdList.Add(Guid.Parse(item));
        }

        _connectedUserIdListReceived = true;
    }

    public void SetImages()
    {
        if (_myPlace == null)
        {
            return;
        }

        if (_myPlace.DoorImage == null || _myPlace.DoorImage.Id == Guid.Empty)
        {
            CreateDoorImageItem(_myPlace);
        }
        else
        {
            CreateDoorImageItem(_myPlace.DoorImage, _myPlace);
        }

        int maxDictionaryKey = 0;

        foreach (var image in _myPlace.Images)
        {
            if (image.Key > maxDictionaryKey) maxDictionaryKey = image.Key;
        }

        if (maxDictionaryKey < _picturesInPlaceCount - 1) maxDictionaryKey = _picturesInPlaceCount - 1;

        for (int imageKey = 0; imageKey <= maxDictionaryKey; imageKey++)
        {
            if (_myPlace.Images.ContainsKey(imageKey)) CreateImageItem(imageKey, _myPlace.Images[imageKey], _myPlace);
            else if (imageKey < _picturesInPlaceCount) CreateImageItem(imageKey, _myPlace);
        }
    }

    /// <summary>
    /// For existing door image
    /// </summary>
    /// <param name="image">existing image</param>
    /// <param name="myPlace"></param>
    private void CreateDoorImageItem(MyImage image, LocalSpace myPlace)
    {
        var picture = Instantiate(_placePictureItemPrefab, _pictureList);
        picture.SetImage(image, isDoorImage: true);
        picture.OnClick += () => { CallDoorImageChooser(myPlace); };
    }

    /// <summary>
    /// When NO door image
    /// </summary>
    /// <param name="myPlace"></param>
    private void CreateDoorImageItem(LocalSpace myPlace)
    {
        var picture = Instantiate(_emptyPictureItemPrefab, _pictureList);
        picture.SetImage(isDoorImage: true);
        picture.OnClick += () => { CallDoorImageChooser(myPlace); };
    }

    /// <summary>
    /// For existing place image
    /// </summary>
    /// <param name="key">image place number in my place</param>
    /// <param name="image"></param>
    /// <param name="myPlace"></param>
    private void CreateImageItem(int key, MyImage image, LocalSpace myPlace)
    {
        var picture = Instantiate(_placePictureItemPrefab, _pictureList);
        picture.SetImage(image);
        picture.OnClick += () => { CallImageChooser(key, myPlace); };
    }

    /// <summary>
    /// For NOT existing place image
    /// </summary>
    /// <param name="key">image place number in my place</param>
    /// <param name="myPlace"></param>
    private void CreateImageItem(int key, LocalSpace myPlace)
    {
        var picture = Instantiate(_emptyPictureItemPrefab, _pictureList);
        picture.SetImage();
        picture.OnClick += () => { CallImageChooser(key, myPlace); };
    }

    private void SetAvailabilitySettings(KeyboardScript keyboard)
    {
        MyPlaceAccessTypeController accessTypeController = new(_apiService);
        _accessTypeView.Init(_myPlace, keyboard);
        accessTypeController.Init(_accessTypeView, _myPlace);
        accessTypeController.RefreshView();
    }

    private void SetAdminUI(KeyboardScript keyboard)
    {
        _placeUsersAdminsUI.gameObject.SetActive(false);
        if (_myPlace == null || _myPlace.SpaceOwner == null)
        {
            return;
        }

        if (_myPlace.SpaceOwner.UserId != _clientData.UserId)
        {
            return;
        }

        _placeUsersAdminsUI.gameObject.SetActive(true);
        _placeUsersAdminsUI.Init(_myPlace, keyboard, _myPlaceService, _accountsService);
    }

    private void SetBlockListUI(KeyboardScript keyboard)
    {
        _placeUsersBlockListUI.gameObject.SetActive(false);
        if (_myPlace == null)
        {
            return;
        }

        _placeUsersBlockListUI.gameObject.SetActive(true);
        _placeUsersBlockListUI.Init(_myPlace, keyboard, _myPlaceService, _accountsService);
    }

    private IEnumerator RefreshingSettingsRoutine()
    {       
        bool isUserInRoom = _spaceManager.CurrentTransitionTarget != null
                            && _spaceManager.CurrentTransitionTarget.Id == _myPlace.Id;

        WaitForSeconds checkPeriod = new WaitForSeconds(isUserInRoom ? 0.5f : 5);
        List<UserInSpaceInfoDto> usersInSpace = new();

        while (_usersInSpacesService != null && _placeUsersVisitorsUI != null)
        {
            Task<UsersInSpaceResponse> task = _usersInSpacesService.GetUsersInSpaces(null).AsTask();
            yield return new WaitUntil(() => task.IsCompleted);

            usersInSpace.Clear();
            if (task.Result == null || task.Result.Users == null)
            {
                RefreshVisitorsUI(usersInSpace, isUserInRoom);
                yield return checkPeriod;
                continue;
            }

            foreach (var user in task.Result.Users)
            {
                if (user.SpaceId == _myPlace.Id)
                {
                    usersInSpace.Add(user);
                }
            }

            if (usersInSpace.Count == 0 || !isUserInRoom)
            {
                RefreshVisitorsUI(usersInSpace, isUserInRoom);
                yield return checkPeriod;
                continue;
            }

            if (_roomUsersWatcher != null && NetworkManager.Singleton != null)
            {
                _connectedUserIdListReceived = false;
                _roomUsersWatcher.RequestConnectedUsersServerRpc(NetworkManager.Singleton.LocalClientId);
                yield return new WaitUntil(() => _connectedUserIdListReceived);

                for (int i = usersInSpace.Count - 1; i >= 0; i--)
                {
                    if (!_connectedUserIdList.Contains(usersInSpace[i].UserId))
                    {
                        usersInSpace.RemoveAt(i);
                    }
                }

                RefreshVisitorsUI(usersInSpace, isUserInRoom);
            }

            yield return checkPeriod;
        }

        _refreshingSettingsRoutine = null;
    }

    private void RefreshVisitorsUI(List<UserInSpaceInfoDto> usersInSpace, bool isUserInRoom)
    {
        if (_placeUsersVisitorsUI == null)
        {
            return;
        }

        if (_clientData == null || _clientData.UserId == null || usersInSpace == null || usersInSpace.Count == 0)
        {
            _placeUsersVisitorsUI.Clear();
            return;
        }

        _placeUsersVisitorsUI.Init(_myPlace, usersInSpace, _clientData.UserId.Value, isUserInRoom, _audioService);
    }
}