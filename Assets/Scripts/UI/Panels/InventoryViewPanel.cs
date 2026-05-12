using Cysharp.Threading.Tasks;
using Infrastructure.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Mechanics.PadKeyboard;
using Assets.Scripts.Trading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;
using Assets.Scripts.TowerObjects;
using Mechanics.UserWallet;
using Mechanics.UserWallet.Service;
using Mechanics.UserWallet.Model;

namespace UI
{
    public class BoughtItems
    {
        public TowerObjectDto TowerObject;
        public int Count;
    }

    public class InventoryViewPanel : GameObjectViewPanel, ITowerObjectRecyclingHandler<InventoryObject>
    {
        [SerializeField] private UserItem[] CommonItems;
        [SerializeField] private InventoryPlace[] Places;
        [SerializeField] private Button CollectAllButton;
        [SerializeField] private InventoryRecyclingArea RecyclingArea;
        [SerializeField] private FingerPhysicButton _physicsButtonUp;
        [SerializeField] private FingerPhysicButton _physicsButtonDown;
        [SerializeField] private XRSimpleInteractable _uiButtonUp;
        [SerializeField] private XRSimpleInteractable _uiButtonDown;

        private NetworkFactory _networkFactory;
        private IXRInteractor[] _interactors;
        private bool _grabBlocked;

        private ITowerObjectsService _towerObjectsService;
        private ITradeService _tradeService;
        private Transform _playerTransform;
        private IWalletService _walletService;
        private bool _initPlacesInProgress;
        private int _currentShelfNumber = 0;
        private int _shelfsCount = 1;

        [Inject]
        public void Construct(ITowerObjectsService towerObjectsService, ITradeService tradeService,
            IWalletService walletService)
        {
            _walletService = walletService;
            _towerObjectsService = towerObjectsService;
            _tradeService = tradeService;
        }

        private void Awake()
        {
            RecyclingArea.Init(this);
        }

        private void OnEnable()
        {
            _physicsButtonUp.OnPress += OnPressUp;
            _physicsButtonDown.OnPress += OnPressDown;
            _uiButtonUp.activated.AddListener((arg) => OnPressUp());
            _uiButtonDown.activated.AddListener((arg) => OnPressDown());

            foreach (var item in Places)
            {
                item.GrabStarted += Item_GrabStarted;
                item.GrabStopped += Item_GrabStopped;
            }

            CollectAllButton.onClick.AddListener(OnCollectAllButtonClick);
            _tradeService.TransactionCompleted += OnTradeTransactionCompleted;

            _walletService.WalletChanged += OnWalletChanged;
        }

        private void OnDisable()
        {
            _physicsButtonUp.OnPress -= OnPressUp;
            _physicsButtonDown.OnPress -= OnPressDown;
            _uiButtonUp.activated.RemoveAllListeners();
            _uiButtonDown.activated.RemoveAllListeners();

            foreach (var item in Places)
            {
                item.GrabStarted -= Item_GrabStarted;
                item.GrabStopped -= Item_GrabStopped;
            }

            CollectAllButton.onClick.RemoveListener(OnCollectAllButtonClick);
            _tradeService.TransactionCompleted -= OnTradeTransactionCompleted;

            if (_networkFactory != null)
            {
                _networkFactory.ItemDespawned -= OnItemDespawned;
                _networkFactory.PhysicModelInstantiated -= OnPhysicModelInstantiated;
            }

            _walletService.WalletChanged -= OnWalletChanged;
        }

        public void Init(IXRInteractor[] handControllers, Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _interactors = handControllers;

            //Vector3 center = _playerTransform.position;
            //for (int i = 0; i < Places.Length; i++)
            //{
            //    int a = i * 15;
            //    Vector3 pos = CirclePosition(center, 0.5f, a);
            //    Quaternion rot = Quaternion.FromToRotation(Vector3.up, center - pos);
            //    Places[i].transform.SetLocalPositionAndRotation(pos, rot);
            //}
        }

