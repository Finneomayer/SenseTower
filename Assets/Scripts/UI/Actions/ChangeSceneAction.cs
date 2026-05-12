using Assets.Localization;
using Assets.Scripts;
using Assets.Scripts.Cinema;
using Assets.Scripts.Client;
using Assets.Scripts.Event;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using Client;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Assets.Mechanics.Lift;
using UnityEngine;
using Zenject;

public class ChangeSceneAction : UIAction
{
    #region Inspector
    public SpaceType spaceType;
    [SerializeField] private CompositionRootEnterScene _compositionRootEnterScene;
    [SerializeField] private int _hallIndex = 0;
    [SerializeField] private string _spaceId = string.Empty;
    [SerializeField] private LocalizationVariant _privateEventLocalizationVariant;
    public int HallIndex => _hallIndex;
    private IClientData _clientData;
    private ITowerEventService _towerEventService;
    private ICinemaService _cinemaService;
    private ISpaceService _spaceService;
    private IHallsService _hallService;

    private LocalSpace space;

    [Inject]
    public void Init(IClientData clientData, ITowerEventService towerEventService,
        ICinemaService cinemaService, ISpaceService spaceService, IHallsService hallService)
    {
        _clientData = clientData;
        _towerEventService = towerEventService;
        _cinemaService = cinemaService;
        _spaceService = spaceService;
        _hallService = hallService;
    }

    #endregion
    private void Awake()
    {
        if (_compositionRootEnterScene == null)
            _compositionRootEnterScene = FindObjectOfType<CompositionRootEnterScene>();
        if (!string.IsNullOrEmpty(_spaceId))
        {
            var placeItem = GetComponent<PlaceItemUI>();
            if (placeItem != null)
            {
                placeItem.SetSpaceId(_spaceId);
            }
        }
    }

    /// <summary>
    /// will run when button clicked
    /// </summary>
    public override void Invoke() 
    {
        base.Invoke();
        TryLoadScene().Forget();
    }

    private async UniTask<Guid> FindSpaceOnTheFloor(SpaceType spaceType, int hallIndex)
    {
        Guid spaceId = Guid.Empty;

        Hall[] halls = await _hallService.GetHalls();
        if (halls.Length <= hallIndex)
        {
            Debug.LogError("halls.Length <= hallIndex. Cannot find space");
            return spaceId;
        }

        if (spaceType == SpaceType.HallScene)
        {
            spaceId = halls[hallIndex].Space.Id;
        }
        else
        {
            foreach (var localSpace in halls[hallIndex].Spaces)
            {
                if (spaceType == localSpace.SpaceType)
                {
                    spaceId = localSpace.Id;
                    break;
                }
            }
        }
        return spaceId;
    }

    private async UniTask TryLoadScene()
    {
        var placeItem = GetComponent<PlaceItemUI>();
        if (placeItem != null)
        {
            if(!string.IsNullOrEmpty(placeItem.GetSpaceId()))
                _spaceId = placeItem.GetSpaceId();

            if (placeItem.GetSpaceType() != 0)
                spaceType = placeItem.GetSpaceType();
        }

        if (string.IsNullOrEmpty(_spaceId)) //for Halls
        {
            Guid firstFloorSpaceId = await FindSpaceOnTheFloor(spaceType, _hallIndex);
            if (firstFloorSpaceId != Guid.Empty)
            {
                _spaceId = firstFloorSpaceId.ToString();
                //PlayerPrefs.SetInt("PlayerInHallIndex", _hallIndex); 
                //PlayerPrefs.SetInt("PlayerOnFloor", 0); 
                PlayerLiftPosition.PlayerInHallIndex = _hallIndex; //saving choosen Hall index at start 
                PlayerLiftPosition.PlayerOnFloorIndex = 0; //saving first floor of Hall at start
                PlayerLiftPosition.PutPlayerToTheLift = false; //tell that not need to put player near lift on spawn
            }
        }

        space = _spaceService.Get(spaceType, _spaceId);
        if (space == null)
        {
            return;
        }
        if (await IsLocalUserAdmin(space))
        {
            LoadScene();
            return;
        }

        TowerEventsFilter filter = new();
        filter.FromMinusSecondsNow = 300;
        filter.UpToPlusSecondsNow = 900;
        filter.States = new[] { TowerEventState.Planned };
        filter.Spaces = new[] { space.Id };

        TowerEvent[] currentEventsInSpace = await _towerEventService.GetEvents(filter);

        if (currentEventsInSpace == null || currentEventsInSpace.Length == 0)
        {
            LoadScene();
            return;
        }

        if (_clientData == null || !_clientData.UserId.HasValue)
        {
            Debug.LogError("<color=red> clientData == null</color>");
            return;
        }

        bool isUserHasTicket = false;
        foreach (var towerEvent in currentEventsInSpace)
        {
            if (towerEvent.TotalTickets == 0)
            {
                isUserHasTicket = true;
                break;
            }

            if (towerEvent.SoldTickets == null)
            {
                continue;
            }

            Ticket userTicket = towerEvent.SoldTickets.FirstOrDefault(t => t.UserId != null
                && t.UserId.Value.Equals(_clientData.UserId.Value));
            if (userTicket != null)
            {
                isUserHasTicket = true;
                break;
            }
        }

        if (isUserHasTicket)
        {
            LoadScene();
            return;
        }
        else
        {
            NotificationPanel.instance.SetInfo(_privateEventLocalizationVariant.Localize());
            NotificationPanel.instance.ShowPanel();
        }
    }

    private async UniTask<bool> IsLocalUserAdmin(LocalSpace space)
    {
        if (space == null)
        {
            return false;
        }

        if (_clientData == null || !_clientData.UserId.HasValue)
        {
            return false;
        }

        if (spaceType != SpaceType.CinemaScene)
        {
            return false;
        }

        Cinema cinema = await _cinemaService.GetBySpaceId(space.Id.ToString());
        if (cinema == null || cinema.Administrators == null)
        {
            return false;
        }

        return cinema.Administrators.FirstOrDefault(a => a.Id.Equals(_clientData.UserId.Value)) != null;
    }

    private void LoadScene()
    {
        ClientDataInSpace clientDataInSpace = new();
        clientDataInSpace.Clear();

        if (spaceType == SpaceType.Null) return;
        NotificationPanel.instance.SetDefaultInfo();
        NotificationPanel.instance.ShowPanel();

        bool isPrivate = space != null ? space.IsPrivate : false;

        if (spaceType == SpaceType.HallScene)
        {
            _clientData.SetLastSpaceDoorData(null);
        }
        else
        {
            _clientData.SetLastSpaceDoorData(new SpaceDoorData(isPrivate, spaceType, _spaceId));
        }
        _compositionRootEnterScene.LoadingSceneChangerView.LoadScene(spaceType, _spaceId);
    }
}
