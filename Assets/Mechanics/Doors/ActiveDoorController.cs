using System;
using Assets.Scripts.Hall;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Linq;
using Assets.Scripts.Space;
using Assets.Scripts.API;
using Assets.Scripts.Event;
using Assets.Scripts.Cinema;
using System.Collections.Generic;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Space.SpaceSettings;

namespace Assets.Mechanics.Doors
{
    public class ActiveDoorController : MonoBehaviour
    {
        private const float RefreshingDoorsPeriod = 10;

        [SerializeField] private Transform ActiveDoorsContent;
        [SerializeField] private SceneChangerView SceneChangerView;
        [SerializeField] private int HallId = 0;

        private IHallsService _hallService;
        private IClientData _clientData;
        private ITowerEventService _towerEventService;
        private ICinemaService _cinemaService;
        private IRegistrationInSpacesService _registrationInSpacesService;

        private ActiveDoor[] _doors;
        private bool _doorsAreInitialized;
        private TowerEventsFilter _currentTowerEventsFilter;

        public int CurrentHallId => HallId;
        public bool DoorsAreInitialized => _doorsAreInitialized;

        [Inject]
        public void Init(IClientData clientData, IHallsService hallService,
            ITowerEventService towerEventService, ICinemaService cinemaService,
            IRegistrationInSpacesService registrationInSpacesService)
        {
            _clientData = clientData;
            _hallService = hallService;
            _towerEventService = towerEventService;
            _cinemaService = cinemaService;
            _registrationInSpacesService = registrationInSpacesService;

#if !UNITY_SERVER
            InitSpaces().Forget();
#endif
        }

        private void Awake()
        {
            _doors = ActiveDoorsContent.GetComponentsInChildren<ActiveDoor>();
            if (SceneChangerView == null)
            {
                SceneChangerView = FindObjectOfType<SceneChangerView>();
            }

            foreach (var door in _doors)
            {
                door.Init(SceneChangerView);
            }

            _currentTowerEventsFilter = new();
            _currentTowerEventsFilter.FromMinusSecondsNow = 300; // 5 minuts after previous event ended
            _currentTowerEventsFilter.UpToPlusSecondsNow = 900; // 15 minutes before next event started
            _currentTowerEventsFilter.States = new[] { TowerEventState.Planned };
        }

        public async UniTask<ActiveDoor> FindDoor(string spaceId)
        {
            await UniTask.WaitUntil(() => _doorsAreInitialized);

            return _doors.FirstOrDefault((d) => d.SpaceId == spaceId);
        }

        public async UniTask<ActiveDoor> FindDoor(SpaceType spaceType)
        {
            await UniTask.WaitUntil(() => _doorsAreInitialized);

            return _doors.FirstOrDefault((d) => d.SpaceType == spaceType && !d.IsPrivate);
        }

        private async UniTask InitSpaces()
        {
            //Hall[] halls = await _hallService.GetHalls();
            //if (halls != null && halls.Length > HallId)
            //{
            //    SetSpacesToDoors(halls[HallId]);
            //}

            //_doorsAreInitialized = true;

            RefreshingDoorsRoutine().Forget();
        }

        private async UniTaskVoid RefreshingDoorsRoutine()
        {
            TimeSpan refreshingDelay = TimeSpan.FromSeconds(RefreshingDoorsPeriod);
            while (true)
            {
                if (this == null)
                {
                    break;
                }

                Hall[] halls = await _hallService.GetHalls();
                if (halls == null || halls.Length <= HallId)
                {
                    Debug.LogError($"There is no Hall {HallId} in the source");
                    await UniTask.Delay(refreshingDelay);
                    continue;
                }

                if (halls[HallId] == null || halls[HallId].Spaces == null)
                {
                    await UniTask.Delay(refreshingDelay);
                    continue;
                }

                TowerEvent[] currentTowerEvents = await _towerEventService.GetEvents(_currentTowerEventsFilter);

                string[] spaceIds = halls[HallId].Spaces.Select(x => x.Id.ToString()).ToArray();
                AccessResultDto[] doorsAccesses = await _registrationInSpacesService.CheckAccess(spaceIds);

                //List<string> spaceIdsPart = new();
                //List<AccessResultDto> doorsAccessesList = new();
                //for (int i = 0; i < halls[HallId].Spaces.Length; i++)
                //{
                //    spaceIdsPart.Add(halls[HallId].Spaces[i].Id.ToString());
                //    if (spaceIdsPart.Count >= 5 || i >= halls[HallId].Spaces.Length - 1)
                //    {
                //        AccessResultDto[] doorsAccessesPart = await _registrationInSpacesService.CheckAccess(spaceIdsPart.ToArray());
                //        doorsAccessesList.AddRange(doorsAccessesPart);
                //        spaceIdsPart.Clear();
                //    }
                //}
                //AccessResultDto[] doorsAccesses = doorsAccessesList.ToArray();

                List<string> adminSpacesOfLocalClient = await GetAdminSpacesOfLocalClient();
                
                SetSpacesToDoors(halls[HallId], currentTowerEvents, doorsAccesses, adminSpacesOfLocalClient);

                _doorsAreInitialized = true;

                await UniTask.Delay(refreshingDelay);
            }
        }

