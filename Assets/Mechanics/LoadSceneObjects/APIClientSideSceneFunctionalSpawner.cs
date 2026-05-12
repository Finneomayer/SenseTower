using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Environmental;
using Assets.Scripts.Gallery;
using Assets.Scripts.Space;
using Data;
using Infrastructure.Factory;
using JetBrains.Annotations;
using Mechanics.LoadSceneObjects.Interfaces;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.Network.Scripts.SpaceObjectsService;
using Mechanics.Network.Scripts.StaticObjectsService;
using Mechanics.VideoService;
using Mechanics.VideoService.Models;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Mechanics.LoadSceneObjects
{
    public class APIClientSideSceneFunctionalSpawner : MonoBehaviour, ISceneObjectSpawner
    {
        #region Inspector

        [SerializeField] private GameObject _levitationZonePrefab;
        [SerializeField] private PicturePanel _spacePicturePrefab;
        [SerializeField] private VideoInfrastructure _spaceVideoPrefab;
        [SerializeField] private GameObject _presentationPrefab;
        [SerializeField] private PadSwitcher _browserPadPrefab;
        [SerializeField] private NetworkFactory _networkFactory;
        #endregion
        

        public event Action<PicturePanel[]> PicturePanelSpawned;
        private List<VideoInfrastructure> _videos = new ();

        private List<Presentation> _presentations;
        private List<GameObject> _spawnedObjects = new();
        private GameObject _localBrowserPad;

        private Coroutine _findBrowser;
        
        private ISpaceManager _spaceManager;
        private IStaticObjectsService _staticObjectsService;
        private ISpaceObjectService _spaceObjectService;
        private DiContainer _diContainer;

        private SceneFactory _sceneFactory;
        
        [Inject]
        private void Construct(DiContainer diContainer, ISpaceObjectService spaceObjectService,
            IStaticObjectsService staticObjectsService, ISpaceManager spaceManager, IClientData clientData)
        {
            _diContainer = diContainer;
            _spaceManager = spaceManager;
            _staticObjectsService = staticObjectsService;
            _spaceObjectService = spaceObjectService;
#if !UNITY_SERVER
            if (_networkFactory == null)
                _networkFactory = FindObjectOfType<NetworkFactory>();
            if (!string.IsNullOrEmpty(clientData.AccessToken))
            {
                CheckExistSceneObjects();
            }
#endif
            _presentations = new List<Presentation>();
        }

        public async void CheckExistSceneObjects()
        {
            //List<StaticObject> sceneObjects = await _staticObjectsService.GetAllSceneStaticObjects(_spaceManager.CurrentTransitionTarget.Id.ToString());
            
            var space = await _spaceObjectService.GetSpaceWithAllObjects(_spaceManager.CurrentTransitionTarget.Id.ToString());
            var sceneObjects = new List<StaticObject>();
            var pictures = new List<PictureSpaceObject>();
            var videos = new List<VideoSpaceObject>();

            foreach (var spaceObject in space.Objects)
            {
                if (spaceObject.Value.GetType() == typeof(PictureSpaceObject))
                {
                    pictures.Add((PictureSpaceObject)spaceObject.Value);
                    continue;
                }
                if (spaceObject.Value.GetType() == typeof(VideoSpaceObject) || spaceObject.Value.TowerObject.ObjectClass.Name == "VideoObject")
                {
                    VideoSpaceObject videoObject = (VideoSpaceObject) spaceObject.Value;
                    videos.Add(videoObject);
                    continue;
                }
                sceneObjects.Add(DataExtensions.SpaceObjectToStaticObject(spaceObject.Value));
            }

            SpawnSceneObjects(sceneObjects, pictures,videos);
        }

        public void SpawnSceneObjects(List<StaticObject> objects, List<PictureSpaceObject> pictures, List<VideoSpaceObject> videos)
        {
            HideVizuale();
            ClearSceneObjects();
            //StaticObject[] pictures = objects
            //    .Where(element => element.PrefabObjectType == Enumenators.PrefabObjectType.Picture).ToArray();
            StaticObject[] levitationInfo = objects
                .Where(element => element.PrefabObjectType == Enumenators.PrefabObjectType.Levitation).ToArray();
            ClearBrowserPad();
            SpawnPictures(pictures.ToArray());
            SpawnVideos(videos.ToArray());
            SpawnLevitation(levitationInfo);
            if(objects.Exists(element=>element.PrefabObjectType == Enumenators.PrefabObjectType.BrowserPlace))
                SetClientBrowser();
        }
        
        public void HideVizuale()
        {
            foreach (VideoInfrastructure video in _videos)
            {
                video.Hide();
            }
            if (_presentations.Count <= 0) return;
            foreach (var presentation in _presentations)
            {
                presentation.HideAll();
            }
        }
        
        private void SpawnLevitation(StaticObject[] levitation)
        {
            foreach (var levitationInfo in levitation)
            {
                if (!levitationInfo.IsActive)
                    continue;
                Vector3 tempPosition = levitationInfo.Vectors.Position.VectorComponentsToVector3();
                Vector3 tempRotation = levitationInfo.Vectors.Rotation.VectorComponentsToVector3();
                Vector3 tempScale = levitationInfo.Vectors.Scale.VectorComponentsToVector3();

                GameObject tempZoneLevitation = Instantiate(_levitationZonePrefab);
                Vector3 scale = tempScale;
                scale.x = tempZoneLevitation.transform.localScale.x;
                tempZoneLevitation.transform.SetPositionAndRotation(tempPosition, Quaternion.Euler(tempRotation));
                tempZoneLevitation.transform.localScale = scale;

                _spawnedObjects.Add(tempZoneLevitation);
            }
        }

        private void SpawnPictures(PictureSpaceObject[] pictures)
        {
            if (_spacePicturePrefab == null)
                return;

            if (pictures.Length == 0)
                return;
            
            PicturePanel[] spawnedPictures = new PicturePanel[pictures.Length];

            for (int i = 0; i < pictures.Length; i++)
            {
                PictureSpaceObject tempPictureObject = pictures[i];
                
                if (!tempPictureObject.IsActive) continue;

                PicturePanel tempPicturePanel = Instantiate(_spacePicturePrefab);
                tempPicturePanel.PlaceNumber = tempPictureObject.PlaceNumber ?? 0;
                tempPicturePanel.transform.name += "num_" + tempPicturePanel.PlaceNumber;
                tempPicturePanel.ToogleVisibility(false);

                tempPicturePanel.transform.position = DataExtensions.CalculateMiddlePosition(DataExtensions.SpaceObjectToStaticObject(tempPictureObject));
                tempPicturePanel.transform.rotation = DataExtensions.CalculateRotation(tempPicturePanel.transform, DataExtensions.SpaceObjectToStaticObject(tempPictureObject));
                tempPicturePanel.transform.localScale = DataExtensions.CalculateScale(DataExtensions.SpaceObjectToStaticObject(tempPictureObject));

                if (tempPicturePanel.TryGetComponent<PicturePanelInfo>(out PicturePanelInfo info))
                {
                    var towerPicture = new TowerPicture()
                    {
                        Author = pictures[i].TowerObject.Picture.Author,
                        Name = pictures[i].TowerObject.Picture.Name,
                        Description = pictures[i].TowerObject.Picture.Description
                    };
                    info.SetDescription(towerPicture);
                }


                spawnedPictures[i] = tempPicturePanel;
                _spawnedObjects.Add(tempPicturePanel.gameObject);
            }

            spawnedPictures = RemoveNullElements(spawnedPictures);
            PicturePanelSpawned?.Invoke(spawnedPictures);
        }

        private void SpawnVideos(VideoSpaceObject[] videoSpaceObjects)
        {
            if (_spacePicturePrefab == null)
                return;

            if (videoSpaceObjects.Length == 0)
                return;

            for (int i = 0; i < videoSpaceObjects.Length; i++)
            {
                VideoInfrastructure tempPicturePanel = Instantiate(_spaceVideoPrefab);
                tempPicturePanel.Init(videoSpaceObjects[i]);
                _videos.Add(tempPicturePanel);
                _spawnedObjects.Add(tempPicturePanel.gameObject);
            }
        }

        private T[] RemoveNullElements<T>( T[] array)
        {
            array = Array.FindAll(array, x => x != null);
            return array;
        }
        
        private void SetBrowserPad(Browser browser, Presentation presentation, MainPresentationHolder holder)
        {
            //ClearBrowserPad();
            if (_localBrowserPad == null)
            {
                _localBrowserPad = _diContainer.InstantiatePrefab(
                    _browserPadPrefab,
                    browser.transform.position,
                    browser.transform.rotation, null);
            }
            
            if (_localBrowserPad.TryGetComponent(out PadSwitcher padSwitcher))
            {
                padSwitcher.AddBrowser(browser, presentation);
                holder.PadSwitcher = padSwitcher;
            }
        }

        private void ClearBrowserPad()
        {
            if (_localBrowserPad != null)
            { 
                Destroy(_localBrowserPad);
            }
            _networkFactory.DespawnPad();
        }

        private void SetClientBrowser()
        {
            if (_findBrowser != null) StopCoroutine(_findBrowser);
            _findBrowser = StartCoroutine(FindBrowser());
        }

        private void ClearSceneObjects()
        {
            if (_spawnedObjects.Count != 0)
            {
                for (int i = 0; i < _spawnedObjects.Count; i++)
                {
                    Destroy(_spawnedObjects[i]);
                }

                _spawnedObjects.Clear();
                _videos.Clear();
            }
        }
        private IEnumerator FindBrowser()
        {
            _presentations.Clear();

            List<Browser> sceneBrowsers = new List<Browser>();
            List<AdminPlace> sceneAdminPlaces = new List<AdminPlace>();

            while (sceneBrowsers.Count == 0)
            {
                Browser[] browsers = FindObjectsOfType<Browser>();
                foreach (var item in browsers)
                {
                    if (item.LogoTowerObjectId.Value != "")
                    {
                        sceneBrowsers.Add(item);
                    }
                }
                yield return new WaitForSeconds(1);
            }

            while (sceneAdminPlaces.Count == 0)
            {
                AdminPlace[] browsers = FindObjectsOfType<AdminPlace>();
                foreach (var item in browsers)
                {
                    if (item.LogoTowerObjectId.Value != "")
                    {
                        sceneAdminPlaces.Add(item);
                    }
                }
                yield return new WaitForSeconds(1);
            }

            foreach (var browser in sceneBrowsers)
            {
                foreach (var logo in sceneAdminPlaces)
                {
                    if (browser.LogoTowerObjectId.Value == logo.LogoTowerObjectId.Value)
                    {
                        var presentationObject = Instantiate(_presentationPrefab);
                        presentationObject.name = $"BrowserConstructor (Presentation) ID={browser.LogoTowerObjectId.Value}";
                        var presentation = presentationObject.GetComponent<Presentation>();
                        var mainPresentationHolder = presentationObject.GetComponent<MainPresentationHolder>();
                        _presentations.Add(presentation);

                        logo.ShowAdminPlace();
                        logo.SetInteractionObject(presentation.SenseLogo.gameObject);

                        if (browser == null) //need for refresh client browser after exit scene editing mode
                        //browser can be null when refreshing on server
                        {
                            SetClientBrowser(); //recursion
                        }
                        else
                        {
                            Vector3 tempPresentationPosition = new Vector3(browser.transform.position.x,
                                browser.transform.position.y,
                                browser.transform.position.z);
                            presentation.transform.position = tempPresentationPosition;
                            presentation.transform.rotation = browser.transform.rotation;
                            presentation.SenseLogo.transform.position = logo.transform.position;
                            presentation.SenseLogo.transform.rotation = logo.transform.rotation;

                            presentation.SetBrowser(browser);
                            presentation.SetAdminPlace(logo);

                            presentation.Init();
                            presentation.SetAdminPanelPosition();

                            Debug.LogWarning($"++BROWSER++ APIClientSideSceneFunctionalSpawner FindBrowser");

                            SetBrowserPad(browser, presentation, mainPresentationHolder);
                        }
                    }
                }
            }
        }
    }
}