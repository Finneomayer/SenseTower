using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Scripts.API;
using Assets.Scripts.Data;
using Assets.Scripts.Space;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Mechanics.LoadSceneObjects.Interfaces;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static Data.Enumenators;

namespace Mechanics.LoadSceneObjects
{
    public class ClientSideRemoteSceneObjectSpawner : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private List<GameObject> _defaultEnviromentTheme;
        [SerializeField] private GameObject _levitationZonePrefab;
        [SerializeField] private PicturePanel _spacePicturePrefab;
        [SerializeField] private AddressablesSceneObject _sceneObjectPrefab;
        [SerializeField] private NetworkStartInvoker _networkInvoker;

        #endregion

        public event Action<PicturePanel[]> PicturePanelSpawned; 
        private List<SceneObject> _sceneObjects = new();
        private List<AddressablesSceneObject> _spawnedObjects = new();

        private ISpaceManager _spaceManager;
        private int _spawnedObjectCount;
        private SceneFactory _sceneFactory;

        [Inject]
        public void Construct(ISpaceManager spaceManager)
        {
            _spaceManager = spaceManager;
        }

        private void Start()
        {
#if !UNITY_SERVER
            Invoke(nameof(GetRemoteCatalog), 1);
#endif
        }

        private void OnDisable()
        {
            for (int i = 0; i < _spawnedObjects.Count; i++)
            {
                _spawnedObjects[i].OnCreate -= OnObjectCreate;
            }
        }

        public void CheckExistSceneObjects()
        {
            if (_sceneObjects.Count == 0) return;

            SceneObject tempScene = _sceneObjects.Find(e => e.ObjectType == RemoteContentType.Scene);

            if (tempScene == null)
                SpawnSceneObjects();
            else
            {
                LoadRemoteEnviroment(tempScene);
            }
        }

        //a3db6e1f-4da8-4feb-8ca2-691137d32af5.json
        public void SpawnSceneObjects()
        {
            try
            {
                for (int i = 0; i < _sceneObjects.Count; i++)
                {
                    SceneObject tempSceneObject = _sceneObjects[i];

                    if (tempSceneObject.PrefabObjectType == PrefabObjectType.Picture)
                        continue;
                    
                    if (tempSceneObject.ObjectKey.Contains("Levitation"))
                    {
                        Vector3 position = new Vector3(tempSceneObject.X, tempSceneObject.Y, tempSceneObject.Z);
                        Vector3 rotation = new Vector3(tempSceneObject.Xr, tempSceneObject.Yr, tempSceneObject.Zr);
                        Vector3 scale = new Vector3(tempSceneObject.Xs, tempSceneObject.Ys, tempSceneObject.Zs);

                        //SpawnLevitation(position,rotation,scale);
                        continue;
                    }

                    AddressablesSceneObject tempAddressablesSceneObject = Instantiate(_sceneObjectPrefab);
                    _spawnedObjects.Add(tempAddressablesSceneObject);

                    tempAddressablesSceneObject.OnCreate += OnObjectCreate;
                    tempAddressablesSceneObject.Init(
                        ResourcesLocation.GetRemoteSceneObjectCatalogPath(tempSceneObject.RerositoryUrl));
                    tempAddressablesSceneObject.Create(
                        tempSceneObject.ObjectKey,
                        new Vector3(tempSceneObject.X, tempSceneObject.Y, tempSceneObject.Z),
                        new Vector3(tempSceneObject.Xr, tempSceneObject.Yr, tempSceneObject.Zr),
                        new Vector3(tempSceneObject.Xs, tempSceneObject.Ys, tempSceneObject.Zs));
                }
            }
            catch
            {
                _networkInvoker.StartNetwork();
            }
        }

        /// <summary>
        /// Spawn zone levitation in scene
        /// </summary>
        private void SpawnLevitation(Vector3 position,Vector3 rotation, Vector3 scale)
        {
            GameObject tempZoneLevitation = Instantiate(_levitationZonePrefab, position, Quaternion.Euler(rotation));
            tempZoneLevitation.transform.localScale = scale;
        }

