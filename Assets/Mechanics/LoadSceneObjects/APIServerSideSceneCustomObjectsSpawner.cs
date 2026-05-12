using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.CustomMehanics;
using Assets.Mechanics.Mafia.Table;
using Assets.Mechanics.Network.Scripts;
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
using Zenject;
using static Data.Enumenators;

namespace Mechanics.LoadSceneObjects
{
    public class APIServerSideSceneCustomObjectsSpawner : NetworkBehaviour, ISceneObjectSpawner
    {
        #region Inspector

        [SerializeField] private NetworkObject _emptyPrefab;
        [SerializeField] private APIClientSideSceneCustomObjectsSpawner _apiClientSideSceneCustomObjectsSpawner;

        [Space] [Header("Custom Objects")]
        [SerializeField] private CustomLogicTowerObjectsSet _customLogicTowerObjectsSet;
        [SerializeField] private GameObject _stormPrefab;
        [SerializeField] private TableExpander _mafiaTable;
        #endregion

        private IServerApiService _serverApiService;
        private IStaticObjectsService _staticObjectService;
        private ISpaceObjectService _spaceObjectService;

        private string _roomId = string.Empty;

        private Dictionary<NetworkObject, string> _networkObjectsInScene = new();

        [Inject]
        private void Construct(IStaticObjectsService staticObjectsService, ISpaceObjectService spaceObjectService, IServerApiService serverApiService)
        {
            _serverApiService = serverApiService;
            _staticObjectService = staticObjectsService;
            _spaceObjectService = spaceObjectService;
#if UNITY_SERVER
            _roomId = DataExtensions.GetSpaceID();
            _serverApiService.ServerAuth += OnServerAuth;
#endif
        }

        private void OnDisable()
        {
#if UNITY_SERVER
            _serverApiService.ServerAuth -= OnServerAuth;
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
#endif
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _roomId = DataExtensions.GetSpaceID();
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            }
        }

        public void GetMovableObjectsFromServer()
        {
            GetMovableObjectsInSceneServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        public async void CheckExistSceneObjects()
        {
            List<StaticObject> sceneObjects = new List<StaticObject>();
            while (true)
            {
                //sceneObjects = await _staticObjectService.GetAllSceneStaticObjects(_roomId);
                var space = await _spaceObjectService.GetSpaceWithAllObjects(_roomId);
                sceneObjects = new List<StaticObject>();

                foreach (var spaceObject in space.Objects)
                {
                    sceneObjects.Add(DataExtensions.SpaceObjectToStaticObject(spaceObject.Value));
                }


                if (sceneObjects != null && sceneObjects.Count > 0)
                {
                    break;
                }

                await UniTask.Delay(1100);
            }

            SpawnSceneObjects(sceneObjects, null,null);
        }

        public void SpawnSceneObjects(List<StaticObject> objects, List<PictureSpaceObject> pictures, List<VideoSpaceObject> videos)
        {
            foreach (StaticObject networkModel in objects)
            {
                //if (networkModel.PrefabObjectType != Enumenators.PrefabObjectType.MovableObject)
                //    continue;
                if (!networkModel.IsActive)
                    continue;

                _customLogicTowerObjectsSet.TryGetPrefabByKey(networkModel.ObjectKey, out GameObject prefab);
                if (prefab != null)
                {
                    if (prefab.TryGetComponent(out NetworkObject networkObject))
                    {
                        CreateCustomObject(networkModel, networkObject);
                    }
                    continue;
                }

                //Debug.LogError($"!!! Spawn static object url: {networkModel?.RepositoryUrl}");

                switch (networkModel.ObjectKey)
                {
                    case "Storm":
                        NetworkObject networkObject = CreateObjectPrototype(networkModel);
                        if (networkObject.TryGetComponent(out CustomBehaviourNetworkObject customBehaviourNetworkObject))
                        {
                            GameObject serverStormGenerator = Instantiate(_stormPrefab, 
                                networkObject.transform.position, networkObject.transform.rotation, networkObject.transform);

                            if (serverStormGenerator.TryGetComponent(out INetworkCustomLogicService networkCustomLogicService))
                            {
                                networkCustomLogicService.Init(networkModel, customBehaviourNetworkObject);
                            }
                        }
                        //CreateCustomObject(networkModel, _storm);
                        break;
                    case "MafiaTable":
                        TableExpander table = Instantiate(_mafiaTable);
                        table.Init(networkModel);
                        table.transform.position = networkModel.Vectors.Position.VectorComponentsToVector3();
                        table.transform.rotation = Quaternion.Euler(networkModel.Vectors.Rotation.VectorComponentsToVector3());
                        table.NetworkObject.Spawn();
                        _networkObjectsInScene.Add(table.NetworkObject, networkModel.ObjectKey);
                        Debug.Log("*** Add table");
                        break;
                    default:
                        //Debug.LogError($"!!! create object prototype start: {networkModel?.RepositoryUrl}");
                        CreateObjectPrototype(networkModel);
                        break;
                }
            }
        }

        private void OnClientDisconnect(ulong clientId)
        {
            int checkCount = NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId) ? 1 : 0;

            if (NetworkManager.Singleton.ConnectedClients.Count <= checkCount)
            {
                foreach (KeyValuePair<NetworkObject, string> objectsInScene in _networkObjectsInScene)
                {
                    if (objectsInScene.Key == null || objectsInScene.Key.gameObject == null)
                        continue;

                    if (objectsInScene.Key.IsSpawned)
                        objectsInScene.Key.Despawn();

                    Destroy(objectsInScene.Key.gameObject);
                }

                _networkObjectsInScene.Clear();
                CheckExistSceneObjects();
                //StartCoroutine(CheckUsersAfterTimer());
            }
        }

