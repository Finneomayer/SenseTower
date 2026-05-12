using System;
using Assets.Scripts.Environmental;
using Assets.Scripts.Player.WindowsMovement;
using Assets.Scripts.Space;
using Assets.Scripts.Zones;
using Cysharp.Threading.Tasks;
using Sense.Interectable.Teleportation;
using System.Collections;
using System.Linq;
using Assets.Mechanics.Advertisement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

public class ClientFillingScene : MonoBehaviour
{
    #region Inspector
    [SerializeField] private GameObject _exitDoorPrefab;
    [SerializeField] private Presentation _presentation;
    [SerializeField] private PadSwitcher _browserPadPrefab;
    [SerializeField] private XRSimpleInteractable _interactable;
    [SerializeField] private ZonesModel _zoneModel;
    [SerializeField] private SimpleTeleportAreasZoneController _areaZoneController;
    [SerializeField] private TextRotation _textRotationPrefab;
    [SerializeField] private GameObject _localTeleportation;
    [SerializeField] private AdvertisementBrowserPageService _advertisementPrefab;

    #endregion

    [SerializeField] private Material _remoteSceneSkyboxMaterial;
    private GameObject _teleportParent;
    private GameObject _exitDoor;
    private GameObject _landingPlaceClient;
    private GameObject _adminSpawnPoint;
    private DiContainer _diContainer;
    private TextRotation _textRotation;

    private void Awake()
    {
        _remoteSceneSkyboxMaterial = RenderSettings.skybox;
    }

    [Inject]
    public void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    void Start()
    {
#if !UNITY_SERVER
        Filling();
#endif
    }

    public async UniTask<Transform> GetAdminSpawnPoint()
    {
        return _adminSpawnPoint != null ? _adminSpawnPoint.transform : null;
    }

