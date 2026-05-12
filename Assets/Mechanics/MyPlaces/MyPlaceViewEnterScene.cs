using System;
using Assets.Scripts.Hall;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.Mechanics.MyPlaces;
using Assets.Scripts.API;
using Assets.Scripts.Space;
using UnityEngine;
using Zenject;

public class MyPlaceViewEnterScene : MonoBehaviour
{
    [SerializeField] private RectTransform _apartmentListUI;
    [SerializeField] private PlaceItemUI _itemPrefab;
    [SerializeField] private MainPanel _mainPanel;
    private DiContainer _diContainer;
    private IMyPlaceService _myPlaceService = new MyPlaceService();
    private LocalSpace[] _mySpaces;
    private IApiService _apiService;

    [Inject]
    public async void Init(IApiService service, IMyPlaceService myPlaceService, DiContainer diContainer)
    {
        _apiService = service;
        _diContainer = diContainer;
        _myPlaceService = myPlaceService;
        _apiService.AuthSuccess += RefreshMySpaces;
        RefreshMySpaces();
    }

    private void Start()
    {
        _mainPanel.PanelRequest += RefreshMySpaces;
    }

    private void OnDestroy()
    {
        _apiService.AuthSuccess -= RefreshMySpaces;
        _mainPanel.PanelRequest -= RefreshMySpaces;
    }

    private async void RefreshMySpaces()
    {
        _mySpaces = await _myPlaceService.GetAllMySpaces();
        GetMySpaces(_mySpaces);
    }

    private void GetMySpaces(LocalSpace[] mySpaces)
    {
        for (int i = 0; i < _apartmentListUI.childCount; i++)
        {
            Destroy(_apartmentListUI.GetChild(i).gameObject);
        }
        
        if (mySpaces != null && mySpaces.Length > 0)
        {
            foreach (var space in mySpaces)
            {
                CreateSpaceUi(_apartmentListUI, space);
            }
        }
        else
        {
            //show message "No My Places!"
        }
    }

    private void CreateSpaceUi(RectTransform parent, LocalSpace myPlace)
    {
        var item = _diContainer.InstantiatePrefab(_itemPrefab, parent);
        PlaceItemUI placeItemUI = item.GetComponent<PlaceItemUI>();
        placeItemUI.SetName(myPlace.SpaceName);
        
        placeItemUI.SetSpaceType(myPlace.SpaceType);
        placeItemUI.SetSpaceId(myPlace.Id.ToString());
    }
}