        /// <summary>
        /// ONLY FOR PrefabObjectType.MovableObject STATIC OBJECTS
        /// </summary>
        /// <param name="staticObject"></param>
        /// <returns></returns>
        private NetworkObject CreateObjectPrototype(StaticObject staticObject)
        {
            //if (staticObject.PrefabObjectType != Enumenators.PrefabObjectType.MovableObject &&
                if (staticObject.PrefabObjectType != Enumenators.PrefabObjectType.CustomLogicObject)
                //staticObject.PrefabObjectType = (PrefabObjectType)18;
            {
                Debug.Log($"-OOO- CreateObjectPrototype {staticObject.ObjectKey} - {staticObject.PrefabObjectType} -- {staticObject.TowerObjectId}");
                return null;
            }
            if (string.IsNullOrEmpty(staticObject.ObjectKey))
                return null;

            NetworkObject tempMovableObject = Instantiate(_emptyPrefab);

            tempMovableObject.transform.position = staticObject.Vectors.Position.VectorComponentsToVector3();
            tempMovableObject.transform.rotation =
                Quaternion.Euler(staticObject.Vectors.Rotation.VectorComponentsToVector3());
            tempMovableObject.Spawn();
            tempMovableObject.DontDestroyWithOwner = true;

            _networkObjectsInScene.Add(tempMovableObject, staticObject.TowerObjectId.ToString());

            return tempMovableObject;
        }

        private void CreateCustomObject(StaticObject staticObject, NetworkObject networkObject)
        {
            //if (staticObject.PrefabObjectType != Enumenators.PrefabObjectType.MovableObject && 
            if (staticObject.PrefabObjectType != Enumenators.PrefabObjectType.CustomLogicObject)
                return;

            Debug.Log($"-OOO- CreateCustomObject {staticObject.ObjectKey}");

            NetworkObject tempMovableObject = Instantiate(networkObject);

            tempMovableObject.transform.position = staticObject.Vectors.Position.VectorComponentsToVector3();
            tempMovableObject.transform.rotation =
                Quaternion.Euler(staticObject.Vectors.Rotation.VectorComponentsToVector3());
            tempMovableObject.transform.localScale = staticObject.Vectors.Scale.VectorComponentsToVector3();

            tempMovableObject.Spawn();

            _networkObjectsInScene.Add(tempMovableObject, staticObject.TowerObjectId.ToString());
        }

        private void OnServerAuth()
        {
            CheckExistSceneObjects();
        }

        #region Server

        [ServerRpc(RequireOwnership = false)]
        private void GetMovableObjectsInSceneServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams();
            clientRpcParams.Send.TargetClientIds = new[] {clientId};

            foreach (KeyValuePair<NetworkObject, string> networkObjectInScene in _networkObjectsInScene)
            {
                SetMovableObjectsInSceneClientRpc(networkObjectInScene.Value, networkObjectInScene.Key,
                    clientRpcParams);
            }
        }

        #endregion

        #region Client

        [ClientRpc]
        private void SetMovableObjectsInSceneClientRpc(string TowerObectId,
            NetworkObjectReference networkObjectReference, ClientRpcParams clientRpcParams = default)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                _apiClientSideSceneCustomObjectsSpawner.SpawnMovableObject(TowerObectId, networkObject);
            }
        }

        #endregion

        #region Coroutine

        private IEnumerator CheckUsersAfterTimer()
        {
            yield return new WaitForSeconds(2f);
            foreach (KeyValuePair<NetworkObject, string> objectsInScene in _networkObjectsInScene)
            {
                objectsInScene.Key.Despawn();
                Destroy(objectsInScene.Key.gameObject);
            }

            _networkObjectsInScene.Clear();
            CheckExistSceneObjects();
        }

        #endregion
    }
}