    private void Filling()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SpaceType.InfrastructureScene.ToString()));
        
        _adminSpawnPoint = GameObject.Find("AdminSpawnPosition");

        if (_remoteSceneSkyboxMaterial != null)
            RenderSettings.skybox = _remoteSceneSkyboxMaterial;

        ///////////////////////////////////////////////////
        // Sally TODO ���������
        // _teleportParent = GameObject.Find("TeleportationArea");
        //  _exitDoor = GameObject.Find("ExitDoor");
        var t = GameObject.FindObjectsOfType<GameObject>(true);
       
        foreach (GameObject gameObj in t)
        {
            if (gameObj.name == "TeleportationArea")
            {
                _teleportParent = gameObj;
                SetTeleportAreaPlayer();
            }

            if (gameObj.name == "ExitDoor")
            {
                _exitDoor = gameObj;
                SetExitDoor();
            }

            if (gameObj.name == "TeleportPointLocal")
            {
                SetLocalTeleports(gameObj);
            }

            if (gameObj.name == "AdvertisementPosition")
            {
                SetAdvertisement(gameObj);
            }
        }

        SetWinTeleportationInteractables();
        //////////////////////////////////////////
        SetClientChair();
    }

    private void SetAdvertisement(GameObject parent)
    {
        var adv = Instantiate(_advertisementPrefab, parent.transform);
    }

    private void SetLocalTeleports(GameObject remoteSceneTeleport)
    {
        var localTeleport = Instantiate(_localTeleportation);
        
        remoteSceneTeleport.transform.parent = localTeleport.transform;

        var targets = remoteSceneTeleport.GetComponentsInChildren<Transform>();
        targets = targets.Where(val => val != remoteSceneTeleport.transform).ToArray();

        var collider = remoteSceneTeleport.GetComponent<Collider>();

        var teleportScript = localTeleport.GetComponent<CustomTeleportationRandomAnchors>();
        teleportScript.SetLocalTeleports(targets, collider);
        teleportScript.enabled = false;
        teleportScript.enabled = true;


        var winClientTeleport = localTeleport.GetComponent<WinClientTeleport>();
        winClientTeleport.SetAnchors(targets);
    }

    private void SetWinTeleportationInteractables()
    {
        var teleportInteractables = FindObjectsOfType<CustomTeleportationInteractable>(true);
        foreach (var teleportInteractable in teleportInteractables)
        {
            if (teleportInteractable.TryGetComponent(out WinClientTeleport _))
            {
                continue;
            }

            var winTeleport = teleportInteractable.gameObject.AddComponent<WinClientTeleport>();
            var winTeleportAdapter = teleportInteractable.gameObject.AddComponent<WinTeleportationInteractableAdapter>();

            winTeleportAdapter.Init(winTeleport, teleportInteractable);
        }
    }

    private void SetTeleportAreaPlayer()
    {
        if (_teleportParent == null) return;
        for (int i = 0; i < _teleportParent.transform.childCount; i++)
        {
            GameObject teleportObject = _teleportParent.transform.GetChild(i).gameObject;
            teleportObject.layer = LayerMask.NameToLayer("Ground");
            var teleportArea = teleportObject.AddComponent<TeleportationArea>();
            teleportArea.interactionLayers = InteractionLayerMask.GetMask("Default", "Ground");
        }
        _areaZoneController.SetTeleportArea(_teleportParent.transform);
        _areaZoneController.Init();
    }

    private void SetExitDoor()
    {
        if (_exitDoor == null) return;

        //Instantiate(_exitDoorPrefab, _exitDoor.transform.position, _exitDoor.transform.rotation);

        if (_exitDoor.transform.parent != null && _exitDoor.transform.parent.name == "Envr")
        {
            //this condition is using for YachtScene to spawn exit door on moving yacht
            _diContainer.InstantiatePrefab(
                _exitDoorPrefab,
                _exitDoor.transform.position,
                _exitDoor.transform.rotation,
                _exitDoor.transform.parent);
        }
        else
        {
            //default exit door spawn on all scenes
            _diContainer.InstantiatePrefab(
                _exitDoorPrefab,
                _exitDoor.transform.position,
                _exitDoor.transform.rotation,
                null);
        }
    }

    private void SetClientChair()
    {
        StartCoroutine(FindChair());
    }

    //private void SetBrowserPad()
    //{
    //    var browserPadPoint = GameObject.Find("PadPosition");
    //    if (browserPadPoint != null)
    //    {
    //        _diContainer.InstantiatePrefab(
    //            _browserPadPrefab,
    //            browserPadPoint.transform.position,
    //            browserPadPoint.transform.rotation, null);
    //    }
    //}

    private IEnumerator FindChair()
    {
        yield return new WaitForSecondsRealtime(2);

        _landingPlaceClient = GameObject.Find("LandingPlace");

        if (_landingPlaceClient == null) yield break;

        yield return new WaitUntil(() => _zoneModel.transform.childCount != 0);
        //var _dynamicYacht = GameObject.Find("DynamicYacht");

        Place[] places =
        //     (_zoneModel.transform.childCount < _landingPlaceClient.transform.childCount && _dynamicYacht == null) ?

        //     _landingPlaceClient.GetComponentsInChildren<Place>() :
             _zoneModel.GetComponentsInChildren<Place>();

        foreach (var item in places)
        {
            item.Init(_zoneModel, null);
        }

    }
    //private void SetClientBrowser()
    //{
    //    var BrowserPlace = GameObject.Find("BrowserPlace");
    //    var BrowserAdminPlace = GameObject.Find("BrowserAdminPlace");
    //    _presentation.gameObject.SetActive(false);
    //    if (BrowserPlace == null || BrowserAdminPlace == null) return;
    //    _presentation.gameObject.SetActive(true);
    //    StartCoroutine(FindBrowser());
    //}

    //private IEnumerator FindBrowser() 
    //{
    //    // Finding main scene browser
    //    Browser browserInstance = null;

    //    while (browserInstance == null)
    //    {
    //        Browser[] browsers = FindObjectsOfType<Browser>();
    //        foreach (var item in browsers)
    //        {
    //            if (item.transform.parent == null)
    //            {
    //                browserInstance = item;
    //                break;
    //            }
    //        }
    //        yield return new WaitForSeconds(1);
    //    }

    //    // Finding main scene admin place
    //    AdminPlace adminPlaceInstance = null;

    //    while (adminPlaceInstance == null)
    //    {
    //        AdminPlace[] browsers = FindObjectsOfType<AdminPlace>();
    //        foreach (var item in browsers)
    //        {
    //            if (item.transform.parent == null)
    //            {
    //                adminPlaceInstance = item;
    //                break;
    //            }
    //        }
    //        yield return new WaitForSeconds(1);
    //    }

    //    XrInteractionAdminService adminService = FindObjectOfType<XrInteractionAdminService>();

    //    if (adminService != null)
    //        adminService.SetInteractableObject(_interactable.gameObject);

    //    Vector3 tempPresentationPosition = new Vector3(browserInstance.transform.position.x, browserInstance.transform.position.y + 0.15f, browserInstance.transform.position.z);
    //    _presentation.transform.position = tempPresentationPosition;
    //    _presentation.transform.rotation = browserInstance.transform.rotation;
    //    _interactable.transform.position = adminPlaceInstance.transform.position;
    //    _interactable.transform.rotation = adminPlaceInstance.transform.rotation;

    //    _presentation.SetBrowser(browserInstance);
    //    _presentation.SetAdminPlace(adminPlaceInstance);

    //    Debug.LogWarning($"++BROWSER++ ClientFillingScene FindBrowser");

    //    _presentation.Init();
    //    _presentation.SetAdminPanelPosition();

    //    SetBrowserPad();
    //}
}
