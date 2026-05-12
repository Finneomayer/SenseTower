using Assets.Scripts.Event;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using Zenject;
using Assets.Localization;

public class LocationsViewPanel : ViewPanel
{
    [SerializeField] private ViewPanel _errorPanel;
    [SerializeField] private ViewPanel _loadingPanel;
    [SerializeField] private TMP_Text _totalUsersText;
    [SerializeField] private LocationViewElement _locationViewPrefab;
    [SerializeField] private List<LocationViewElement> _locationViewItems;
    [SerializeField] private LocalizationVariant _usersFoundLocalizationVariant;

    private Hall[] _halls;
    private List<LocalSpace> _spaces;
    private Coroutine _updateCoroutine;
    private IHallsService _hallService;
    private ITowerEventService _towerEventService;

    [Inject]
    private async void Init(IHallsService hallService, ITowerEventService towerEventService)
    {
        _hallService = hallService;
        _towerEventService = towerEventService;
        _halls = await _hallService.GetHalls();

        //int publicPlacesCount = _halls[0].PublicPlaces.Length;
        //int userPlacesCount = _halls[0].UserPlaces.Length;

        //Debug.LogWarning($"UI items count: {_locationViewItems.Count}. " +
        //                 $"Public places count: {publicPlacesCount}. " +
        //                 $"User places count: {userPlacesCount}");

        //for (int i = 0; i < _locationViewItems.Count; i++)
        //{
        //    if (i < publicPlacesCount) //public places
        //    {
        //        _locationViewItems[i].SetLocationValue(_halls[0].PublicPlaces[i].SpaceName);
        //        _locationViewItems[i].SetStatusValue("Открыта");
        //        _locationViewItems[i].SetOwnerValue("SENSE");
        //    }
        //    else if (i - publicPlacesCount < userPlacesCount) //user places
        //    {
        //        _locationViewItems[i].SetLocationValue(_halls[0].UserPlaces[i - publicPlacesCount].LocalSpace.SpaceName);
        //        _locationViewItems[i].SetOwnerValue(_halls[0].UserPlaces[i - publicPlacesCount].OwnerName);
        //        _locationViewItems[i].SetStatusValue(AccessTypeToString(_halls[0].UserPlaces[i - publicPlacesCount].PublicAccessType));
        //    }
        //}

    }

    private void Start()
    {
        _totalUsersText.text = _usersFoundLocalizationVariant.Localize(0);
    }

    private async UniTask<TowerEvent[]> GetEvents()
    {
        var currentTowerEventsFilter = new TowerEventsFilter();
        currentTowerEventsFilter.FromMinusSecondsNow = 300; // 5 minuts after previous event ended
        currentTowerEventsFilter.UpToPlusSecondsNow = 900; // 15 minutes before next event started
        currentTowerEventsFilter.States = new[] { TowerEventState.Planned };

        TowerEvent[] currentTowerEvents = await _towerEventService.GetEvents(currentTowerEventsFilter);
        return currentTowerEvents;
    }

    private string AccessTypeToString(SpaceAccessType type)
    {
        switch ((int)type)
        {
            case 0: return "Открыта";
            case 1: return "Приватно";
            case 2: return "Закрыта";
            default: return "Открыта";
        }
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        //_updateCoroutine = StartCoroutine(UpdateLocatioonsCoroutine());
    }

    public override void HidePanel()
    {
        base.HidePanel();
        _errorPanel.HidePanel();
        _loadingPanel.HidePanel();
        if (_updateCoroutine != null)
            StopCoroutine(_updateCoroutine);
    }
}
