using Assets.Scripts.API;
using Assets.Scripts.Space;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.MyPlaces
{
    /// <summary>
    /// For special events using
    /// </summary>
    public class SpecialPlaceViewEnterScene : MonoBehaviour
    {
        [SerializeField] private bool _enabled;
        [Space]
        [SerializeField] private RectTransform _apartmentListUI;
        [SerializeField] private PlaceItemUI _itemPrefab;
        private DiContainer _diContainer;


        [Inject]
        public void Init(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void Start()
        {
            if (_enabled) CreateSpaceUi(_apartmentListUI);
        }

        private void CreateSpaceUi(RectTransform parent)
        {
            var item = _diContainer.InstantiatePrefab(_itemPrefab, parent);
            PlaceItemUI placeItemUI = item.GetComponent<PlaceItemUI>();
            placeItemUI.SetName("Sense Meta");
        
            placeItemUI.SetSpaceType(SpaceType.InfrastructureScene);
            placeItemUI.SetSpaceId("466c4f30-6b7d-4155-878e-08f80e9b7303");
        }
    }
}
