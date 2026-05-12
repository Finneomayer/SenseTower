using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.Mirror_selfie;
using Assets.Mechanics.Network.Scripts;
using Assets.Mechanics.NetworkInteraction;
using Assets.Mechanics.NetworkInteraction.Services;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Infrastructure.Factory;
using Assets.Scripts.TowerObjects;
using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using Data;
using Infrastructure.AssetManagement;
using Mechanics.Inventory;
using Mechanics.Transactions;
using Mechanics.UserWallet;
using Oculus.Avatar2;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace Infrastructure.Factory
{
    public class NetworkFactory : NetworkBehaviour
    {
        public const string TwrCoinObjectClassName = "TWRCoin";
        public const string CameraObjectClassName = "Camera";
        public const string MirrorObjectClassName = "Mirror";
        public const string PadObjectClassName = "Tablet";

        #region inspector

        [SerializeField] private TransactionInfrastructure _transactionInfrastructure;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private TowerObjectUI _towerObjectUIPrefab;
        [SerializeField] private GameObject _TWRCoinPrefab;
        [SerializeField] private GameObject _CameraPrefab;

        [SerializeField] private List<Button> _createButtons;
        [SerializeField] private Button _despawnButton;
        [SerializeField] private PadFactory _padFactory;
        [SerializeField] private MirrorFactory _mirrorFactory;
        [SerializeField] private LocalItemsFactory _localItemsFactory;
        [SerializeField] private CustomLogicTowerObjectsSpawner _customLogicTowerObjectsSpawner;

        #endregion

        private Dictionary<GameObject, SpawnedItemInfo> _spawnedItemsInfo = new();
        private List<SpawnedItemInfo> _requestedItems = new();
        private List<NetworkItemType> _spawnedNetworkItems = new();
        private Dictionary<int, GameObject> _spawnedEmptyPrefabs = new();

        private Dictionary<string, Factory> _factories = new();

        private IClientData _clientData;
        private DiContainer _diContainer;
        private IGrabInteraction _grabInteraction;
        private GrabbingHand _grabbingHand;
        
        private GameObject _coinInstance;
        private GameObject _cameraInstance;
        private NetworkObject _cameraServerInstance;
        private ulong _cameraOwnerId = 0;

        private bool _factoryIsBusy;
        private bool _factoryIsCreating;
        public bool AreItemsSpawned => _spawnedItemsInfo.Count > 0;

        public event Action<GameObject, TowerObjectDto> ItemSpawned;
        public event Action<GameObject, TowerObjectDto> PhysicModelInstantiated;
        public event Action<TowerObjectDto> ItemDespawned;
        public event Action<GameObject,int,ulong> EmptyPrefabInstantiate;

        public event Action<string> CameraDespawn;
        public event Action<bool,GameObject> CameraCreate;
        [Inject]
        private void Construct(IClientData clientData, DiContainer diContainer, IGrabInteraction grabInteraction)
        {
            _grabInteraction = grabInteraction;
            _clientData = clientData;
            _diContainer = diContainer;
        }

        private void OnEnable()
        {
#if !UNITY_SERVER
            _padFactory.PadCreated += OnPadCreated;
            _padFactory.PadDestroyRequested += OnPadDestroyRequested;

            _mirrorFactory.MirrorCreated += OnMirrorCreated;
            _mirrorFactory.MirrorDestroyRequested += OnMirrorDestroyRequested;

            _customLogicTowerObjectsSpawner.ObjectSpawned += OnCustomLogicTowerObjectSpawned;
#endif
        }

        private void OnDisable()
        {
#if !UNITY_SERVER
            foreach (var item in _createButtons)
            {
                item.interactable = true;
                item.onClick.RemoveAllListeners();
            }
            
            _despawnButton.onClick.RemoveAllListeners();

            foreach (var item in _factories)
            {
                item.Value?.Clean();
            }

            _padFactory.PadCreated -= OnPadCreated;
            _padFactory.PadDestroyRequested -= OnPadDestroyRequested;

            _mirrorFactory.MirrorCreated -= OnMirrorCreated;
            _mirrorFactory.MirrorDestroyRequested -= OnMirrorDestroyRequested;

            _customLogicTowerObjectsSpawner.ObjectSpawned -= OnCustomLogicTowerObjectSpawned;
#endif
        }

        public GameObject GetLocalPrefab(TowerObjectDto towerObject)
        {
            //this part for Browser, Place, Blackboard, Levitation, Mirror, Tablet, Picture
            if (towerObject.PrefabObjectType.HasValue 
                && _localItemsFactory.TryGetPrefab(towerObject.PrefabObjectType.Value, out GameObject prefab))
            {
                return prefab;
            }

            //this part for Chess, AvatarRecorder, StopWatch etc.
            if (_customLogicTowerObjectsSpawner.TryGetInventoryPrefab(towerObject.ObjectKey, out prefab))
            {
                return prefab;
            }
            
            return null;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer) 
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            }
            if (IsClient)
            {
                RequestSpawnedItemsServerRpc(NetworkManager.LocalClientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }
        }

        public bool IsItemSpawned(TowerObjectDto towerObject)
        {
            if (towerObject == null)
            {
                return false;
            }
            var spawnedItem =
                _spawnedItemsInfo.FirstOrDefault(item => item.Value.TowerObject.Id == towerObject.Id
                && item.Value.TowerObject.TowerObjectClassName == towerObject.TowerObjectClassName);
            return spawnedItem.Key != null;
        }

        public bool TryDespawnItem(TowerObjectDto towerObject)
        {
            if (towerObject == null)
            {
                return false;
            }

            if (!IsItemSpawned(towerObject))
            {
                return false;
            }

            if (towerObject.TowerObjectClassName == TwrCoinObjectClassName)
            {
                DespawnCoin();
                return true;
            }

            if (towerObject.LoadingObjectType == LoadingObjectType.Remote)
            {
                DespawnRemoteObject(towerObject.Id);
                return true;
            }

            switch (towerObject.PrefabObjectType)
            {
                case null:
                    return false;
                case Enumenators.PrefabObjectType.Camera:
                    DispawnCameraOnServerRpc(NetworkManager.Singleton.LocalClientId);
                    return true;
                case Enumenators.PrefabObjectType.Tablet:
                    DespawnPad();
                    return true;
                case Enumenators.PrefabObjectType.Mirror:
                    DespawnMirror();
                    return true;
                case Enumenators.PrefabObjectType.CustomLogicObject:
                    DespawnCustomLogicObject(towerObject);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Method works on client when you take custom logic object from your shelf
        /// </summary>
        /// <param name="towerObject"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private async void CreateCustomLogicObject(TowerObjectDto towerObject, Vector3 position, Quaternion rotation)
        {
            Debug.LogError($"-OOO- NetworkFactory. Create custom logic object {towerObject.ObjectKey}");

            var spawnedItem =
                _spawnedItemsInfo.FirstOrDefault(element => element.Value.TowerObject.Id == towerObject.Id);

            if (spawnedItem.Key == null)
            {
                if (string.IsNullOrEmpty(towerObject.OwnerName))
                {
                    towerObject.OwnerName = _clientData.UserName;
                }

                if (TryAddRequestedItem(new SpawnedItemInfo(towerObject)))
                {
                    _customLogicTowerObjectsSpawner.SpawnWithLocalUserOwnership(towerObject, position, rotation);
                }
            }
            else
            {
                GameObject instance = spawnedItem.Key;
                Transform grabbableTransform;
                NetworkObject networkObject = instance.GetComponent<NetworkObject>();
                NetworkItemType networkItem = null;
                if (networkObject != null)
                {
                    networkItem = networkObject.GetComponent<NetworkItemType>();
                    if (_grabInteraction != null)
                    {
                        _grabInteraction.ChangeOwnership(networkObject, NetworkManager.Singleton.LocalClientId);
                        await UniTask.Delay(400);
                    }
                }

                if (networkItem != null && networkItem.AimObject != null)
                {
                    grabbableTransform = networkItem.AimObject;
                }
                else
                {
                    grabbableTransform = instance.transform;
                }

                if (_grabbingHand == null)
                {
                    grabbableTransform.position = position;
                }
                else
                {
                    grabbableTransform.SetPositionAndRotation(_grabbingHand.CurrentObjectAnchorPosition,
                        _grabbingHand.CurrentObjectAnchorRotation);
                }
            }
        }

        private void DespawnCustomLogicObject(TowerObjectDto towerObject)
        {
            var spawnedItem =
                _spawnedItemsInfo.FirstOrDefault(element => element.Value.TowerObject.Id == towerObject.Id);

            if (spawnedItem.Key != null)
            {
                RemoveSpawnedObject(spawnedItem.Key);
                _customLogicTowerObjectsSpawner.Despawn(towerObject.Id.ToString());
            }
        }

        public async UniTask<GameObject> GetRemoteModel(string remoteObjectKey, string remoteObjectRepositoryUrl)
        {
            if (_factoryIsBusy)
            {
                await UniTask.WaitUntil(() => !_factoryIsBusy);
            }
            
            _factoryIsBusy = true;
            remoteObjectKey = remoteObjectKey.Trim('/');

            var factory = await GetFactoryInstance(remoteObjectRepositoryUrl);

            GameObject model = await factory.LoadObject<GameObject>(remoteObjectKey);
            if (model == null)
            {
                model = await factory.CreateObject(remoteObjectKey);
            }

            _factoryIsBusy = false;

            return model;
        }

        private async UniTask<GameObject> InstantiateRemoteModel(string remoteObjectKey, string remoteObjectRepositoryUrl)
        {
            if (_factoryIsBusy)
            {
                await UniTask.WaitUntil(() => !_factoryIsBusy);
            }

            _factoryIsBusy = true;
            remoteObjectKey = remoteObjectKey.Trim('/');

            var factory = await GetFactoryInstance(remoteObjectRepositoryUrl);
            GameObject createdObject = await factory.CreateObject(remoteObjectKey);

            _factoryIsBusy = false;

            return createdObject;
        }

        public async UniTask<GameObject> GetRemoteModel(RemoteObjectTypeInfo remoteObjectTypeInfo)
        {
            if (remoteObjectTypeInfo == null)
            {
                return null;
            }
            return await GetRemoteModel(remoteObjectTypeInfo.ObjectKey, remoteObjectTypeInfo.ObjectRepositoryUrl);
        }

        public async UniTask<Factory> GetFactoryInstance(string remoteObjectRepositoryUrl)
        {
            if (_factoryIsCreating)
            {
                await UniTask.WaitUntil(() => !_factoryIsCreating);
            }

            remoteObjectRepositoryUrl = remoteObjectRepositoryUrl.Trim('/');

            if (!_factories.TryGetValue(remoteObjectRepositoryUrl, out Factory factory))
            {
                _factoryIsCreating = true;
                bool isFactoryInitialized = false;

                factory = new Factory(new AddressableResourcesLocation(
                    ResourcesLocation.GetRemoteObjectCatalogPath(remoteObjectRepositoryUrl),
                    () => { isFactoryInitialized = true; }));

                await UniTask.WaitUntil(() => isFactoryInitialized);

                _factories[remoteObjectRepositoryUrl] = factory;
                _factoryIsCreating = false;
            }
            return factory;
        }

        public void CreateCurrentUserCoin(TowerObjectDto towerObject, Vector3 position, Quaternion rotation, bool isGrabbing = true)
        {
            if (towerObject == null)
            {
                towerObject = new TowerObjectDto();
                towerObject.Id = Guid.Empty;
                towerObject.TowerObjectClassName = TwrCoinObjectClassName;
            }

            if (_coinInstance != null)
            {
                if (_grabbingHand == null)
                {
                    _coinInstance.transform.SetPositionAndRotation(position, rotation);
                }
                else
                {
                    _coinInstance.transform.SetPositionAndRotation(_grabbingHand.CurrentObjectAnchorPosition,
                        _grabbingHand.CurrentObjectAnchorRotation);
                }
            }
            else
            {
                var userItem = new SpawnedItemInfo(towerObject);
                if (TryAddRequestedItem(userItem))
                {
                    CreateCoinServerRpc(NetworkManager.Singleton.LocalClientId, position, rotation,isGrabbing);
                }
            }
        }

        public void SetCurrentGrabbingHand(GrabbingHand grabbingHand)
        {
            _grabbingHand = grabbingHand;
        }

        public void CreateTowerObject(GrabbingHand grabbingHand, TowerObjectDto towerObject, 
            Vector3 position, Quaternion rotation)
        {
            if (towerObject == null)
            {
                return;
            }
            SetCurrentGrabbingHand(grabbingHand);

            if (towerObject.TowerObjectClassName == TwrCoinObjectClassName)
            {
                CreateCurrentUserCoin(towerObject, position, rotation);
                return;
            }

            if (towerObject.LoadingObjectType == LoadingObjectType.Remote)
            {
                CreateNetworkObject(towerObject, position);
                return;
            }

            switch (towerObject.PrefabObjectType)
            {
                case null:
                    return;
                case Enumenators.PrefabObjectType.Camera:
                    CreateCamera(towerObject, position, rotation);
                    break;
                case Enumenators.PrefabObjectType.Tablet:
                    CreatePad(towerObject, position, rotation);
                    break;
                case Enumenators.PrefabObjectType.Mirror:
                    CreateMirror(towerObject, position, rotation); 
                    break;
                case Enumenators.PrefabObjectType.CustomLogicObject:
                    CreateCustomLogicObject(towerObject, position, rotation);
                    break;
            }
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            _spawnedNetworkItems.RemoveAll(item => item == null || item.NetworkObject.OwnerClientId == clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateCoinServerRpc(ulong clientId, Vector3 position, Quaternion rotation, bool isGrabbing)
        {
            var instance = Instantiate(_TWRCoinPrefab);

            instance.transform.SetPositionAndRotation(position, rotation);
            NetworkObject networkObject = instance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(clientId);
            CreateCoinOnClientRpc(networkObject, isGrabbing);
        }

        [ClientRpc]
        private void CreateCoinOnClientRpc(NetworkObjectReference networkObjectReference, bool isGrabbing)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    var requestedObject = GetRequestedObjectByClassName(TwrCoinObjectClassName);
                    AddSpawnedObject(networkObject.gameObject, requestedObject, isGrabbing);
                    _coinInstance = networkObject.gameObject;
                    
                    if (_coinInstance.TryGetComponent(out CoinInfrastructure coinInfrastructure))
                    {
                        coinInfrastructure.TransactionInfrastructure = _transactionInfrastructure;
                        coinInfrastructure.NetworkFactory = this;
                    }

                    PhysicModelInstantiated?.Invoke(_coinInstance, requestedObject);
                }
            }
        }

        private TowerObjectDto GetRequestedObjectById(Guid id)
        {
            var requestedObj = _requestedItems.FirstOrDefault(
                (x) => x.TowerObject.Id == id);
            return requestedObj != null ? requestedObj.TowerObject : null;
        }

        private TowerObjectDto GetRequestedObjectByClassName(string className)
        {
            var requestedObj = _requestedItems.FirstOrDefault((x) => x.TowerObject.TowerObjectClassName == className);
            return requestedObj != null ? requestedObj.TowerObject : null;
        }

        private TowerObjectDto GetRequestedObjectByType(Enumenators.PrefabObjectType prefabObjectType)
        {
            var requestedObj = _requestedItems.FirstOrDefault((x) => x.TowerObject.PrefabObjectType == prefabObjectType);
            return requestedObj != null ? requestedObj.TowerObject : null;
        }

        [ContextMenu("OnEditor Camera Create")]
        public void OnEditorCameraCreate()
        {
            CreateCamera(new Vector3(0.3f,1f,4f), Quaternion.identity);
        }

        public GameObject GetCamera()
        {
            if (_cameraServerInstance == null)
                return null;
            
            return _cameraServerInstance.gameObject;
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateNetworkCameraServerRpc(ulong clientId,Vector3 position, Quaternion rotation, string towerObjectId)
        {
            if (_cameraServerInstance == null)
            {
                GameObject tempObject = Instantiate(_prefab);
                if(tempObject.TryGetComponent(out NetworkObject networkObject))
                {
                    _cameraServerInstance = networkObject;
                    networkObject.SpawnWithOwnership(clientId);
                    _cameraOwnerId = networkObject.OwnerClientId;
                    networkObject.transform.SetPositionAndRotation(position,rotation);
                    networkObject.DontDestroyWithOwner = true;
                    SetCameraStateClientRpc(networkObject, towerObjectId);
                }
            }
            else if (_cameraServerInstance != null)
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams();
                clientRpcParams.Send.TargetClientIds = new[] {clientId};
                SetCameraStateClientRpc(_cameraServerInstance, towerObjectId, clientRpcParams);
            }
        }

        public void ChangeOwnerNetworkCamera(ulong clientId)
        {
            if (_cameraServerInstance != null)
            {
                _cameraServerInstance.ChangeOwnership(clientId);
                _cameraOwnerId = clientId;
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DispawnCameraOnServerRpc(ulong clientId)
        {
            TryDespawnNetworkCamera(clientId);
        }

        public void ForceDespawnNetworkCamera()
        {
            if (_cameraServerInstance != null)
            {
                TryDespawnNetworkCamera(_cameraServerInstance.OwnerClientId);
            }
        }
        private void CreateMirror(TowerObjectDto towerObject, Vector3 position, Quaternion rotation)
        {
            if (TryGetSpawnedInstance(towerObject.PrefabObjectType, out GameObject spawnedInstance))
            {
                var grabbable = spawnedInstance.GetComponent<MirrorSelfie>().MirrorGrabable;
                if (grabbable != null)
                {
                    grabbable.transform.SetPositionAndRotation(position, rotation);
                }
                else
                {
                    spawnedInstance.transform.SetPositionAndRotation(position, rotation);
                }
            }
            else
            {
                if (TryAddRequestedItem(new SpawnedItemInfo(towerObject)))
                {
                    _mirrorFactory.CreateMirror(position, rotation);
                }
            }
        }

        public void DespawnMirror()
        {
            if (TryGetSpawnedInstance(Enumenators.PrefabObjectType.Mirror, out GameObject spawnedInstance))
            {
                RemoveSpawnedObject(spawnedInstance);
            }

            _mirrorFactory.DespawnMirror();
        }

        public void TryDespawnNetworkCamera(ulong clientId)
        {
            if (_cameraServerInstance != null && _cameraOwnerId == clientId)
            {
                _cameraOwnerId = 0;
                Destroy(_cameraServerInstance.gameObject);
                DespawnCameraClientRpc();
            }
        }

        [ClientRpc]
        private void DespawnCameraClientRpc()
        {
            DespawnCamera();
        }

        [ClientRpc]
        private void SetCameraStateClientRpc(NetworkObjectReference cameraObjectReference, 
            string towerObjectId, ClientRpcParams clientRpcParams = default)
        {
            if (cameraObjectReference.TryGet(out NetworkObject networkObject))
            {
                if (_cameraInstance == null)
                {
                    _cameraInstance = _diContainer.InstantiatePrefab(_CameraPrefab);
                    _cameraInstance.transform.SetPositionAndRotation(networkObject.transform.position, networkObject.transform.rotation);
                }

                if (networkObject.TryGetComponent(out NetworkItemType networkItemType) && _cameraInstance.TryGetComponent(out CameraCreator cameraCreator))
                {
                    networkItemType.SetAimObject(cameraCreator.GetCameraGrab().transform);
                    cameraCreator.CameraId = towerObjectId;
                }

                CameraCreate?.Invoke(networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId, _cameraInstance);
            }
        }

        public void CreateCamera(Vector3 position, Quaternion rotation)
        {
            var towerObject = new TowerObjectDto();
            towerObject.TowerObjectClassName = CameraObjectClassName;
            //towerObject.BehaviorType = TowerObjectBehaviorType.Movable;
            towerObject.PrefabObjectType = Enumenators.PrefabObjectType.Camera;
            towerObject.Id = Guid.Empty;
            towerObject.OwnerBusinessUnitType = BusinessUnitType.User;

            CreateCamera(towerObject, position, rotation);
        }

        public void CreateCamera(TowerObjectDto towerObject, Vector3 position, Quaternion rotation)
        {
            if (_cameraInstance != null)
            {
                if (_cameraInstance.TryGetComponent(out CameraCreator cameraCreator))
                {
                    if (!cameraCreator._cameraService._recState)
                    {
                        var grabbable = _cameraInstance.GetComponentInChildren<RecordingCameraGrabbable>();
                        if (grabbable != null)
                        {
                            grabbable.transform.SetPositionAndRotation(position, rotation);
                        }
                        else
                        {
                            _cameraInstance.transform.SetPositionAndRotation(position, rotation);
                        } 
                    }
                }
            }
            else
            {
                if (TryAddRequestedItem(new SpawnedItemInfo(towerObject)))
                {
                    _cameraInstance = _diContainer.InstantiatePrefab(_CameraPrefab);
                    _cameraInstance.transform.SetPositionAndRotation(position, rotation);

                    CreateNetworkCameraServerRpc(NetworkManager.Singleton.LocalClientId, position, rotation, towerObject.Id.ToString());
                    AddSpawnedObject(_cameraInstance, towerObject);
                    PhysicModelInstantiated?.Invoke(_cameraInstance, towerObject);
                }
            }
        }

        public void DespawnCoin()
        {
            DespawnNetworkObject(_coinInstance);
            _coinInstance = null;
        }

        private void CreatePad(TowerObjectDto towerObject, Vector3 position, Quaternion rotation)
        {
            if (TryGetSpawnedInstance(towerObject.PrefabObjectType, out GameObject spawnedInstance))
            {
                var grabbable = spawnedInstance.GetComponent<PresentationPad>().PadGrabbable;
                if (grabbable != null)
                {
                    grabbable.transform.SetPositionAndRotation(position, rotation);
                }
                else
                {
                    spawnedInstance.transform.SetPositionAndRotation(position, rotation);
                }
            }
            else
            {
                if (TryAddRequestedItem(new SpawnedItemInfo(towerObject)))
                {
                    _padFactory.CreatePad(position, rotation);
                }
            }
        }

        public void DespawnPad()
        {
            if (TryGetSpawnedInstance(Enumenators.PrefabObjectType.Tablet, out GameObject spawnedInstance))
            {
                RemoveSpawnedObject(spawnedInstance);
            }

            _padFactory.DespawnPad();
        }

        private void OnPadCreated(GameObject padInstance)
        {
            var requestedObject = GetRequestedObjectByType(Enumenators.PrefabObjectType.Tablet);
            AddSpawnedObject(padInstance, requestedObject);
            PhysicModelInstantiated?.Invoke(padInstance, requestedObject);
        }

        private void OnPadDestroyRequested()
        {
            DespawnPad();
        }
        private void OnMirrorCreated(GameObject mirrorInstance)
        {
            var requestedObject = GetRequestedObjectByType(Enumenators.PrefabObjectType.Mirror);
            AddSpawnedObject(mirrorInstance, requestedObject);
            PhysicModelInstantiated?.Invoke(mirrorInstance, requestedObject);
        }

        private void OnMirrorDestroyRequested()
        {
            DespawnMirror();
        }

        private void OnCustomLogicTowerObjectSpawned(string towerObjectId)
        {
            if (!_customLogicTowerObjectsSpawner.TryGetSpawnedObjectData(towerObjectId, out SpawnedObjectData spawnedObjectData))
            {
                return;
            }

            NetworkObject networkObject = spawnedObjectData.NetworkObject;

            if (networkObject.TryGetComponent(out NetworkXrGrab xrGrabInteractable))
            {
                xrGrabInteractable.Init(networkObject, _grabInteraction);
            }

            if (networkObject.TryGetComponent(out OnObjectData objectPublicData))
            {
                objectPublicData.ThisTowerObjectId = towerObjectId;
            }

            CreateUIForObject(networkObject.gameObject, spawnedObjectData.OwnerName, spawnedObjectData.OwnerId);

            if (networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                var requestedItem = GetRequestedObjectById(Guid.Parse(towerObjectId));
                AddSpawnedObject(networkObject.gameObject, requestedItem);
                PhysicModelInstantiated?.Invoke(networkObject.gameObject, requestedItem);
            }
        }

        private bool TryGetSpawnedInstance(Enumenators.PrefabObjectType? prefabObjectType, 
            out GameObject spawnedInstance)
        {
            spawnedInstance = null;
            if (prefabObjectType == null)
            {
                return false;
            }

            foreach (var item in _spawnedItemsInfo)
            {
                if (item.Value != null && item.Value.TowerObject != null
                    && item.Value.TowerObject.PrefabObjectType == prefabObjectType)
                {
                    spawnedInstance = item.Key;
                    break;
                }
            }

            return spawnedInstance != null;
        }
        
        public async void CreateNetworkObject(TowerObjectDto userItem, Vector3 position)
        {
            var remoteItemElement = 
                _spawnedItemsInfo.FirstOrDefault(element => element.Value.TowerObject.Id == userItem.Id);

            if (remoteItemElement.Key == null)
            {
                if (userItem.RemoteObjectTypeInfo == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(userItem.OwnerName))
                {
                    userItem.OwnerName = _clientData.UserName;
                }

                if (TryAddRequestedItem(new SpawnedItemInfo(userItem)))
                {
                    SpawnPrefabServerRpc(NetworkManager.Singleton.LocalClientId, position,userItem.OwnerName,userItem.OwnerId.ToString(),
                        userItem.Id.ToString(), userItem.RemoteObjectTypeInfo.ObjectKey, userItem.RemoteObjectTypeInfo.ObjectRepositoryUrl);
                }
            }
            else
            {
                var networkObject = remoteItemElement.Key;
                var networkItem = networkObject.GetComponent<NetworkItemType>();
                
                if (networkItem != null && networkItem.AimObject != null)
                {
                    if(_grabInteraction != null)
                        _grabInteraction.ChangeOwnership(networkItem.NetworkObject,NetworkManager.Singleton.LocalClientId);
                    await UniTask.Delay(400);
                    if (_grabbingHand == null)
                    {
                        networkItem.AimObject.position = position;
                    }
                    else
                    {
                        networkItem.AimObject.SetPositionAndRotation(_grabbingHand.CurrentObjectAnchorPosition,
                            _grabbingHand.CurrentObjectAnchorRotation);
                    }
                }
            }
        }

        public void DespawnAllObjects()
        {
            SpawnedItemInfo[] infos = _spawnedItemsInfo.Values.ToArray();
            foreach (var info in infos)
            {
                if (info != null)
                {
                    TryDespawnItem(info.TowerObject);
                }
            }

            //TODO: Remove if there are no ghost cameras in scene
            DispawnCameraOnServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
        public void DespawnCamera()
        {
            if (_cameraInstance == null)
            {
                return;
            }

            var towerObjectId = _cameraInstance.GetComponent<CameraCreator>().CameraId;
            RemoveSpawnedObject(_cameraInstance);
            Destroy(_cameraInstance);
            _cameraInstance = null;
            CameraDespawn?.Invoke(towerObjectId);
        }

        private void DespawnNetworkObject(GameObject networkGameObject)
        {
            if (networkGameObject == null)
            {
                return;
            }
            RemoveSpawnedObject(networkGameObject);

            if (networkGameObject.TryGetComponent(out NetworkObject networkObject))
            {
                DespawnNetworkItemServerRpc(networkObject);
            }
        }

        private void DespawnRemoteObjects()
        {
            List<GameObject> objectsToDespawn = new();
            foreach (var item in _spawnedItemsInfo)
            {
                if (item.Value.TowerObject != null &&
                    item.Value.TowerObject.LoadingObjectType == LoadingObjectType.Remote)
                {
                    objectsToDespawn.Add(item.Key);
                }
            }

            foreach (var item in objectsToDespawn)
            {
                DespawnNetworkObject(item);
            }
        }

        private void DespawnRemoteObject(Guid towerObjectId)
        {
            foreach (var item in _spawnedItemsInfo)
            {
                if (item.Value.TowerObject != null && item.Value.TowerObject.Id == towerObjectId
                    && item.Value.TowerObject.LoadingObjectType == LoadingObjectType.Remote)
                {
                    DespawnNetworkObject(item.Key);
                    return;
                }
            }
        }

        #region Client

        [ClientRpc]
        private void EmptyPrefabInstantiateClientRpc(NetworkObjectReference networkObjectReference, int objectId)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                EmptyPrefabInstantiate?.Invoke(networkObject.gameObject,objectId,networkObject.OwnerClientId);
            }
        }

        private async UniTask<GameObject> CreateObjectOnClient(NetworkObject networkObject, string userNickName,string ownerId,
            string remoteObjectKey, string remoteObjectRepositoryUrl)
        {
            GameObject tempGameObject = await CreateDefaultArchetype(networkObject.OwnerClientId, networkObject,
                remoteObjectKey, remoteObjectRepositoryUrl);
            CreateUIForObject(tempGameObject,userNickName,ownerId);
            return tempGameObject;
        }

        private void CreateUIForObject(GameObject parentObj, string userNickName,string ownerId)
        {
            var tempIU = _diContainer.InstantiatePrefab(_towerObjectUIPrefab);
            if (tempIU.TryGetComponent(out TowerObjectUI towerObjectUI))
            {
                towerObjectUI.SetText(userNickName);
                towerObjectUI.SetParent(parentObj);
                towerObjectUI.SetOwner(ownerId);
                if (parentObj.TryGetComponent(out NetworkXrGrab xrGrabInteractable))
                {
                    towerObjectUI.OnActionListener(xrGrabInteractable);
                }
            }
            
            tempIU.transform.localPosition = new Vector3(0, 0.25f, 0);
            tempIU.transform.localRotation = Quaternion.identity;
        }
        
        private async UniTask<GameObject> CreateDefaultArchetype(ulong uid, NetworkObject objectParent,
            string remoteObjectKey, string remoteObjectRepositoryUrl)
        {
            GameObject networkPrefabLoading = await InstantiateRemoteModel(remoteObjectKey, remoteObjectRepositoryUrl);

            CreateXRArchetype(uid, networkPrefabLoading, objectParent);

            return networkPrefabLoading;
        }

        private void CreateXRArchetype(ulong uid, GameObject networkPrefabLoading, NetworkObject objectParent)
        {
            networkPrefabLoading.transform.position = objectParent.transform.position;
            networkPrefabLoading.transform.rotation = objectParent.transform.rotation;
            if (!networkPrefabLoading.TryGetComponent(out NetworkXrGrab xrGrabInteractable))
            {
                xrGrabInteractable = networkPrefabLoading.AddComponent<NetworkXrGrab>();
            }
            
            xrGrabInteractable.interactionLayers = InteractionLayerMask.GetMask("Grab");

            xrGrabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            xrGrabInteractable.throwVelocityScale = 3f;
            xrGrabInteractable.useDynamicAttach = true;
            xrGrabInteractable.matchAttachPosition = true;
            xrGrabInteractable.matchAttachRotation = true;
            xrGrabInteractable.Init(objectParent,_grabInteraction);

            foreach (var item in xrGrabInteractable.colliders)
            {
                item.gameObject.layer = 20;
            }

            var children = networkPrefabLoading.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.gameObject.name == "beer_inner") child.AddComponent<Wobble>().MaxWobble = 0.5f;
                if (child.gameObject.name == "Anchor")
                {
                    if (networkPrefabLoading.TryGetComponent(out XRGrabInteractable xRGrab))
                        xRGrab.attachTransform = child;
                }
            }

            if (uid == NetworkManager.Singleton.LocalClientId)
                _despawnButton.gameObject.SetActive(true);

            if (objectParent.TryGetComponent(out NetworkItemType networkItem))
            {
                networkItem.SetAimObject(networkPrefabLoading.transform);
            }
        }

        private void AddSpawnedObject(GameObject spawnedGO, TowerObjectDto towerObject, bool isGrabbing = true)
        {
            if (towerObject == null)
            {
                Debug.LogError("AddSpawnedObject. towerObject == null");
                return;
            }

            if (_grabbingHand != null && isGrabbing)
            {
                spawnedGO.transform.position = _grabbingHand.transform.position;
                spawnedGO.transform.SetPositionAndRotation(_grabbingHand.CurrentObjectAnchorPosition,
                    _grabbingHand.CurrentObjectAnchorRotation);
            }

            SpawnedItemInfo newItem = new(towerObject);
            RemoveRequestedItem(newItem);

            _spawnedItemsInfo[spawnedGO] = newItem;
            ItemSpawned?.Invoke(spawnedGO, towerObject);
        }

        private void RemoveSpawnedObject(GameObject spawnedGO)
        {
            if (_spawnedItemsInfo.TryGetValue(spawnedGO, out SpawnedItemInfo itemInfo))
            {
                RemoveRequestedItem(itemInfo);
                _spawnedItemsInfo.Remove(spawnedGO);
                ItemDespawned?.Invoke(itemInfo.TowerObject);
            }
        }

        private bool IsItemRequested(SpawnedItemInfo itemInfo)
        {
            return FindRequestedItem(itemInfo) != null;
        }

        private SpawnedItemInfo FindRequestedItem(SpawnedItemInfo itemInfo)
        {
            if (itemInfo.TowerObject.Id != Guid.Empty)
            {
                return _requestedItems.FirstOrDefault((x) => x.TowerObject.Id == itemInfo.TowerObject.Id);
            }

            return _requestedItems.FirstOrDefault((x) => 
                x.TowerObject.PrefabObjectType == itemInfo.TowerObject.PrefabObjectType
                || x.TowerObject.TowerObjectClassName == itemInfo.TowerObject.TowerObjectClassName);
        }

        private bool TryAddRequestedItem(SpawnedItemInfo itemInfo)
        {
            if (IsItemRequested(itemInfo))
            {
                return false;
            }

            _requestedItems.Add(itemInfo);
            return true;
        }

        private void RemoveRequestedItem(SpawnedItemInfo itemInfo)
        {
            var requestedItem = FindRequestedItem(itemInfo);
            if (requestedItem != null)
            {
                _requestedItems.Remove(requestedItem);
            }
        }

        [ClientRpc]
        private void SetGraphicsNetworkObjectClientRpc(NetworkObjectReference networkObjectReference, string userNickName,string OwnerId,
            string towerObjectId, string remoteObjectKey, string remoteObjectRepositoryUrl, 
            ClientRpcParams clientRpcParams = default)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.gameObject.TryGetComponent(out NetworkItemType networkItem))
                {
                    if (!_spawnedNetworkItems.Contains(networkItem))
                    {
                        _spawnedNetworkItems.Add(networkItem);
                    }
                }

                if (networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    var requestedItem = GetRequestedObjectById(Guid.Parse(towerObjectId));
                    AddSpawnedObject(networkObject.gameObject, requestedItem);
                    CreateObjectOnClientForOwner(networkObject, requestedItem).Forget();
                }
                else
                {
                    CreateObjectOnClient(networkObject,userNickName, OwnerId,remoteObjectKey, remoteObjectRepositoryUrl).Forget();
                }
            }
        }

        private async UniTask CreateObjectOnClientForOwner(NetworkObject networkObject, TowerObjectDto itemInfo)
        {
            if (itemInfo.RemoteObjectTypeInfo == null)
            {
                return;
            }
            GameObject model = await CreateObjectOnClient(networkObject, itemInfo.OwnerName,itemInfo.OwnerId.ToString(),itemInfo.RemoteObjectTypeInfo.ObjectKey,
                itemInfo.RemoteObjectTypeInfo.ObjectRepositoryUrl);
            PhysicModelInstantiated?.Invoke(model, itemInfo);
        }

        [ClientRpc]
        private void RemoveNetworkItemClientRpc(NetworkObjectReference networkObjectReference)
        {
            _spawnedNetworkItems.RemoveAll(item => item == null);

            if (!networkObjectReference.TryGet(out NetworkObject networkObject) || networkObject == null)
            {
                return;
            }

            _spawnedNetworkItems.RemoveAll(item => item.gameObject == networkObject.gameObject);
        }

        #endregion

        #region Server

        [ServerRpc(RequireOwnership = false)]
        private void RequestSpawnedItemsServerRpc(ulong clientId)
        {
            if (!NetworkManager.ConnectedClientsIds.Contains(clientId))
            {
                return;
            }

            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            foreach (var networkItem in _spawnedNetworkItems)
            {
                if (networkItem == null)
                {
                    continue;
                }
                
                SetGraphicsNetworkObjectClientRpc(networkItem.gameObject,networkItem.UserNickName,
                    networkItem.OwnerId, networkItem.TowerObjectId, networkItem.RemoteObjectKey,
                    networkItem.RemoteObjectRepositoryUrl, clientRpcParams);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPrefabServerRpc(ulong uId, Vector3 position, string userNickName,string ownerId,string towerObjectId, string remoteObjectKey, 
            string remoteObjectRepositoryUrl)
        {
            GameObject tempInstance = Instantiate(_prefab);
            tempInstance.transform.position = position;
            NetworkObject networkObject = tempInstance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(uId);

            NetworkItemType networkItem = tempInstance.GetComponent<NetworkItemType>();
            if (networkItem != null)
            {
                networkItem.Init(ownerId,towerObjectId, remoteObjectKey, remoteObjectRepositoryUrl,userNickName);
                _spawnedNetworkItems.Add(networkItem);
            }

            SetGraphicsNetworkObjectClientRpc(tempInstance, userNickName,ownerId,towerObjectId, remoteObjectKey, remoteObjectRepositoryUrl);
        }

        [ServerRpc(RequireOwnership = false)]
        public void GetEmptyPrefabServerRpc()
        {
            if (_spawnedEmptyPrefabs.Count == 0)
                return;
            
            foreach (var networkPrefab in _spawnedEmptyPrefabs)
            {
                if (networkPrefab.Value != null)
                {
                    if (networkPrefab.Value.TryGetComponent(out NetworkObject networkObject))
                    {
                        EmptyPrefabInstantiateClientRpc(networkObject, networkPrefab.Key);
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnEmptyPrefabWithIdServerRpc(ulong uId,int objectId, Vector3 position)
        {
            GameObject tempInstance = Instantiate(_prefab);
            tempInstance.transform.position = position;
            NetworkObject networkObject = tempInstance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(uId);
            _spawnedEmptyPrefabs[objectId] = tempInstance;
            EmptyPrefabInstantiateClientRpc(networkObject, objectId);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DespawnNetworkItemServerRpc(NetworkObjectReference networkObjectReference)
        {
            _spawnedNetworkItems.RemoveAll(item => item == null);
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                _spawnedNetworkItems.RemoveAll(item => item.gameObject == networkObject.gameObject);
                RemoveNetworkItemClientRpc(networkObjectReference);

                networkObject.Despawn();
            }
        }

        #endregion

        private class SpawnedItemInfo
        {
            public TowerObjectDto TowerObject { get; }

            public SpawnedItemInfo(TowerObjectDto towerObject)//, string remoteItemName = "")
            {
                TowerObject = towerObject;
            }
        }
    }
}