        public override void ShowPanel()
        {
            base.ShowPanel();

            //AssetPath.SetRemoteBasePath("https://storage.yandexcloud.net/st-scenes/dev");
            if (_networkFactory == null)
            {
                _networkFactory = FindObjectOfType<NetworkFactory>();
                if (_networkFactory == null)
                {
                    return;
                }
            }

            _networkFactory.ItemDespawned += OnItemDespawned;
            _networkFactory.PhysicModelInstantiated += OnPhysicModelInstantiated;

            CollectAllButton.gameObject.SetActive(_networkFactory.AreItemsSpawned);

            InitPlaces(_currentShelfNumber).Forget();
        }

        public override void HidePanel()
        {
            base.HidePanel();

            if (_networkFactory != null)
            {
                _networkFactory.ItemDespawned -= OnItemDespawned;
                _networkFactory.PhysicModelInstantiated -= OnPhysicModelInstantiated;
            }
        }

        private async void OnPressDown()
        {
            if (_currentShelfNumber == 0) _currentShelfNumber = _shelfsCount - 1;
            else _currentShelfNumber--;
            await InitPlaces(_currentShelfNumber);
        }

        private async void OnPressUp()
        {
            if (_currentShelfNumber == _shelfsCount - 1) _currentShelfNumber = 0;
            else _currentShelfNumber++;
            await InitPlaces(_currentShelfNumber);
        }

        public bool CanBeRecycled(InventoryObject towerObject)
        {
            if (towerObject == null)
            {
                return false;
            }
            return _networkFactory.IsItemSpawned(towerObject.TowerObjectDto);
        }

        public bool ProcessRecycling(InventoryObject towerObject)
        {
            if (towerObject == null)
            {
                return false;
            }
            return _networkFactory.TryDespawnItem(towerObject.TowerObjectDto);
        }

        private void OnWalletChanged()
        {
            InitPlaces(_currentShelfNumber).Forget();
        }

        private void OnCollectAllButtonClick()
        {
            if (_networkFactory != null)
            {
                _networkFactory.DespawnAllObjects();
            }
        }

        private void OnTradeTransactionCompleted()
        {
            InitPlaces(_currentShelfNumber).Forget();
        }

        private void Item_GrabStarted(GrabbingHand grabbingHand, InventoryPlace place)
        {
            if (place.UserItem == null)
            {
                return;
            }

            if (_grabBlocked)
            {
                return;
            }

            BlockDoubleItemGrabbingAsync().Forget();

            _networkFactory.CreateTowerObject(grabbingHand, place.UserItem,
                place.transform.position, place.transform.rotation);
        }

        private void Item_GrabStopped(InventoryPlace obj)
        {
            //TODO: Destroy object on grab stopped
        }

        private void OnPhysicModelInstantiated(GameObject model, TowerObjectDto itemInfo)
        {
            CollectAllButton.gameObject.SetActive(_networkFactory.AreItemsSpawned);

            InventoryObject inventoryObject = TryAddInventoryObjectComponent(model, itemInfo);
            TryAddObjectToSellComponent(inventoryObject, itemInfo);
        }

        private InventoryObject TryAddInventoryObjectComponent(GameObject go, TowerObjectDto itemInfo)
        {
            XRGrabInteractable grabInteractable = go.GetComponentInChildren<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                return AddInventoryObject(grabInteractable.gameObject, itemInfo);
            }

            IInventoryObjectGrabbable objectGrabbable = go.GetComponentInChildren<IInventoryObjectGrabbable>();
            if (objectGrabbable != null)
            {
                return AddInventoryObject(objectGrabbable.Transform.gameObject, itemInfo);
            }

            return null;
        }

        private void AddObjColliderComponents<T>(GameObject go, T obj)
        {
            var colliders = go.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
            {
                return;
            }

            List<GameObject> collidersGameObjects = new();
            foreach (Collider collider in colliders)
            {
                if (collidersGameObjects.Contains(collider.gameObject))
                {
                    continue;
                }

                collidersGameObjects.Add(collider.gameObject);

                if (obj is ObjectToSell)
                {
                    ObjectToSellCollider objToSellCollider =
                        collider.gameObject.AddComponent<ObjectToSellCollider>();
                    objToSellCollider.Init(obj as ObjectToSell);
                }
                else if (obj is InventoryObject)
                {
                    InventoryObjectCollider inventoryObjCollider =
                        collider.gameObject.AddComponent<InventoryObjectCollider>();
                    inventoryObjCollider.Init(obj as InventoryObject);
                }
            }
        }

