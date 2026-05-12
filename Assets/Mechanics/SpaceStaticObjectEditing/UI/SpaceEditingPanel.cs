using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Interaction;
using Mechanics.SpaceStaticObjectEditing.Model;
using Mechanics.SpaceStaticObjectEditing.Utils;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Mechanics.SpaceStaticObjectEditing.UI
{
    public class SpaceEditingPanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private SpaceEditingPanelRecyclingArea _spaceEditingPanelRecyclingArea;
        [SerializeField] private SpaceEditingPlace[] _places;
        [SerializeField] private Button _collectAllButton;
        [SerializeField] private Button _closeButton;

        #endregion

        private ISpaceFactory _spaceFactory;
        private ISpaceEditingService _spaceEditingService;
        private IXRInteractor[] _interactors;
        private Dictionary<string, GameObject> _spawnedObjects = new();
        public event Action CloseButtonClick;

        [Inject]
        private void Construct(ISpaceFactory spaceFactory, ISpaceEditingService spaceEditingService)
        {
            _spaceEditingService = spaceEditingService;
            _spaceFactory = spaceFactory;
        }

        private void OnEnable()
        {
            foreach (var item in _places)
            {
                item.GrabStarted += Item_GrabStarted;
            }

            _closeButton.onClick.AddListener(OnCloseButtonClick);
            _collectAllButton.onClick.AddListener(OnCollectAllButtonClick);
        }

        private void OnDisable()
        {
            foreach (var item in _places)
            {
                item.GrabStarted -= Item_GrabStarted;
            }

            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            _collectAllButton.onClick.RemoveListener(OnCollectAllButtonClick);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            gameObject.SetActive(true);
            if (_spawnedObjects.Count == 0)
            {
                InitPlaces();
            }
        }

        public override void HidePanel()
        {
            base.HidePanel();

            _spaceEditingPanelRecyclingArea.ForseDisable();
            gameObject.SetActive(false);
        }

        public void Init(IXRInteractor[] handControllers)
        {
            _interactors = handControllers;
        }

        private void OnCollectAllButtonClick()
        {
            DestroySpawnedObjects(true);
            _spaceEditingService.InvokeSaveOnClient();
            InitPlaces();
        }

        private void OnCloseButtonClick()
        {
            DestroySpawnedObjects(false);
            CloseButtonClick?.Invoke();
        }

        private void Item_GrabStarted(SpaceEditingPlace place, GrabbingHand grabbingHand = default)
        {
            CreateObject(place, grabbingHand);
        }
        /// <summary>
        /// Using when open space editing mode
        /// </summary>
        /// <param name="place"></param>
        /// <param name="grabbingHand"></param>
        private void CreateObject(SpaceEditingPlace place, GrabbingHand grabbingHand = default)
        {
            //Debug.LogError($"CreateEditingObject {place.ItemModel.StaticObject.PrefabObjectType}");
            //_collectAllButton.gameObject.SetActive(true);
            GameObject tempSpaceObject = _spaceFactory.CreateSpaceEditingObject(
                place.ItemModel.StaticObject.TowerObjectId.ToString(),
                place.transform.position, place.ItemModel.offsetPosition, grabbingHand);

            if (!string.IsNullOrEmpty(place.ItemModel.StaticObject.TempRelatedObjectId))
            {
                GameObject tempRelatedSpaceObject = _spaceFactory.CreateSpaceEditingObject(
                    place.ItemModel.StaticObject.TempRelatedObjectId,
                    place.transform.position, place.ItemModel.offsetPosition, grabbingHand);

                _spawnedObjects[place.ItemModel.StaticObject.TempRelatedObjectId] = tempRelatedSpaceObject;
            }

            _spawnedObjects[place.ItemModel.StaticObject.TowerObjectId.ToString()] = tempSpaceObject;
            place.DeInit();
            InitPersonalPlace(place);
        }
        /// <summary>
        /// Using when pick up an object from shelf
        /// </summary>
        /// <param name="towerObjectId"></param>
        /// <param name="prefabObjectType"></param>
        /// <param name="tempRelatedObjectId"></param>
        private void CreateObjectWithParameters(string towerObjectId, Enumenators.PrefabObjectType prefabObjectType,
            string tempRelatedObjectId)
        {
            //Debug.LogError($"CreateEditingObjectWithParameters {prefabObjectType}");
            //_collectAllButton.gameObject.SetActive(true);
            GameObject tempSpaceObject = _spaceFactory.CreateObjectWithParametrs(towerObjectId);

            if (!string.IsNullOrEmpty(tempRelatedObjectId))
            {
                GameObject tempDependanceSpaceObject =
                    _spaceFactory.CreateObjectWithParametrs(tempRelatedObjectId);
                _spawnedObjects[tempRelatedObjectId] = tempDependanceSpaceObject;
            }

            _spawnedObjects[towerObjectId] = tempSpaceObject;
            InitPersonalPlace(prefabObjectType);
        }

        private void DestroySpawnedObjects(bool clearData)
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                if (clearData)
                {
                    if (spawnedObject.Value.TryGetComponent(out ISpaceGrabingService mainGrabbingService))
                    {
                        mainGrabbingService.SetInactiveObject();
                    }
                }

                Destroy(spawnedObject.Value);
            }

            _spawnedObjects.Clear();
        }

        public void TryDestroyObject(SpaceStaticObjectModel destroyObject)
        {
            if (destroyObject == null)
                return;

            if (!_spawnedObjects.ContainsKey(destroyObject.StaticObject.TowerObjectId.ToString()))
                return;

            var tempDestroyObject = _spawnedObjects[destroyObject.StaticObject.TowerObjectId.ToString()];

            if (tempDestroyObject.TryGetComponent(out ISpaceGrabingService mainGrabbingService))
            {
                mainGrabbingService.SetInactiveObject();
            }

            if (!string.IsNullOrEmpty(destroyObject.StaticObject.TempRelatedObjectId))
            {
                string tempRelatedobjectId = destroyObject.StaticObject.TempRelatedObjectId;
                if (_spawnedObjects[tempRelatedobjectId].TryGetComponent(out ISpaceGrabingService dependsGrabbingService))
                {
                    dependsGrabbingService.SetInactiveObject();
                }
                Destroy(_spawnedObjects[tempRelatedobjectId]);
                _spawnedObjects.Remove(tempRelatedobjectId);
            }
            //foreach (SpaceStaticObjectModel destroyObjectDependantObject in destroyObject._dependantObjects)
            //{
            //    if (_spawnedObjects[destroyObjectDependantObject.StaticObject.TowerObjectId.ToString()]
            //        .TryGetComponent(out ISpaceGrabingService dependsGrabbingService))
            //    {
            //        dependsGrabbingService.SetInactiveObject();
            //    }
//
            //    Destroy(_spawnedObjects[destroyObjectDependantObject.StaticObject.TowerObjectId.ToString()]);
            //    _spawnedObjects.Remove(destroyObjectDependantObject.StaticObject.TowerObjectId.ToString());
            //}

            Destroy(tempDestroyObject);
            _spawnedObjects.Remove(destroyObject.StaticObject.TowerObjectId.ToString());

            //CheckState();
            if (destroyObject.PrefabObjectType != Enumenators.PrefabObjectType.Unknown)
            {
                InitPersonalPlace(destroyObject.StaticObject.PrefabObjectType);
            }

            _spaceEditingService.InvokeSaveOnClient();
        }

        private void CheckState()
        {
            if (_spawnedObjects.Count == 0)
            {
                _collectAllButton.gameObject.SetActive(false);
            }
        }

        private void InitPersonalPlace(Enumenators.PrefabObjectType prefabObjectType)
        {
            SpaceEditingPlace tempPlace =
                _places.FirstOrDefault(element => element.PlaceTowerPrefabType == prefabObjectType);
            
            InitPersonalPlace(tempPlace);
        }

        private void InitPersonalPlace(SpaceEditingPlace personalPlace)
        {
            personalPlace.DeInit();
            if (personalPlace.ItemModel != null)
                return;

            StaticObject[] existModelPrefabs = _spaceFactory.GetExistModelPrefabs()
                .Where(element => element.PrefabObjectType == personalPlace.PlaceTowerPrefabType).ToArray();

            StaticObject initStaticObject = null;

            for (int i = 0; i < existModelPrefabs.Length; i++)
            {
                if (!_spawnedObjects.ContainsKey(existModelPrefabs[i].TowerObjectId.ToString()))
                {
                    initStaticObject = existModelPrefabs[i];
                }
            }

            if (initStaticObject == null)
                return;

            SpaceStaticObjectModel spaceStaticObjectModel =
                _spaceFactory.CreateObjectVisualize(initStaticObject.TowerObjectId.ToString());

            personalPlace.Init(_interactors, spaceStaticObjectModel);
        }

        private void InitPlaces()
        {
            //_collectAllButton.gameObject.SetActive(false);

            StaticObject[] existModelPrefabs = _spaceFactory.GetExistModelPrefabs();

            Enumenators.PrefabObjectType[] towerPrefabTypes = existModelPrefabs
                .Select(element => element.PrefabObjectType).ToArray().Distinct().ToArray();

            int placeCount = towerPrefabTypes.Length;

            for (int i = 0; i < towerPrefabTypes.Length; i++)
            {
                _places[i].SetPlaceTowerPrefabType(towerPrefabTypes[i]);
            }

            for (int i = 0; i < placeCount; i++)
            {
                SpaceEditingPlace tempPlace = _places[i];
                StaticObject[] staticObjects = existModelPrefabs.Where(element =>
                    element.PrefabObjectType == tempPlace.PlaceTowerPrefabType).ToArray();

                StaticObject initStaticObject = null;

                for (int j = 0; j < staticObjects.Length; j++)
                {
                    if (!_spawnedObjects.ContainsKey(staticObjects[j].TowerObjectId.ToString()))
                    {
                        initStaticObject = staticObjects[j];
                    }
                }

                if (initStaticObject == null)
                    continue;

                SpaceStaticObjectModel spaceStaticObjectModel =
                    _spaceFactory.CreateObjectVisualize(initStaticObject.TowerObjectId.ToString());

                tempPlace.Init(_interactors, spaceStaticObjectModel);
            }

            for (int k = 0; k < existModelPrefabs.Length; k++)
            {
                StaticObject tempStaticObject = existModelPrefabs[k];
                
                if (tempStaticObject.IsActive)
                {
                    CreateObjectWithParameters(tempStaticObject.TowerObjectId.ToString(),
                        tempStaticObject.PrefabObjectType, tempStaticObject.TempRelatedObjectId);
                }
            }
        }
    }
}