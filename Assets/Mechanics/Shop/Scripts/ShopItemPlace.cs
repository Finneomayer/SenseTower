using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Trading;
using Cysharp.Threading.Tasks;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Assets.Mechanics.Shop.Scripts
{
    public class ShopItemPlace : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private int PlaceNumber;
        [SerializeField] private BoxCollider PlaceCollider;
        [SerializeField] private InventoryPlaceGrabbable Grabbable;
        [SerializeField] private ShopPlaceView PlaceInfo;

        private DiContainer _diContainer;

        private ShopItemDto _shopItemDto;
        private GameObject _shopItemPrefab;
        private GameObject _shopItemModel;
        private ObjectToBuy _objectToBuyInstance;
        private GameObject _networkObjectTransmitter;
        public ShopItemDto ShopItemDto => _shopItemDto;
        public int ShopPlaceNumber => PlaceNumber;
        public InventoryPlaceGrabbable PlaceGrabbable => Grabbable;

        public event Action<ShopItemPlace, Transform> GrabStarted;
        public event Action ItemReturnToShop;
        public event Action<ShopItemPlace> RequestToBuy;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void OnEnable()
        {
            Grabbable.GrabStarted += Grabbable_GrabStarted;
        }

        private void OnDisable()
        {
            Grabbable.GrabStarted -= Grabbable_GrabStarted;
        }

        public void Init(IXRInteractor[] interactors, GameObject prefab, ShopItemDto shopItemDto)
        {
            Debug.LogWarning($"Init place {PlaceNumber} with {shopItemDto.Item?.RemoteObjectTypeInfo?.ObjectRepositoryUrl}");

            SetEnabledInteraction(true);

            PlaceInfo.Init(this, PlaceCollider, interactors);
            if (_shopItemModel != null)
            {
                Destroy(_shopItemModel);
            }

            _shopItemPrefab = prefab;
            _shopItemDto = shopItemDto;
            _shopItemModel = _diContainer.InstantiatePrefab(_shopItemPrefab, new Vector3(0, 10000, 0),
                PlaceCollider.transform.rotation, transform);

            PlaceItemFitter.PlaceModel(_shopItemModel, PlaceCollider).Forget();
        }

        public void SetEnabledInteraction(bool enabled)
        {
            PlaceGrabbable.enabled = enabled;
            PlaceCollider.enabled = enabled;
        }

        public void DeInit()
        {
            if (_networkObjectTransmitter != null &&
                _networkObjectTransmitter.TryGetComponent(out NetworkItemType networkItem))
            {
                networkItem.DespawnCurrentObjectServerRpc();
            }

            PlaceInfo.DeInit();

            _shopItemPrefab = null;
            if (_shopItemModel != null)
            {
                Destroy(_shopItemModel);
                _shopItemModel = null;
            }

            _shopItemDto = null;

            if (_objectToBuyInstance != null)
            {
                Destroy(_objectToBuyInstance.gameObject);
                _objectToBuyInstance = null;
            }
        }

        public void CreateObjectToBuy(GameObject parentObj, Transform startTransform)
        {
            if (_shopItemPrefab == null || _shopItemModel == null)
            {
                return;
            }

            if (_objectToBuyInstance != null)
            {
                return;
            }

            GameObject go;

            if (startTransform != null)
            {
                go = _diContainer.InstantiatePrefab(_shopItemPrefab,
                    startTransform.position, startTransform.rotation, null);
            }
            else
            {
                go = _diContainer.InstantiatePrefab(_shopItemPrefab,
                    PlaceCollider.transform.position, PlaceCollider.transform.rotation, null);
            }

            _networkObjectTransmitter = parentObj;
            PlaceItemFitter.ClearModel(go);

            var colliders = go.GetComponentsInChildren<Collider>().Where((c) => !c.isTrigger).ToList();
            if (colliders.Count == 0)
            {
                Debug.LogError($"{go} doesn't have colliders");
                return;
            }

            if (!go.TryGetComponent(out Rigidbody rb))
            {
                rb = go.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.isKinematic = true;

            var xrGrabInteractable = go.AddComponent<XRGrabInteractable>();
            xrGrabInteractable.interactionLayers = InteractionLayerMask.GetMask("Grab");
            xrGrabInteractable.throwOnDetach = false;
            xrGrabInteractable.useDynamicAttach = true;
            xrGrabInteractable.matchAttachPosition = true;
            xrGrabInteractable.matchAttachRotation = true;

            if (parentObj.TryGetComponent(out NetworkItemType networkItem))
            {
                networkItem.SetAimObject(go.transform);
            }

            _objectToBuyInstance = go.AddComponent<ObjectToBuy>();
            _objectToBuyInstance.Init(this, go);
            ToogleModelView(false);

            var inventoryObject = go.AddComponent<InventoryObject>();
            inventoryObject.Init(_shopItemDto.Item);

            List<GameObject> collidersGameObjects = new();
            foreach (Collider collider in colliders)
            {
                if (collidersGameObjects.Contains(collider.gameObject))
                {
                    continue;
                }
                collidersGameObjects.Add(collider.gameObject);

                ObjectToBuyCollider objToBuyCollider = collider.gameObject.AddComponent<ObjectToBuyCollider>();
                objToBuyCollider.Init(_objectToBuyInstance);

                InventoryObjectCollider inventoryObjectCollider = collider.gameObject.AddComponent<InventoryObjectCollider>();
                inventoryObjectCollider.Init(inventoryObject);

                collider.gameObject.layer = 20;
            }           
        }

        public void CreateVisualObject(GameObject parentObj)
        {
            if (_shopItemPrefab == null || _shopItemModel == null)
            {
                return;
            }

            if (_networkObjectTransmitter != null)
            {
                return;
            }

            parentObj.transform.position = PlaceCollider.transform.position;
            parentObj.transform.rotation = PlaceCollider.transform.rotation;

            GameObject go = _diContainer.InstantiatePrefab(_shopItemPrefab,
                PlaceCollider.transform.position, PlaceCollider.transform.rotation, null);

            if (parentObj.TryGetComponent(out NetworkItemType networkItem))
            {
                _networkObjectTransmitter = parentObj;

                networkItem.SetAimObject(go.transform);
            }

            if (go.TryGetComponent(out Rigidbody rb))
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        public void ToogleModelView(bool isVisible)
        {
            if (_shopItemModel != null)
            {
                _shopItemModel.SetActive(isVisible);
                PlaceCollider.enabled = isVisible;
            }
        }
        
        public void ReturnToShop()
        {
            if (_objectToBuyInstance == null)
            {
                return;
            }

            if (_networkObjectTransmitter != null &&
                _networkObjectTransmitter.TryGetComponent(out NetworkItemType networkItem))
            {
                if (networkItem.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    networkItem.DespawnCurrentObjectServerRpc();
            }
            _networkObjectTransmitter = null;
            _objectToBuyInstance = null;

            ItemReturnToShop?.Invoke();

            if (_shopItemModel != null)
            {
                _shopItemModel.SetActive(true);
            }

            if (PlaceCollider != null)
            {
                PlaceCollider.enabled = true;
            }
        }
        
        [ContextMenu("Grab item")]
        private void Grabbable_GrabStarted(GrabbingHand grabbingHand)
        {
            if (_networkObjectTransmitter != null)
                return;

            GrabStarted?.Invoke(this, Grabbable.LastGrabbingHand.transform);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PlaceInfo.SetActiveText(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlaceInfo.SetActiveText(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RequestToBuy?.Invoke(this);
        }
    }
}