        private InventoryObject AddInventoryObject(GameObject go, TowerObjectDto itemInfo)
        {
            if (!go.TryGetComponent(out InventoryObject inventoryObject))
            {
                inventoryObject = go.AddComponent<InventoryObject>();
                AddObjColliderComponents(go, inventoryObject);
            }

            inventoryObject.Init(itemInfo);
            return inventoryObject;
        }

        private async void TryAddObjectToSellComponent(InventoryObject inventoryObject, TowerObjectDto itemInfo)
        {
            if (inventoryObject == null)
            {
                return;
            }

            if (itemInfo.Id == Guid.Empty)
            {
                return;
            }

            // Если магазин принимает товар, то добавим товару компонент для продажи

            TowerObjectDto[] userItems = await _towerObjectsService.GetUserObjects();
            itemInfo = userItems.FirstOrDefault((x) => x.Id == itemInfo.Id);

            if (itemInfo == null)
            {
                return;
            }

            ShopDto[] shops = await _tradeService.GetShops();

            if (shops == null || shops.Length == 0 || shops[0].CommissionPrices == null)
            {
                return;
            }

            CommissionInfoDto itemCommission = shops[0].CommissionPrices.FirstOrDefault(
                (c) => c.ObjectClassId == itemInfo.ObjectClassId);

            if (itemCommission == null)
            {
                return;
            }

            AddObjectToSellComponent(inventoryObject.gameObject, itemInfo, itemCommission);
        }

        private void AddObjectToSellComponent(GameObject go, TowerObjectDto itemInfo,
            CommissionInfoDto itemCommission)
        {
            if (!go.TryGetComponent(out ObjectToSell objectToSell))
            {
                objectToSell = go.AddComponent<ObjectToSell>();
                AddObjColliderComponents(go, objectToSell);
            }

            objectToSell.Init(itemInfo, itemCommission);
        }

        private void OnItemDespawned(TowerObjectDto itemInfo)
        {
            CollectAllButton.gameObject.SetActive(_networkFactory.AreItemsSpawned);
        }

        private async UniTask InitPlaces(int shelfNumber)
        {
            if (_initPlacesInProgress)
            {
                await UniTask.WaitWhile(() => _initPlacesInProgress);
            }

            _initPlacesInProgress = true;

            foreach (var place in Places)
            {
                place.DeInit();
            }

            InitCoin(_currentShelfNumber); //coin loads without awaiting!

            await InitCustomItems(_currentShelfNumber); //objects on the user's shelf

            _initPlacesInProgress = false;
        }

        private void InitCoin(int shelfNumber)
        {
            for (int i = 0; i < Places.Length; i++)
            {
                int index = i + shelfNumber * Places.Length;

                if (index >= CommonItems.Length) break;

                SpawnCoin(Places[i], CommonItems[index]).Forget();
            }
        }

