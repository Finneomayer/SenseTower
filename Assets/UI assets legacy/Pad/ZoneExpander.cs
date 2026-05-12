using Assets.Localization;
using Assets.Scripts.Player;
using Assets.Scripts.Zones;
using Oculus.Platform;
using Sense.Interectable.Teleportation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Assets.Mechanics.Mafia.Table;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

[Serializable]
public class ZoneVariant
{
    public List<GameObject> Models;
    public List<Place> Places;
}

public class ZoneExpander : NetworkBehaviour
{
    [SerializeField] private List<ZoneVariant> _zoneVariants;
    [SerializeField] private ZoneExpanderView _zoneExpanderView;
    //[SerializeField] private List<GameObject> _modelsOfStandartZone;
    //[SerializeField] private List<GameObject> _modelsOfBigZone;
    //[SerializeField] private List<Place> _placesOfStandartZone;
    //[SerializeField] private List<Place> _placesOfBigZone;

    [SerializeField] private LocalizationVariant _expandingInProgressLocalizationVariant;

    public NetworkVariable<int> ZoneSize;
    public event Action<Transform> ZoneSizeChangingStart;
    public event Action<Transform> ZoneSizeChangingEnd;

    private ZonesModel _zonesModel;
    private ZoneController _zoneController;
    private PlayerLogic _player;
    //private SceneChangerView _sceneChanger;
    private OnPlayerUI _onPlayerUI;
    //private TMP_Text _loadingText;

    private Coroutine _changeZoneObjectsRoutine;

    private void Awake()
    {
        _zonesModel = GetComponentInParent<ZonesModel>();
        _zoneController = GetComponent<ZoneController>();
        _onPlayerUI = FindObjectOfType<OnPlayerUI>();
    }

    public override void OnNetworkSpawn()
    {
#if !UNITY_SERVER
        SwitchZoneObjectsAtStart();
#endif
    }

    private void OnEnable()
    {
        _zoneExpanderView.ChangeSize += SetZoneSize;
        ZoneSize.OnValueChanged += ChangeZoneObjects;
        //_sceneChanger.PlayerInited += SwitchZoneObjectsAtStart;
    }
    private void ChangeZoneObjects(int oldv, int newv)
    {
        _player = _zonesModel.PlayerOwner;
#if !UNITY_SERVER
        _zoneExpanderView.SelectVariant(newv);

        StartCoroutine(ChangeZoneObjectsRoutine(oldv, newv));
#endif
    }

    private void SwitchPlaces(List<Place> places, bool flag)
    {
        if (places.Count == 0) return;
        foreach (var place in places)
        {
            place.SwitchPlace(flag);
            if (flag && _zonesModel.GetTeleportationProvider() != null)
            {
                place.GetComponent<CustomTeleportationAnchor>().teleportationProvider = _zonesModel.GetTeleportationProvider();
            }
        }
    }
    private void SwitchModels(List<GameObject> models, bool flag)
    {
        if (models.Count == 0) return;
        foreach (var model in models)
        {
            model.SetActive(flag);
        }
    }

    private void SwitchZoneObjectsAtStart()
    {
        foreach (var zone in _zoneVariants)
        {
            SwitchPlaces(zone.Places, false);
            SwitchModels(zone.Models, false);
        }

        SwitchPlaces(_zoneVariants[ZoneSize.Value].Places, true);
        SwitchModels(_zoneVariants[ZoneSize.Value].Models, true);

        _zoneExpanderView.SelectVariant(ZoneSize.Value);      
    }

    private void OnDisable()
    {
        _zoneExpanderView.ChangeSize -= SetZoneSize;
        ZoneSize.OnValueChanged -= ChangeZoneObjects;
        //_sceneChanger.PlayerInited -= SwitchZoneObjectsAtStart;
        if (_changeZoneObjectsRoutine != null) StopCoroutine(_changeZoneObjectsRoutine);
    }
    
