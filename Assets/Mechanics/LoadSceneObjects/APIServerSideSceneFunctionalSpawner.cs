using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Blackboard;
using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using Data;
using Mechanics.LoadSceneObjects.Interfaces;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.Network.Scripts.SpaceObjectsService;
using Mechanics.Network.Scripts.StaticObjectsService;
using Mechanics.VideoService.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Mechanics.LoadSceneObjects
{
    public class APIServerSideSceneFunctionalSpawner : NetworkBehaviour, ISceneObjectSpawner
    {
        #region Inspector

        [SerializeField] private Place _placePrefab;
        [SerializeField] private BlackBoard _blackBoardPrefab;
        [SerializeField] private Browser _browserPrefab;
        [SerializeField] private AdminPlace _adminPlacePrefab;
        [SerializeField] private GameObject _presentationPrefab;
        #endregion

        private string _roomId = string.Empty;
        private List<GameObject> _presentations;
        private List<Place> _places = new();
        private Dictionary<string, GameObject> _spawnedObjects = new();
        
        private IServerApiService _serverApiService;
        private IStaticObjectsService _staticObjectService;
        private ISpaceObjectService _spaceObjectService;

        [Inject]
        private void Construct(IStaticObjectsService staticObjectsService, IServerApiService serverApiService, ISpaceObjectService spaceObjectService)
        {
            _serverApiService = serverApiService;
            _staticObjectService = staticObjectsService;
            _spaceObjectService = spaceObjectService;
            _presentations = new List<GameObject>();
#if UNITY_SERVER
            _roomId = DataExtensions.GetSpaceID();
            _serverApiService.ServerAuth += OnServerAuth;
#endif
        }

        private void OnDisable()
        {
#if UNITY_SERVER
            _serverApiService.ServerAuth -= OnServerAuth;
#endif
        }

        public async void CheckExistSceneObjects()
        {
            if (string.IsNullOrEmpty(_roomId))
                return;

            List<StaticObject> sceneObjects = new List<StaticObject>();
            while (true)
            {
                //_sceneObjects = await _staticObjectService.GetAllSceneStaticObjects(_roomId);
                var space = await _spaceObjectService.GetSpaceWithAllObjects(_roomId);

                if (space != null && space.Objects != null) Debug.LogError($"_______________space-{space.Objects.Count}");

                foreach (var spaceObject in space.Objects)
                {
                    sceneObjects.Add(DataExtensions.SpaceObjectToStaticObject(spaceObject.Value));
                }

                Debug.LogError($"_______________static-{sceneObjects.Count}");

                if (sceneObjects != null && sceneObjects.Count > 0)
                {
                    break;
                }

                await UniTask.Delay(1000);
            }

            SpawnSceneObjects(sceneObjects, null,null);
        }

        public void SpawnSceneObjects(List<StaticObject> objects, List<PictureSpaceObject> picture,List<VideoSpaceObject> videos)
        {
            List<StaticObject> BrowserStaticObjects = new List<StaticObject>();
            List<StaticObject> BrowserAdminPlaceStaticObjects = new List<StaticObject>();

            Dictionary<string, List<PointData>> blackboardsPointsMap = GetReloadingBlackboardsPointsMap(objects);

            ClearSceneObjects();
            foreach (StaticObject networkModel in objects)
            {
                Debug.Log($"___________ {networkModel.PrefabObjectType}");
                if (networkModel.PrefabObjectType == Enumenators.PrefabObjectType.Blackboard)
                {
                    string staticObjectKey = networkModel.TowerObjectId.ToString();
                    blackboardsPointsMap.TryGetValue(staticObjectKey, out List<PointData> pointsToRestore);
                    SetBlackboard(networkModel, pointsToRestore);
                }

                if (networkModel.PrefabObjectType == Enumenators.PrefabObjectType.BrowserPlace)
                {
                    if (!String.IsNullOrEmpty(networkModel.TempRelatedObjectId)) BrowserStaticObjects.Add(networkModel);
                }

                if (networkModel.PrefabObjectType == Enumenators.PrefabObjectType.BrowserAdminPlace)
                {
                    BrowserAdminPlaceStaticObjects.Add(networkModel);
                }
            }

            int browserCounter = 1;

            foreach (var browser in BrowserStaticObjects)
            {
                foreach (var logo in BrowserAdminPlaceStaticObjects)
                {
                    if (Guid.TryParse(browser.TempRelatedObjectId, out Guid browserGuid))
                    {
                        if (browserGuid == logo.TowerObjectId)
                        {
                            SetBrowserOnScene(browser, logo, browserCounter);
                            browserCounter++;
                        }
                    }
                }
            }

            SetChairOnScene(objects.Where(element => element.PrefabObjectType == Enumenators.PrefabObjectType.Place)
                .ToList());
        }

        private void OnServerAuth()
        {
            CheckExistSceneObjects();
        }

        private Dictionary<string, List<PointData>> GetReloadingBlackboardsPointsMap(
            List<StaticObject> newLoadingStaticObjects)
        {
            Dictionary<string, List<PointData>> blackboardsPointsMap = new();
            foreach (StaticObject staticObject in newLoadingStaticObjects)
            {
                if (staticObject.PrefabObjectType != Enumenators.PrefabObjectType.Blackboard)
                {
                    continue;
                }

                string staticObjectKey = staticObject.TowerObjectId.ToString();

                if (!_spawnedObjects.TryGetValue(staticObjectKey, out GameObject blackboardGO)
                    || blackboardGO == null)
                {
                    continue;
                }

                if (!blackboardGO.TryGetComponent(out BlackBoard blackboard))
                {
                    continue;
                }

                blackboardsPointsMap.Add(staticObjectKey, blackboard.GetDrawingPointsServer());
            }

            return blackboardsPointsMap;
        }

        private void SetBlackboard(StaticObject networkModel, List<PointData> pointsToRestore)
        {
            if (networkModel.PrefabObjectType != Enumenators.PrefabObjectType.Blackboard)
                return;

            Vector3 position = DataExtensions.CalculateMiddlePosition(networkModel);
            Vector3 scale = DataExtensions.CalculateBlackboardScale(networkModel);

            if (!networkModel.IsActive)
                return;

            BlackBoard blackBoardInstance = Instantiate(_blackBoardPrefab);
            
            Vector3 rotation = DataExtensions.CalculateRotation(blackBoardInstance.transform, networkModel)
                .eulerAngles;

            blackBoardInstance.transform.position = position;
            blackBoardInstance.transform.rotation = Quaternion.Euler(rotation);
            blackBoardInstance.BlackboardRenderTexture.transform.localScale = scale;

            blackBoardInstance.SetId(networkModel.TowerObjectId);
            blackBoardInstance.SetDrawingPointsServer(pointsToRestore);

            if (blackBoardInstance.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.Spawn();
            }

            //blackBoardInstance.SetId(networkModel.TowerObjectId);

            _spawnedObjects[networkModel.TowerObjectId.ToString()] = blackBoardInstance.gameObject;
        }

        private GameObject SpawnBrowserAdminPlace(StaticObject networkModel)
        {
            if (networkModel.PrefabObjectType != Enumenators.PrefabObjectType.BrowserAdminPlace)
                return null;
            
            if (_spawnedObjects.TryGetValue(networkModel.TowerObjectId.ToString(), out GameObject browserAdminPlace))
            {
                if (browserAdminPlace.TryGetComponent(out NetworkObject netObj))
                {
                    if (netObj.IsSpawned)
                        netObj.Despawn();
                }
                Destroy(browserAdminPlace);
                _spawnedObjects.Remove(networkModel.TowerObjectId.ToString());
            }
            
            if (!networkModel.IsActive)
                return null;
            
            AdminPlace adminPlaceInstance = Instantiate(_adminPlacePrefab);
            adminPlaceInstance.LogoTowerObjectId.Value = networkModel.TowerObjectId.ToString();

            Vector3 browserAdminPlacePosition = networkModel.Vectors.Position.VectorComponentsToVector3();
            Quaternion _browserAdminPlaceRotation =
                DataExtensions.CalculateRotation(adminPlaceInstance.transform, networkModel);
            adminPlaceInstance.transform.SetPositionAndRotation(browserAdminPlacePosition,
                _browserAdminPlaceRotation);
            
            _spawnedObjects[networkModel.TowerObjectId.ToString()] = adminPlaceInstance.gameObject;
            
            return adminPlaceInstance.gameObject;
        }

        private GameObject SpawnBrowser(StaticObject networkModel, int browserCounter)
        {
            if (networkModel.PrefabObjectType != Enumenators.PrefabObjectType.BrowserPlace)
                return null;

            if (_spawnedObjects.TryGetValue(networkModel.TowerObjectId.ToString(), out GameObject browserPlace))
            {
                if (browserPlace.TryGetComponent(out NetworkObject netObj))
                {
                    if (netObj.IsSpawned)
                        netObj.Despawn();
                }
                Destroy(browserPlace);
                _spawnedObjects.Remove(networkModel.TowerObjectId.ToString());
            }
            
            if (!networkModel.IsActive)
                return null;

            Browser browserInstance = Instantiate(_browserPrefab);
            browserInstance.LogoTowerObjectId.Value = networkModel.TempRelatedObjectId;
            browserInstance.BrowserNumber.Value = browserCounter;

            Vector3 positon = DataExtensions.CalculateMiddlePosition(networkModel);
            Quaternion rotation = DataExtensions.CalculateRotation(browserInstance.transform, networkModel) *
                                  Quaternion.Euler(0, 90, 0);

            browserInstance.transform.position = positon - Vector3.up * 1.39f;
            browserInstance.transform.rotation = rotation;

            _spawnedObjects[networkModel.TowerObjectId.ToString()] = browserInstance.gameObject;
            
            return browserInstance.gameObject;
        }

        private GameObject SpawnBrowserConstructor()
        {
            foreach (var presentationObject in _presentations)
            {
                Destroy(presentationObject);
            }

            Debug.LogWarning($"+++Browser+++ SpawnBrowserConstructor");
            GameObject presentation = Instantiate(_presentationPrefab);
            _presentations.Add(presentation);
            return presentation;
        }

        private void SetBrowserOnScene(StaticObject browserStaticObject, StaticObject browserAdminPlaceStaticObject, int browserCounter)
        {
            Debug.LogWarning($"+++Browser+++ Browser TempRelatedObjectId = {browserStaticObject.TempRelatedObjectId}");
            Debug.LogWarning($"+++Browser+++ AdminPlaceId = {browserAdminPlaceStaticObject.TowerObjectId} ");

            GameObject browserConstructor = SpawnBrowserConstructor();
            GameObject browserAdminPlaceInstance = SpawnBrowserAdminPlace(browserAdminPlaceStaticObject);
            GameObject browserInstance = SpawnBrowser(browserStaticObject, browserCounter);

            var presentation = browserConstructor.GetComponent<Presentation>();

            if (browserAdminPlaceInstance == null || browserInstance == null)
                return;
            
            if (browserAdminPlaceInstance.TryGetComponent(out AdminPlace browserAdminPlace))
            {
                browserAdminPlace.SetInteractionObject(presentation.SenseLogo.gameObject);

                presentation.SetAdminPlace(browserAdminPlace);
                browserAdminPlace.NetworkObject.Spawn();
            }
            
            if (browserInstance.TryGetComponent(out Browser browser))
            {
                presentation.SetBrowser(browser);
                browser.NetworkObject.Spawn();
                browser.BrowserScale.Value = DataExtensions.CalculateBrowserScale(browserStaticObject);
                browser.BrowserAdminPlace.Value = browserAdminPlaceInstance.transform.position;
            }

            Debug.Log($"++BROWSER++ APIServerSideSceneFunctionalSpawner SetBrowserOnScene");

            presentation.Init();
        }

        private void SetChairOnScene(List<StaticObject> chairs)
        {
            if (_places.Count != 0)
            {
                for (int i = 0; i < _places.Count; i++)
                {
                    _places[i].NetworkObject.Despawn();
                    Destroy(_places[i].gameObject);
                }

                _places.Clear();
            }
            if (chairs.Count == 0)
                return;

            _places = new();
            for (int i = 0; i < chairs.Count; i++)
            {
                if (chairs[i].PrefabObjectType != Enumenators.PrefabObjectType.Place)
                    continue;
                
                if (!chairs[i].IsActive)
                    continue;
                
                Place newPlace = Instantiate(_placePrefab);
                newPlace.transform.position = chairs[i].Vectors.Position.VectorComponentsToVector3();
                newPlace.transform.rotation = Quaternion.Euler(chairs[i].Vectors.Rotation.VectorComponentsToVector3());
                newPlace.NetworkObject.Spawn();
                _places.Add(newPlace);
                // newPlace.transform.parent = chairGameObject.transform;
            }
        }

        private void ClearSceneObjects()
        {
            foreach (KeyValuePair<string,GameObject> spawnedObject in _spawnedObjects)
            {
                if (string.IsNullOrEmpty(spawnedObject.Key) || spawnedObject.Value == null)
                    continue;
                if (spawnedObject.Value.TryGetComponent(out NetworkObject networkObject))
                {
                    if (networkObject.IsSpawned)
                        networkObject.Despawn();
                }
                
                Destroy(spawnedObject.Value);
            }
            _spawnedObjects.Clear();
            
            if (_places.Count != 0)
            {
                for (int i = 0; i < _places.Count; i++)
                {
                    _places[i].NetworkObject.Despawn();
                    Destroy(_places[i].gameObject);
                }
                _places.Clear();
            }
        }
    }
}