        private void SpawnPictures(SpacePictureObject[] pictures)
        {
            if (_spacePicturePrefab == null)
                return;
            
            if (pictures.Length == 0)
                return;
            
            PicturePanel[] picturePanels = new PicturePanel[pictures.Where(element=>element.PrefabObjectType == PrefabObjectType.Picture).ToArray().Length];
            
            for (int i = 0; i < pictures.Length; i++)
            {
                SpacePictureObject tempPictureObject = pictures[i];
                if (tempPictureObject.PrefabObjectType != PrefabObjectType.Picture)
                    continue;
                
                PicturePanel tempPicturePanel = Instantiate(_spacePicturePrefab);
                tempPicturePanel.PlaceNumber = tempPictureObject.PlaceNumber;
                tempPicturePanel.ToogleVisibility(false);
                
                tempPicturePanel.transform.position = DataExtensions.CalculateMiddlePosition(tempPictureObject);
                tempPicturePanel.transform.rotation = DataExtensions.CalculateRotation(tempPicturePanel.transform, tempPictureObject);
                tempPicturePanel.transform.localScale = DataExtensions.CalculateScale(tempPictureObject);
                
                picturePanels[i] = tempPicturePanel;
            }
            
            PicturePanelSpawned?.Invoke(picturePanels);
        }

        private void LoadRemoteEnviroment(SceneObject tempSceneObject)
        {
            _sceneFactory = new SceneFactory(new AddressableResourcesLocation(
                ResourcesLocation.GetRemoteSceneObjectCatalogPath(tempSceneObject.RerositoryUrl),
                () => OnRemoteSceneFactoryLoad(tempSceneObject.ObjectKey)));
        }

        private async void OnRemoteSceneFactoryLoad(string sceneKey)
        {
            DisableStartEnviroment();
            await _sceneFactory.LoadSceneAsync(sceneKey, LoadSceneMode.Additive).Task;
            await _sceneFactory.ActiveSceneAsync();
            _networkInvoker.StartNetwork();
        }

        private void DisableStartEnviroment()
        {
            if (_defaultEnviromentTheme.Count > 0)
            {
                for (int i = 0; i < _defaultEnviromentTheme.Count; i++)
                {
                    _defaultEnviromentTheme[i].SetActive(false);
                }
            }
        }

        private void OnObjectCreate(bool isCreate)
        {
            _spawnedObjectCount++;

            if (_spawnedObjectCount == _spawnedObjects.Count)
                _networkInvoker.StartNetwork();
        }

        private void GetRemoteCatalog()
        {
            if (string.IsNullOrEmpty(APIService.GetRemoteSceneObjectLocationEndPoint))
            {
                _networkInvoker.StartNetwork();
                return;
            }

            WebClient web = new WebClient();
            web.DownloadDataCompleted += OnDownloadComplete;

            string url =
                $"{APIService.GetRemoteSceneObjectLocationEndPoint}/{_spaceManager.CurrentTransitionTarget.Id.ToString().ToLower()}.json";

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri path))
            {
                web.DownloadDataAsync(path);
            }
            else
            {
                _networkInvoker.StartNetwork();
            }
        }

        private void OnDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _networkInvoker.StartNetwork();
                return;
            }

            try
            {
                var result = Encoding.ASCII.GetString(e.Result);
                //SpacePictureObject[] spacePictureObjects = JsonConvert.DeserializeObject<SpacePictureObject[]>(result);
                //SpawnPictures(spacePictureObjects);
                //TODO uncomment this 2 lines on product branch
                _sceneObjects = JsonConvert.DeserializeObject<List<SceneObject>>(result);
                CheckExistSceneObjects();
                
            }
            catch
            {
                _networkInvoker.StartNetwork();
            }
        }
    }
}