        private async UniTask InitCustomItems(int shelfNumber)
        {
            //this shelf is already busy with commonItems, there is nowhere to place boughtItems at that shelf
            if (shelfNumber * Places.Length + Places.Length <= CommonItems.Length) return; 

            var userObjects = await _towerObjectsService.GetUserObjects();

            var boughtItems = ConvertToBoughtItems(userObjects);

            _shelfsCount = (int)Math.Ceiling((decimal)(CommonItems.Length + boughtItems.Length) / Places.Length);

            if (_shelfsCount > 1)
            {
                _physicsButtonUp.gameObject.SetActive(true);
                _physicsButtonDown.gameObject.SetActive(true);
            }
            else
            {
                _physicsButtonUp.gameObject.SetActive(false);
                _physicsButtonDown.gameObject.SetActive(false);
            }

            //there are not enough items to load on this shelf
            if (CommonItems.Length + boughtItems.Length <= shelfNumber * Places.Length) return;

            int currentPlaceIndex;

            if (CommonItems.Length <= shelfNumber * Places.Length) currentPlaceIndex = 0;
            else currentPlaceIndex = CommonItems.Length - shelfNumber * Places.Length;

            int currentBoughtIndex;

            if (CommonItems.Length >= shelfNumber * Places.Length) currentBoughtIndex = 0;
            else currentBoughtIndex = shelfNumber * Places.Length - CommonItems.Length;

            for (int i = currentBoughtIndex; i < boughtItems.Length; i++)
            {
                if (currentPlaceIndex >= Places.Length)
                {
                    break;
                }

                GameObject prefab = null;
                if (boughtItems[i].TowerObject.LoadingObjectType == LoadingObjectType.Remote)
                {
                    if (boughtItems[i].TowerObject.RemoteObjectTypeInfo == null)
                    {
                        Debug.LogError("TowerObject.RemoteObjectTypeInfo == null");
                        continue;
                    }

                    prefab = await _networkFactory.GetRemoteModel(boughtItems[i].TowerObject.RemoteObjectTypeInfo);
                }
                else if (boughtItems[i].TowerObject.LoadingObjectType == LoadingObjectType.Prefab)
                {
                    prefab = _networkFactory.GetLocalPrefab(boughtItems[i].TowerObject);
                }

                if (prefab == null)
                {
                    continue;
                }

                Places[currentPlaceIndex].Init(_interactors, prefab, boughtItems[i]);

                currentPlaceIndex++;
            }
            Debug.LogWarning($"999 End init custom objects");
        }

        private BoughtItems[] ConvertToBoughtItems(TowerObjectDto[] items)
        {
            var dictionary = new Dictionary<TowerObjectDto, int>(new TowerObjectDtoComparer());

            foreach (TowerObjectDto item in items)
            {
                if (dictionary.ContainsKey(item)) dictionary[item] += 1;
                else dictionary.Add(item, 1);
            }

            BoughtItems[] result = new BoughtItems[dictionary.Count];

            int i = 0;

            foreach (var item in dictionary)
            {
                result[i] = new BoughtItems
                {
                    TowerObject = item.Key,
                    Count = item.Value
                };
                i++;
            }

            return result;
        }

        //private Vector3 CirclePosition(Vector3 center, float radius, int a)
        //{
        //    Debug.Log(a);
        //    float ang = a;
        //    Vector3 pos;
        //    pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        //    pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        //    pos.z = center.z;
        //    return pos;
        //}

        private async UniTask InitPlace(InventoryPlace place, UserItem userItem)
        {
            if (userItem.ItemModelPrefab == null)
            {
                if (userItem.RemoteObjectTypeInfo == null)
                {
                    Debug.LogError(
                        "userItem.ItemModelPrefab == null but userItem.ItemType != UserItemType.RemoteObject");
                    return;
                }

                userItem.ItemModelPrefab = await _networkFactory.GetRemoteModel(userItem.RemoteObjectTypeInfo);
            }

            var towerObject = userItem.ConvertToBoughtItems();
            place.Init(_interactors, userItem.ItemModelPrefab, towerObject);
        }

        private async UniTask BlockDoubleItemGrabbingAsync()
        {
            _grabBlocked = true;
            await UniTask.WaitForFixedUpdate();
            _grabBlocked = false;
        }

        private async UniTask<bool> CheckAvailableItemInInventory(UserItem userItem)
        {
            if (userItem.TowerObjectClassName != NetworkFactory.TwrCoinObjectClassName)
            {
                return true;
            }

            TwrWallet userWallet = await _walletService.GetMyWalletAsync();
            if (userWallet == null)
            {
                return false;
            }

            return userWallet.sum > 0;
        }

        private async  UniTask SpawnCoin(InventoryPlace inventoryPlace, UserItem userItem)
        {
            TwrWallet userWallet = await _walletService.GetMyWalletAsync();

            if (userWallet == null) return;

            if (userWallet.sum > 0) await InitPlace(inventoryPlace, userItem);
        }
    }
}