        private async UniTask<List<string>> GetAdminSpacesOfLocalClient()
        {
            if (!_clientData.UserId.HasValue)
            {
                return null;
            }

            Cinema[] cinemas = await _cinemaService.GetCinemas();

            if (cinemas == null)
            {
                return null;
            }

            List<string> adminSpaces = new();
            foreach (var cinema in cinemas)
            {
                if (cinema.Space == null || cinema.Administrators == null)
                {
                    continue;
                }

                if (cinema.Administrators.FirstOrDefault(
                    (user) => user.Id.Equals(_clientData.UserId.Value)) != null)
                {
                    adminSpaces.Add(cinema.Space.Id.ToString());
                }
            }

            return adminSpaces;
        }

        private void SetSpacesToDoors(Hall hall, TowerEvent[] currentTowerEvents = null, 
            AccessResultDto[] doorsAccesses = null, List<string> adminSpacesOfLocalClient = null)
        {
            List<string> resultAdminSpacesOfLocalClient = new();
            if(adminSpacesOfLocalClient != null)
                resultAdminSpacesOfLocalClient.AddRange(adminSpacesOfLocalClient);
            foreach (var door in _doors)
            {
                if (door.IsPrivate)
                {
                    LocalSpace myPlace = null;
                    foreach (var item in hall.Spaces)
                    {
                        if (item != null && item.Number == door.NumberInHall)
                        {
                            myPlace = item;
                            break;
                        }
                    }
                    if (myPlace != null)
                    {
                        if (myPlace.AdminIds.Contains(_clientData.UserId.ToString()))
                            resultAdminSpacesOfLocalClient.Add(myPlace.Id.ToString());
                        //bool isOwnerInPlace = true;
                        //if (usersInSpaces != null && myPlace.SpaceOwner != null)
                        //{
                        //    UserInSpaceInfoDto ownerInSpace =
                        //        usersInSpaces.FirstOrDefault(
                        //            (item) => item.SpaceId == myPlace.Id && item.UserId == myPlace.SpaceOwner.UserId);
                        //    isOwnerInPlace = ownerInSpace != null;
                        //}
                        door.SetMySpace(myPlace, GetDoorAccess(doorsAccesses, myPlace.Id));
                        if (myPlace.IsForSale != null && (bool)myPlace.IsForSale) 
                            door.SetDoorFrame(DoorFrameType.Free);
                    }
                    else
                    {
                        door.SetEmptyMySpace();
                    }
                }
                else
                {
                    LocalSpace localSpace = hall.Spaces.FirstOrDefault(
                        (d) => d.SpaceType == door.SpaceType && d.IsPrivate == door.IsPrivate && d.Number == door.NumberInHall);

                    if (localSpace == null)
                    {
                        door.SetEmptySpace();
                    }
                    else
                    {
                        door.SetLocalSpace(localSpace, GetDoorAccess(doorsAccesses, localSpace.Id));
                    }
                }
            }

            SetLocalUserAdminStateForDoors(resultAdminSpacesOfLocalClient);
            SetCurrentTowerEventsForDoors(currentTowerEvents);
        }

        private AccessResultDto GetDoorAccess(AccessResultDto[] doorsAccesses, Guid spaceId)
        {
            if (doorsAccesses == null)
            {
                return null;
            }
            return doorsAccesses.FirstOrDefault(x => x.SpaceId.HasValue && x.SpaceId.Value == spaceId);
        }

        private void SetLocalUserAdminStateForDoors(List<string> adminSpacesOfLocalClient)
        {
            foreach (var door in _doors)
            {
                door.RefreshAdminStateByUserAdminSpaces(adminSpacesOfLocalClient);
            }
        }

        private void SetCurrentTowerEventsForDoors(TowerEvent[] currentTowerEvents)
        {
            if (currentTowerEvents == null)
            {
                foreach (var door in _doors)
                {
                    door.SetTowerEvent(null);
                }
                return;
            }

            foreach (var door in _doors)
            {
                if (string.IsNullOrEmpty(door.SpaceId))
                {
                    door.SetTowerEvent(null);
                    continue;
                }

                TowerEvent currentDoorTowerEvent = null;
                foreach (var towerEvent in currentTowerEvents)
                {
                    if (towerEvent.Space == null || !towerEvent.Space.Id.Equals(Guid.Parse(door.SpaceId)))
                    {
                        continue;
                    }
                    // Will take the latest tower event of current events
                    if (currentDoorTowerEvent == null || currentDoorTowerEvent.From < towerEvent.From)
                    {
                        currentDoorTowerEvent = towerEvent;
                    }
                }

                door.SetTowerEvent(currentDoorTowerEvent);
            }
        }
    }
}
