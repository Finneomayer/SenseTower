using System;
using Cysharp.Threading.Tasks;
using Data;
using Mechanics.SpaceStaticObjectEditing.Model;
using UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Mechanics.SpaceStaticObjectEditing.UI
{
    public class SpaceEditingPlace : MonoBehaviour
    {
        [SerializeField] private BoxCollider PlaceCollider;
        [SerializeField] private InventoryPlaceGrabbable Grabbable;
        [SerializeField] private SpaceStaticObjectView PlaceInfo;

        public InventoryPlaceGrabbable PlaceGrabbable => Grabbable;
        public Enumenators.PrefabObjectType PlaceTowerPrefabType => _placeTowerPrefabType;
        public SpaceStaticObjectModel ItemModel;
        public event Action<SpaceEditingPlace,GrabbingHand> GrabStarted;
        public event Action<SpaceEditingPlace,GrabbingHand> GrabStopped;
        public event Action HandInPlaceChanged;

        private ISpaceFactory _spaceFactory;
        private Enumenators.PrefabObjectType _placeTowerPrefabType = Enumenators.PrefabObjectType.Unknown;
        [Inject]
        private void Construct(ISpaceFactory spaceFactory)
        {
            _spaceFactory = spaceFactory;
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

        public void SetPlaceTowerPrefabType(Enumenators.PrefabObjectType towerPrefabType)
        {
            _placeTowerPrefabType = towerPrefabType;
        }

        public void Init(IXRInteractor[] interactors, SpaceStaticObjectModel staticObjectModel)
        {
            DeInit();

            PlaceInfo.Init(this, interactors);

            ItemModel = staticObjectModel;
            
            if (staticObjectModel == null)
            {
                Debug.LogError("_userItem.ItemModelPrefab == null");
                return;
            }
            ItemModel.transform.SetParent(transform);
            ItemModel.transform.position = new Vector3(0, 10000, 0);
            ItemModel.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.down * 90);

            PlaceItemFitter.PlaceModel(ItemModel.gameObject, PlaceCollider).Forget();
        }

        public void DeInit()
        {
            PlaceInfo.DeInit();
            if (ItemModel != null)
            {
                Destroy(ItemModel.gameObject);
                ItemModel = null;
            }
        }
        [ContextMenu("Grab item")]
        private void GrabItem()
        {
            GrabStarted?.Invoke(this,null );
        }

        private void OnGrabStarted(GrabbingHand grabbingHand)
        {
            if (ItemModel != null)
                GrabStarted?.Invoke(this,grabbingHand);
        }

        private void OnGrabStopped(GrabbingHand grabbingHand)
        {
            GrabStopped?.Invoke(this,grabbingHand);
        }

        private void OnHandInPlaceChanged()
        {
            HandInPlaceChanged?.Invoke();
        }
    }
}