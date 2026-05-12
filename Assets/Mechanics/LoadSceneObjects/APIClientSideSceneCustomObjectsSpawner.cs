using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.NetworkInteraction;
using Assets.Mechanics.NetworkInteraction.Services;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Infrastructure.Factory;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using Data;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Mechanics.LoadSceneObjects.Interfaces;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.Network.Scripts.SpaceObjectsService;
using Mechanics.Network.Scripts.StaticObjectsService;
using Mechanics.VideoService.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Mechanics.LoadSceneObjects
{
    [Serializable]
    public class SceneEnvironmentThemeBehaviour
    {
        [field: SerializeField]
        public string SceneObjectKey { get; private set; }

        [field: SerializeField]
        public bool IsActiveBehaviour { get; private set; }

        [field: SerializeField]
        public List<GameObject> ObjectsToDisable { get; private set; }
    }

    public class APIClientSideSceneCustomObjectsSpawner: NetworkBehaviour, ISceneObjectSpawner
    {
         #region Inspector
        [SerializeField] private List<SceneEnvironmentThemeBehaviour> _defaultSceneEnvironmentThemeBehaviours;
        [SerializeField] private AddressablesSceneObject _decorationEmptySceneObjectPrefab;
        [SerializeField] private MovableSceneObject _movableEmptySceneObjectPrefab;
        [SerializeField] private NetworkStartInvoker _networkInvoker;
        [SerializeField] private List<CustomObjectData> _customObjectsPrefab = new();
        [SerializeField] private APIServerSideSceneCustomObjectsSpawner _apiServerSideSceneCustomObjectsSpawner;
        #endregion

        private List<AddressablesSceneObject> _spawnedObjects = new();
        private List<StaticObject> _sceneStaticObjects = new();
        private int _spawnedObjectCount;
        private int _delayedSpawnObjectsCount;
        private IStaticObjectsService _staticObjectsService;
        private ISpaceObjectService _spaceObjectService;
        private IGrabInteraction _grabInteraction;
        private ISpaceManager _spaceManager;
        private IClientData _clientData;
        private SceneFactory _sceneFactory;

        private MeshRenderer[] _initialEnvironmentMeshRenderers;

        [Inject]
        private void Construct(IStaticObjectsService staticObjectsService, ISpaceObjectService spaceObjectService,
            IClientData clientData, ISpaceManager spaceManager, IGrabInteraction grabInteraction)
        {
            _grabInteraction = grabInteraction;
            _spaceManager = spaceManager;
            _staticObjectsService = staticObjectsService;
            _spaceObjectService = spaceObjectService;
            _clientData = clientData;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                return;

            _apiServerSideSceneCustomObjectsSpawner.GetMovableObjectsFromServer();
        }

        private void Awake()
        {
#if !UNITY_SERVER
            _initialEnvironmentMeshRenderers = FindObjectsOfType<MeshRenderer>();

            if (!string.IsNullOrEmpty(_clientData.AccessToken))
            {
                CheckExistSceneObjects();
            }
#endif
        }
        private void OnDisable()
        {
            for (int i = 0; i < _spawnedObjects.Count; i++)
            {
                _spawnedObjects[i].OnCreate -= OnObjectCreate;
            }
        }

        public void SpawnSceneObjects(List<StaticObject> sceneStaticObjects, List<PictureSpaceObject> picture, List<VideoSpaceObject> videos)
        {
            _spawnedObjectCount = 0;
            _delayedSpawnObjectsCount = 0;
            try
            {
                for (int i = 0; i < sceneStaticObjects.Count; i++)
                {
                    StaticObject tempStaticObject = sceneStaticObjects[i];
                    
                    if (!tempStaticObject.IsActive)
                        continue;
                    
                    if (tempStaticObject.RemoteObjectType == Enumenators.RemoteContentType.Scene)
                    {
                        _delayedSpawnObjectsCount++;
                        LoadRemoteEnviroment(tempStaticObject);
                        continue;
                    }

                    if (tempStaticObject.PrefabObjectType == Enumenators.PrefabObjectType.DecorationModel)
                    {
                        _delayedSpawnObjectsCount++;
                        LoadDecorationModels(tempStaticObject);
                        continue;
                    }

                    if (_initialEnvironmentMeshRenderers != null && tempStaticObject.ObjectKey == "HalloweenEnvironment")
                    {
                        RenderSettings.reflectionIntensity = 0;
                        RenderSettings.fogColor = new Color(0.2f, 0.2f, 0.2f);
                        RenderSettings.fogDensity = 0.1f;

                        foreach (var renderer in _initialEnvironmentMeshRenderers)
                        {
                            foreach (var material in renderer.materials)
                            {
                                if (material.HasColor("_Color"))
                                {
                                    material.SetColor("_Color", material.GetColor("_Color") / 2);
                                }
                                if (material.HasColor("_MainColor"))
                                {
                                    material.SetColor("_MainColor", material.GetColor("_MainColor") / 2);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                _networkInvoker.StartNetwork();
            }

            if (_delayedSpawnObjectsCount <= 0)
            {
                _networkInvoker.StartNetwork();
            }
        }

        public void SpawnMovableObject(string towerObjectId, NetworkObject networkObject)
        {
            StaticObject tempStaticObject =
                _sceneStaticObjects.FirstOrDefault(element => element.TowerObjectId.ToString() == towerObjectId);

            if (tempStaticObject != null)
            {
                SpawnCustomObjects(tempStaticObject,networkObject);
            }
        }

        private async void CheckExistSceneObjects()
        {
            string currentSpaceId = _spaceManager.CurrentTransitionTarget.Id.ToString();

            float endTryingTime = Time.time + 3;

            while (Time.time < endTryingTime)
            {
                //_sceneStaticObjects = await _staticObjectsService.GetAllSceneStaticObjects(currentSpaceId);
                var  space = await _spaceObjectService.GetSpaceWithAllObjects(currentSpaceId);
                _sceneStaticObjects = new List<StaticObject>();

                foreach (var spaceObject in space.Objects)
                {
                    _sceneStaticObjects.Add(DataExtensions.SpaceObjectToStaticObject(spaceObject.Value));
                }

                if (_sceneStaticObjects != null && _sceneStaticObjects.Count > 0)
                {
                    break;
                }

                await UniTask.Delay(1000);
            }

            if (_sceneStaticObjects == null)
            {
                _sceneStaticObjects = new List<StaticObject>();
            }

            SpawnSceneObjects(_sceneStaticObjects, null,null);
        }

        private void LoadRemoteEnviroment(StaticObject tempSceneObject)
        {
            if (!tempSceneObject.IsActive)
                return;
            
            _sceneFactory = new SceneFactory(new AddressableResourcesLocation(
                ResourcesLocation.GetRemoteSceneObjectCatalogPath(tempSceneObject.RepositoryUrl),
                () => OnRemoteSceneFactoryLoad(tempSceneObject.ObjectKey)));
        }

        private void LoadDecorationModels(StaticObject staticObject)
        {
            if (staticObject.PrefabObjectType != Enumenators.PrefabObjectType.DecorationModel)
                return;
            if (!staticObject.IsActive)
                return;
            
            Vectors tempTransformValue = staticObject.Vectors;

            AddressablesSceneObject tempAddressablesSceneObject = Instantiate(_decorationEmptySceneObjectPrefab);
            _spawnedObjects.Add(tempAddressablesSceneObject);

            tempAddressablesSceneObject.OnCreate += OnObjectCreate;
            tempAddressablesSceneObject.Init(
                ResourcesLocation.GetRemoteSceneObjectCatalogPath(staticObject.RepositoryUrl));
                    
            tempAddressablesSceneObject.Create(
                staticObject.ObjectKey,
                new Vector3(tempTransformValue.Position.X, tempTransformValue.Position.Y,
                    tempTransformValue.Position.Z),
                new Vector3(tempTransformValue.Rotation.X, tempTransformValue.Rotation.Y,
                    tempTransformValue.Rotation.Z),
                new Vector3(tempTransformValue.Scale.X, tempTransformValue.Scale.Y,
                    tempTransformValue.Scale.Z));
            
            if(!string.IsNullOrEmpty(staticObject.HelpContent))
                tempAddressablesSceneObject.SetTipsContent(staticObject.HelpContent);
        }

        private async void OnRemoteSceneFactoryLoad(string sceneKey)
        {
            await _sceneFactory.LoadSceneAsync(sceneKey, LoadSceneMode.Additive).Task;
            await _sceneFactory.ActiveSceneAsync();
            DisableStartEnviroment(sceneKey);
            OnObjectCreate(true);
            //_networkInvoker.StartNetwork();
        }

        private void DisableStartEnviroment(string sceneKey)
        {
            var defaultSceneBehaviour = _defaultSceneEnvironmentThemeBehaviours.FirstOrDefault(x => x.IsActiveBehaviour
                && x.SceneObjectKey.Equals(sceneKey, StringComparison.OrdinalIgnoreCase));
            
            if (defaultSceneBehaviour == null)
            {
                return;
            }

            foreach (var sceneObject in defaultSceneBehaviour.ObjectsToDisable)
            {
                if (sceneObject != null)
                {
                    sceneObject.SetActive(false);
                }
            }
        }

        private void OnObjectCreate(bool isCreate)
        {
            _spawnedObjectCount++;

            if (_spawnedObjectCount >= _delayedSpawnObjectsCount)
                _networkInvoker.StartNetwork();
        }

        private void SpawnCustomObjects(StaticObject staticObject,NetworkObject networkObject)
        {
            Debug.LogWarning($"Spawn object {staticObject.PrefabObjectType} - {staticObject.ObjectKey} -- {staticObject.TowerObjectId}");

            if (networkObject.TryGetComponent(out OnObjectData objectPublicData))
            {
                objectPublicData.ThisTowerObjectId = staticObject.TowerObjectId.ToString();
            }

            GameObject tempPrefab = _customObjectsPrefab.FirstOrDefault(element=> element.ObjectKey == staticObject.ObjectKey)?.CustomObjectPrefab;

            if (tempPrefab != null)
            {
                GameObject tempPrefabGameObject = Instantiate(tempPrefab);
                ConstructMovableObject(staticObject,tempPrefabGameObject,networkObject);
            }
            else if (!string.IsNullOrEmpty(staticObject.RepositoryUrl))
            {
                MovableSceneObject tempMovableSceneObject = Instantiate(_movableEmptySceneObjectPrefab);
                
                tempMovableSceneObject.Init(
                    ResourcesLocation.GetRemoteObjectCatalogPath(staticObject.RepositoryUrl));
                    
                tempMovableSceneObject.Create(
                    staticObject.ObjectKey, 
                    staticObject.Vectors.Position.VectorComponentsToVector3(),
                    staticObject.Vectors.Rotation.VectorComponentsToVector3(),
                    staticObject.Vectors.Scale.VectorComponentsToVector3());
                
                tempMovableSceneObject.ConstructMovableObject(staticObject,networkObject, _grabInteraction);
                if(!string.IsNullOrEmpty(staticObject.HelpContent))
                    tempMovableSceneObject.SetTipsContent(staticObject.HelpContent);
            }
            else
            {
                if (networkObject.TryGetComponent(out NetworkXrGrab networkXrGrab))
                {
                    networkXrGrab.Init(networkObject, _grabInteraction);
                }
            }
        }

        private void ConstructMovableObject(StaticObject staticObject, GameObject PrefabGameObject,NetworkObject networkObject)
        {
            PrefabGameObject.transform.localScale = staticObject.Vectors.Scale.VectorComponentsToVector3();
            if (PrefabGameObject.TryGetComponent(out NetworkXrGrab networkXrGrab))
            {
                networkXrGrab.Init(networkObject,_grabInteraction);
            }

            if (networkObject.TryGetComponent(out NetworkItemType networkItemType))
            {
                networkItemType.SetAimObject(PrefabGameObject.transform);
            }

            if (networkObject.TryGetComponent(out CustomBehaviourNetworkObject customBehaviour))
            {
                if (PrefabGameObject.TryGetComponent(out INetworkCustomLogicService networkCustomLogicService))
                {
                    networkCustomLogicService.Init(staticObject, customBehaviour);
                }

                StaticObjectCollider[] colliders = PrefabGameObject.GetComponentsInChildren<StaticObjectCollider>();

                foreach (var item in colliders)
                {
                    item.Init(staticObject);
                }
            }
        }

        #region InnerClass

        [Serializable]
        public class CustomObjectData
        {
            public string ObjectKey;
            public GameObject CustomObjectPrefab;
        }

        #endregion
    }
}