    private IEnumerator ChangeZoneObjectsRoutine(int oldv, int newv)
    {
        Task fadingTask = _onPlayerUI.FadeToBlackDefault(_expandingInProgressLocalizationVariant.Localize()).AsTask();
        yield return new WaitUntil(() => fadingTask.IsCompleted);

        ZoneSizeChangingStart?.Invoke(_player.transform);

        SwitchPlaces(_zoneVariants[newv].Places, true);
        SwitchModels(_zoneVariants[newv].Models, true);
        SwitchModels(_zoneVariants[oldv].Models, false);
        
        if (newv == 0)
        {
            yield return StartCoroutine(ReplaceMeToSmallerZone(_zoneVariants[0].Places));
            yield return new WaitForSeconds(0.1f);
        }
        else if (newv == 1)
        {
            ReplaceMeToBiggerZone(_zoneVariants[1].Places);
            yield return new WaitForSeconds(0.1f);
        }

        SwitchPlaces(_zoneVariants[oldv].Places, false);

        ZoneSizeChangingEnd?.Invoke(_player.transform);
        
        _onPlayerUI.FadeToTransparent().Forget();
    }

    private void ReplaceMeToBiggerZone(List<Place> places)
    {
        var myId = _zonesModel.OwnerId;
        int myPlaceNumber = -1;

        for (int i = 0; i < _zoneVariants[0].Places.Count; i++) //search my place number in standart zone
        {
            if (_zoneVariants[0].Places[i].IsOccupiedID.Value == myId)
            {
                myPlaceNumber = i;
                break;
            }
        }
        //and put me on the place with the same number in big zone

        if (myPlaceNumber != -1)
        {
            places[myPlaceNumber].LeaveAndOccupyPlaceExternal(places[myPlaceNumber]);

            ResetPlayerPosition(places[myPlaceNumber]);
        }        

        foreach (var place in _zoneVariants[1].Places)
        {
            place.HideSignal();
        }
    }

    private IEnumerator ReplaceMeToSmallerZone(List<Place> places)
    {
        if (!_zoneController.IsMeAdmin()) yield return new WaitForSeconds(1f); //if I'm not an admin, I have to wait 1 sec for the admin to sit down

        var myId = _zonesModel.OwnerId;
        int myPlaceNumber = -1;

        for (int i = 0; i < _zoneVariants[1].Places.Count; i++) //search my place number in big zone
        {
            if (_zoneVariants[1].Places[i].IsOccupiedID.Value == myId)
            {
                myPlaceNumber = i;
                break;
            }
        }

        if (myPlaceNumber != -1)
        {
            if (myPlaceNumber > 5) myPlaceNumber -= 6;

            //and put me on the place with the same number in big zone
            if (places[myPlaceNumber].IsOccupiedID.Value != 0) //if target place is already occupied
            {
                yield return new WaitForSeconds(myPlaceNumber * 0.1f);

                bool foundFreePlace = false;
                foreach (var place in places) //searching free places
                {
                    if (!foundFreePlace && place.IsOccupiedID.Value == 0)
                    {
                        place.LeaveAndOccupyPlaceExternal(place);

                        ResetPlayerPosition(places[myPlaceNumber]);

                        foundFreePlace = true;
                    }
                }

                if (!foundFreePlace) //if no free places, set player to occupied place (!)
                                     //it's needed to autokick player from zone
                {
                    places[myPlaceNumber].LeaveAndOccupyPlaceExternal(places[myPlaceNumber]);
                }
            }
            else
            {
                places[myPlaceNumber].LeaveAndOccupyPlaceExternal(places[myPlaceNumber]);

                ResetPlayerPosition(places[myPlaceNumber]);
            }

            foreach (var place in _zoneVariants[0].Places)
            {
                place.HideSignal();
            }
        }

        yield return null;
    }

    private void ResetPlayerPosition(Place targetPlace)
    {
        if (_player != null)
        {
            _player.transform.position = targetPlace.TeleportAnchor.TeleportAnchorTransform.position;
            _player.transform.rotation = targetPlace.TeleportAnchor.TeleportAnchorTransform.rotation;
        }
    }

    private void SetZoneSize(int obj)
    {
        SetZoneSizeServerRPC(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetZoneSizeServerRPC(int size)
    {
        ZoneSize.Value = size;
    }
}
