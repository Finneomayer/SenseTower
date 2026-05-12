using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using Assets.Scripts.TowerObjects;
using Assets.Scripts.Trading;
using Cysharp.Threading.Tasks;
using Infrastructure.Factory;
using Mechanics.UserWallet.Service;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Assets.Mechanics.Shop.Scripts
{
    public class ShopItemsView : MonoBehaviour, ITowerObjectRecyclingHandler<InventoryObject>
    {
        [SerializeField]
        public Transform ShopPlacesContent;
        [SerializeField]
        public InventoryRecyclingArea RecyclingArea;
        [SerializeField] private WinClientBuyController _winClientBuyAbility;

        public event Action<ShopItemDto> ItemGrab;
        public event Action ReturnToShop;
        public event Action ShopInitialize;

        private ShopItemPlace[] _itemPlaces;
        private NetworkFactory _networkFactory;
        private DiContainer _diContainer;
        private ITradeService _tradeService;
        private IWalletService _walletService;
        private IXRInteractor[] _interactors;
        private Transform _startGrab;

        private ShopItemPlace _currentShopItemPlace;
        private bool isFirstInit = true;

        [Inject]
        private void Construct(DiContainer diContainer, ITradeService tradeService, IWalletService walletService)
        {
            _diContainer = diContainer;
            _tradeService = tradeService;
            _walletService = walletService;
        }

        private void Awake()
        {
            _itemPlaces = ShopPlacesContent.GetComponentsInChildren<ShopItemPlace>();
            RecyclingArea.Init(this);

            _winClientBuyAbility.Init(_tradeService, _walletService, _itemPlaces);
        }

        private void OnEnable()
        {
            foreach (var item in _itemPlaces)
            {
                item.GrabStarted += OnItemGrabStarted;
                item.ItemReturnToShop += OnReturnToShop;
            }
        }

        private void OnDisable()
        {
            foreach (var item in _itemPlaces)
            {
                item.GrabStarted -= OnItemGrabStarted;
                item.ItemReturnToShop -= OnReturnToShop;
            }
        }

        public async void Init(ShopDto shop, int[] pickedIds)
        {
            var compositionRoot = FindObjectOfType<CompositionRootNetworkScene>();
            if (compositionRoot != null)
            {
                PlayerLogic player = await compositionRoot.GetLocalPlayerAsync();
                _interactors = new IXRInteractor[2];
                _interactors[0] = player.GetLeftArm().RayInteractor;
                _interactors[1] = player.GetRightArm().RayInteractor;
            }

            if (_networkFactory == null)
            {
                _networkFactory = FindObjectOfType<NetworkFactory>();
                if (_networkFactory == null)
                {
                    return;
                }
            }

            _currentShopItemPlace = null;
            InitPlaces(shop, pickedIds);
        }

        public void DisablePlaceNumberObject(int placeNumber)
        {
            foreach (ShopItemPlace shopItemPlace in _itemPlaces)
            {
                if (shopItemPlace.ShopPlaceNumber == placeNumber)
                {
                    shopItemPlace.ToogleModelView(false);
                }
            }
        }

        public ShopItemPlace GetObjectPlace(TowerObjectDto towerObjectDto)
        {
            if (towerObjectDto == null)
            {
                return null;
            }
            return _itemPlaces.FirstOrDefault(item => item.ShopItemDto != null
                && item.ShopItemDto.Item == towerObjectDto);
        }

        public bool CanBeRecycled(InventoryObject towerObject)
        {
            if (towerObject == null)
            {
                return false;
            }

            return GetObjectPlace(towerObject.TowerObjectDto) != null;
        }

        public bool ProcessRecycling(InventoryObject towerObject)
        {
            if (towerObject == null)
            {
                return false;
            }
            ShopItemPlace shopItemPlace = GetObjectPlace(towerObject.TowerObjectDto);
            if (shopItemPlace != null)
            {
                shopItemPlace.ReturnToShop();
                return true;
            }
            return false;
        }

        private void OnReturnToShop()
        {
            ReturnToShop?.Invoke();
        }

        private void OnEmptyPrefabInstantiate(GameObject emptyPrefab, int objectId, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                if(_currentShopItemPlace != null)
                    _currentShopItemPlace.CreateObjectToBuy(emptyPrefab, _startGrab);
                
                _currentShopItemPlace = null;
            }
            else
            {
                Debug.Log("set vizuals");
                foreach (ShopItemPlace shopItemPlace in _itemPlaces)
                {
                    if (shopItemPlace.ShopPlaceNumber == objectId)
                    {
                        shopItemPlace.CreateVisualObject(emptyPrefab);
                        break;
                    }
                }
            }
        }

        private void OnItemGrabStarted(ShopItemPlace place, Transform handTransform)
        {
            if (_currentShopItemPlace != null)
                return;

            _startGrab = handTransform;
            _currentShopItemPlace = place;

            ItemGrab?.Invoke(place.ShopItemDto);

            _networkFactory.SpawnEmptyPrefabWithIdServerRpc(NetworkManager.Singleton.LocalClientId,
                place.ShopPlaceNumber, place.transform.position);

            Debug.LogWarning("create in place");

            foreach (ShopItemPlace itemPlace in _itemPlaces)
            {
                itemPlace.SetEnabledInteraction(false);
            }
        }

        private async void InitPlaces(ShopDto shop, int[] pickedIds)
        {
            Debug.Log("Init places");
            if (shop == null || shop.Items == null)
            {
                return;
            }

            Debug.Log(shop.Items.Length);
            List<UniTask> tasks = new List<UniTask>();
            foreach (var item in shop.Items)
            {
                Debug.Log($"add shop item: place:{item.PlaceNumber} name:{item.Item.Name}");
                ShopItemPlace place = _itemPlaces.FirstOrDefault(
                    (x) => x.ShopPlaceNumber == item.PlaceNumber);
                if (place != null)
                {
                    if (isFirstInit)
                        tasks.Add(InitPlace(place, item));
                    else
                    {
                        if (!pickedIds.Contains(item.PlaceNumber))
                            tasks.Add(InitPlace(place, item));
                    }
                }
            }

            await UniTask.WhenAll(tasks);

            if (isFirstInit)
            {
                ShopInitialize?.Invoke();
                _networkFactory.EmptyPrefabInstantiate += OnEmptyPrefabInstantiate;
                isFirstInit = false;
                _networkFactory.GetEmptyPrefabServerRpc();
            }
        }

        private async UniTask InitPlace(ShopItemPlace place, ShopItemDto itemDto)
        {
            Debug.LogWarning($"Try init place {place.ShopPlaceNumber}");

            place.DeInit();
            if (itemDto.Item == null)
            {
                return;
            }

            GameObject prefab = null;
            if (itemDto.Item.LoadingObjectType == LoadingObjectType.Remote)
            {
                prefab = await _networkFactory.GetRemoteModel(itemDto.Item.RemoteObjectTypeInfo);
            }
            else if (itemDto.Item.LoadingObjectType == LoadingObjectType.Prefab)
            {
                prefab = _networkFactory.GetLocalPrefab(itemDto.Item);
            }

            if (prefab == null)
            {
                Debug.LogError($"Can't get prefab for {itemDto.Item.Name} at {place.ShopPlaceNumber} {itemDto.Item.RemoteObjectTypeInfo?.ObjectRepositoryUrl}");
                return;
            }

            place.Init(_interactors, prefab, itemDto);
        }
    }
}