using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Assets.Blackboard;
using Assets.Scripts.Data;
using Assets.Scripts.Environmental;
using Cysharp.Threading.Tasks;
using Mechanics.LoadSceneObjects.Interfaces;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.LoadSceneObjects
{
    public class ServerSideObjectSpawner : NetworkBehaviour
    {
        private const float BrowserDefaultSize = 4.68365f;
        
        #region Inspector

        [SerializeField] private BlackBoard _blackBoardPrefab;
        [SerializeField] private List<StaticObject> _loadedModel = new();
        [SerializeField] private Browser _browserPrefab;
        [SerializeField] private AdminPlace _adminPlacePrefab;
        [SerializeField] private Presentation Presentation;
        [SerializeField] private XRSimpleInteractable _interactable;

        #endregion

        public bool Initialized => _initialized;

        WebClient web = new();
        private string _roomId = string.Empty;
        private string _remoteServerContentPath = String.Empty;

        private AdminPlace _adminPlaceInstance;
        private Browser _browserInstance;
        private Vector3 _browserAdminPlacePosition;
        private bool _initialized = true;
        private StaticObject _browserStaticObj;
        
        /// <summary>
        /// Disable spawn objects with information from files because work spawn objects with Database 
        /// </summary>
        
        //public override void OnNetworkSpawn()
        //{
        //    if (!IsServer) return;
           
        //    _roomId = DataExtensions.GetSpaceID();
        //    web.DownloadDataCompleted += OnDownloadComplete;

        //    InitializeEndPoints().Forget();
        //    // Uncomment this if need spawn object from json files in storage 
        //    //if(!string.IsNullOrEmpty(_roomId) && !string.IsNullOrEmpty(_remoteServerContentPath))
        //    //    CheckExistSceneObjects();
        //}
        
        //public override void OnNetworkDespawn()
        //{
        //    web.DownloadDataCompleted -= OnDownloadComplete;
        //}

        //public void CheckExistSceneObjects()
        //{
        //    string objectsPath = $"{_remoteServerContentPath}/{_roomId.ToLower()}.json";
        //    if (string.IsNullOrEmpty(objectsPath))
        //    {
        //        return;
        //    }

        //    if (Uri.TryCreate(objectsPath, UriKind.Absolute, out Uri path))
        //    {
        //        web.DownloadDataAsync(path);
        //    }
        //}

        //public void SpawnSceneObjects()
        //{
        //    foreach (StaticObject networkModel in _loadedModel)
        //    {
        //        if (networkModel.ObjectKey == "Blackboard")
        //        {
        //            var blackBoardInstance = Instantiate(_blackBoardPrefab);

        //            blackBoardInstance.transform.position = DataExtensions.CalculateMiddlePosition(networkModel);
        //            blackBoardInstance.transform.rotation =
        //                DataExtensions.CalculateRotation(blackBoardInstance.transform, networkModel);
        //            blackBoardInstance.BlackboardRenderTexture.transform.localScale = DataExtensions.CalculateBlackboardScale(networkModel);

        //        }

        //        if (networkModel.ObjectKey == "BrowserAdminPlace")
        //        {
        //            _browserAdminPlacePosition = DataExtensions.CalculateMiddlePosition(networkModel);
        //            _adminPlaceInstance = Instantiate(_adminPlacePrefab, _browserAdminPlacePosition, Quaternion.identity);
        //        }

        //        if (networkModel.ObjectKey == "BrowserPlace")
        //        {
        //            _browserInstance = Instantiate(_browserPrefab);
        //            _browserStaticObj = networkModel;
        //            _browserInstance.transform.position = DataExtensions.CalculateMiddlePosition(networkModel) - Vector3.up * 1.39f;
        //            _browserInstance.transform.rotation = DataExtensions.CalculateRotation(_browserInstance.transform, networkModel) * Quaternion.Euler(0, 90, 0);
        //        }
        //    }

        //    SetBrowserOnScene();
        //}

        //public void TryInstantiateSceneBrowser(GameObject browserPlace, GameObject browserAdminPlace)
        //{
        //    if (browserPlace == null || browserAdminPlace == null)
        //    {
        //        return;
        //    }

        //    if (_adminPlaceInstance != null && _browserInstance != null)
        //    {
        //        return;
        //    }

        //    _adminPlaceInstance = Instantiate(_adminPlacePrefab, browserAdminPlace.transform.position, browserAdminPlace.transform.rotation);
        //    _browserInstance = Instantiate(_browserPrefab, browserPlace.transform.position, browserPlace.transform.rotation);
        //    SetBrowserOnScene();
        //}

        //private float CalculateBrowserScale(StaticObject scalableObject)
        //{
        //    float newSize = Vector3.Distance(
        //        new Vector3(scalableObject.LeftTopX, scalableObject.LeftTopY, scalableObject.LeftTopZ),
        //        new Vector3(scalableObject.RightDownX, scalableObject.RightDownY, scalableObject.RightDownZ));

        //    return newSize / BrowserDefaultSize;
        //}

        private async UniTask InitializeEndPoints()
        {
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.RemoteServerContentUrl));
            _remoteServerContentPath = ServerApiService.RemoteServerContentUrl;
        }

        //private void OnDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        //{
        //    if (e.Error != null)
        //    {
        //        Debug.Log(e.Error.Message);
        //        _initialized = true;
        //        return;
        //    }

        //    try
        //    {
        //        var result = Encoding.ASCII.GetString(e.Result);
        //        _loadedModel = JsonConvert.DeserializeObject<List<StaticObject>>(result);
        //        SpawnSceneObjects();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Log(ex.Message);
        //    }
        //    _initialized = true;
        //}

        //private void SetBrowserOnScene()
        //{
        //    if (_adminPlaceInstance == null && _browserInstance == null)
        //    {
        //        return;
        //    }

        //    Debug.Log($"++BROWSER++ ServerSideObjectSpawner SetBrowserOnScene");

        //    XrInteractionAdminService adminService = FindObjectOfType<XrInteractionAdminService>();
        //    if (adminService != null)
        //        adminService.SetInteractableObject(_interactable.gameObject);

        //    Presentation.SetAdminPlace(_adminPlaceInstance);
        //    _adminPlaceInstance.NetworkObject.Spawn();

        //    Presentation.SetBrowser(_browserInstance);
            
        //    _browserInstance.NetworkObject.Spawn();
                
        //    _browserInstance.BrowserScale.Value = DataExtensions.CalculateBrowserScale(_browserStaticObj);
        //    _browserInstance.BrowserAdminPlace.Value = _browserAdminPlacePosition;

        //    Presentation.Init();
        //}
    }
}