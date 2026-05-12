using System;
using System.Collections.Generic;
using Assets.Scripts.Zones;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerFillingScene : NetworkBehaviour
{
    #region Inspector
    [SerializeField] private Place _placePrefab;
    [SerializeField] private ZonesModel zonesModel;
    
    [SerializeField] private GameObject _sallyPref;
    #endregion

    //private const string RemoteContentUrlKey = "RemoteContent_Url";
    private const string RemoteSceneKey = "RemoteSceneKey";
    private const string RemoteFolderKey = "RemoteFolderKey";
    private const string RemoteCatalogKey = "RemoteCatalogKey";

    private GameObject _landingPlace;
    private SceneFactory _sceneFactory;

    private List<GameObject> networkgameObjects = new List<GameObject>();

    //private string _remoteSceneName;
    //private string _remoteFolderName;
    //private string _remoteCatalogName;
    //private string _remoteContentUrl;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (IsHost)
        {
            Filling("https://storage.yandexcloud.net/st-scenes/dev");
            return;
        }

        InitializeServer().Forget();
    }

    private async UniTask InitializeServer()
    {
        string remoteFolderName = Environment.GetEnvironmentVariable(RemoteFolderKey);
        string remoteCatalogName = Environment.GetEnvironmentVariable(RemoteCatalogKey);
        string remoteSceneName = Environment.GetEnvironmentVariable(RemoteSceneKey);
        //_remoteContentUrl = Environment.GetEnvironmentVariable(RemoteContentUrlKey);

        //_remoteFolderName = "YachtScene";
        //_remoteCatalogName = "catalog_YachtScene.json";
        //_remoteSceneName = "YachtScene";
        //_remoteContentUrl = "https://storage.yandexcloud.net/st-scenes/dev";

        if (string.IsNullOrEmpty(remoteSceneName)) return;

        await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.RemoteContentUrl));
        string remoteContentUrl = ServerApiService.RemoteContentUrl;

        ResourcesLocation.SetRemoteBasePath(remoteContentUrl);

        string remotePath = ResourcesLocation.GetRemoteScenePath(remoteFolderName, remoteCatalogName);
        _sceneFactory = new SceneFactory(new AddressableResourcesLocation(remotePath, () => Filling(remoteSceneName)));
    }

    private async void Filling(string remoteSceneName)
    {
        if (_sceneFactory != null)
        {
            await _sceneFactory.LoadSceneAsync(remoteSceneName, loadMode: LoadSceneMode.Additive);
            await _sceneFactory.ActiveSceneAsync();
        }

        _landingPlace = GameObject.Find("LandingPlace");
        
        CreateDynamicObject();

        _sceneFactory?.Cleanup();
    }


    private GameObject yacht;
    private void CreateDynamicObject()
    {
        var _dynamicYacht = GameObject.Find("DynamicYacht");
        
        if (_dynamicYacht != null)
        {
            if (_dynamicYacht.transform.childCount > 0)
                Destroy(_dynamicYacht.transform.GetChild(0).gameObject);

            yacht = Instantiate(_sallyPref, _dynamicYacht.transform.position, _dynamicYacht.transform.rotation);
            if (!networkgameObjects.Contains(yacht))
            {
                networkgameObjects.Add(yacht);
                if (yacht.TryGetComponent(out NetworkObject networkObject))
                {
                    var path = FindObjectOfType<PathCreation.PathCreator>();
                    var pathGO = new GameObject("Path");
                    path.transform.parent = pathGO.transform;
                    yacht.GetComponent<Sense.RemouteScene.PathFollower>().Init(path);

                    networkObject.Spawn();
                }
            }
            SetChairOnScene(yacht.transform);
        }
    }

    private void SetChairOnScene(Transform chairParent)
    {
        if (_landingPlace == null) return;

        for (int i = 0; i < _landingPlace.transform.childCount; i++)
        {
            GameObject chairGameObject = _landingPlace.transform.GetChild(i).gameObject;
            Vector3 tempPosition = new Vector3(chairGameObject.transform.position.x, chairGameObject.transform.position.y + 0.12f, chairGameObject.transform.position.z);
            Place newPlace = Instantiate(_placePrefab);
            
            newPlace.Init(zonesModel,null);
            newPlace.NetworkObject.Spawn();
            newPlace.transform.parent = chairParent == null ? zonesModel.transform : chairParent;
            newPlace.transform.position = tempPosition;
            newPlace.transform.rotation = chairGameObject.transform.rotation;
           // newPlace.transform.parent = chairGameObject.transform;
        }

        if (yacht != null)
        {
            zonesModel.transform.parent = yacht.transform;
        }
    }

    private void OnDisable()
    {
        if (_sceneFactory != null)
            _sceneFactory.Cleanup();
    }
}
