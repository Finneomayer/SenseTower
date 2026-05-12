using Cysharp.Threading.Tasks;
using System;
using Assets.Scripts.TowerObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace UI
{
    public class InventoryPlace : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider PlaceCollider;
        [SerializeField]
        private InventoryPlaceGrabbable Grabbable;
        [SerializeField]
        private InventoryPlaceView PlaceInfo;

        [SerializeField] private Transform _rotatingParent;

        private GameObject _itemModel;
        private TowerObjectDto _userItem;
        private int _itemCount;
        private DiContainer _diContainer;
        private float _angle;

        public TowerObjectDto UserItem => _userItem;
        public int ItemCount => _itemCount;
        public InventoryPlaceGrabbable PlaceGrabbable => Grabbable;

        public event Action<GrabbingHand,InventoryPlace> GrabStarted;
        public event Action<InventoryPlace> GrabStopped;
        public event Action<InventoryPlace> HandInPlaceChanged;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void OnEnable()
        {
            Grabbable.GrabStarted += OnGrabStarted;
            Grabbable.GrabStopped += OnGrabStopped;
            Grabbable.HandInPlaceChanged += OnHandInPlaceChanged;
        }

        private void OnDisable()
        {
            Grabbable.GrabStarted -= OnGrabStarted;
            Grabbable.GrabStopped -= OnGrabStopped;
            Grabbable.HandInPlaceChanged -= OnHandInPlaceChanged;
        }

        private void FixedUpdate()
        {
            if (_rotatingParent != null)
            {
                if (_angle < 1f)
                {
                    _angle += Time.fixedDeltaTime * _angle;
                }
                _rotatingParent.Rotate(Vector3.up, _angle);
            }
        }

        public void Init(IXRInteractor[] interactors, GameObject prefab, BoughtItems userItem)
        {
            DeInit();

            _userItem = userItem.TowerObject;
            _itemCount = userItem.Count;

            PlaceInfo.Init(this, interactors);

            if (prefab == null)
            {
                Debug.LogError("_userItem.ItemModelPrefab == null");
                return;
            }

            Transform parent = _rotatingParent != null ? _rotatingParent : transform;

            _itemModel = _diContainer.InstantiatePrefab(prefab, new Vector3(0, 10000, 0),
                PlaceCollider.transform.rotation, parent);

            PlaceItemFitter.PlaceModel(_itemModel, PlaceCollider).Forget();

            _angle = 0.1f;
        }

        public void DeInit()
        {
            PlaceInfo.DeInit();
            if (_itemModel != null)
            {
                Destroy(_itemModel);
                _itemModel = null;
            }
            _userItem = null;
        }

        [ContextMenu("Grab item")]
        private void GrabItem()
        {
            GrabStarted?.Invoke(null, this);
        }

        private void OnGrabStarted(GrabbingHand grabbingHand)
        {
            GrabStarted?.Invoke(grabbingHand, this);
            _angle = 0.1f;
        }

        private void OnGrabStopped(GrabbingHand grabbingHand)
        {
            GrabStopped?.Invoke(this);
        }

        private void OnHandInPlaceChanged()
        {
            HandInPlaceChanged?.Invoke(this);
        }
    }
}
