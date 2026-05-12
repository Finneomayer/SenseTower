using Assets.Scripts.Space;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Client;
using UnityEngine;
using Zenject;
using TMPro;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;
using Assets.Localization;
using ModestTree;
using Client;
using Unity.Netcode;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Server;
using Assets.Scripts.Shared;
using System.Threading.Tasks;

//[RequireComponent(typeof(SceneMetrica))]
public class SceneChangerView : MonoBehaviour
{
    [Header("Fader")]
    [SerializeField] private float _timeForFade;
    [SerializeField] private float _steps;
    [SerializeField] private TMP_Text _loadingText;
    private OnPlayerUI _onPlayerUi;
    [Space]

    [Header("Camera")]
    [SerializeField] private Camera _temporaryCamera;
    [SerializeField] private GameObject _temporaryObjects;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Animation _loadingAnimation;

    [Header("Parameters")]
    [SerializeField] private bool _isNetworkScene;
    [SerializeField] private float _denyTime;

    [Header("Spawn")]
    [SerializeField] private ClientFillingScene _remoteSceneClientFilling;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    [Header("Localization")]
    [SerializeField] private LocalizationVariant _defaultSceneErrorLocalizationVariant;
    [SerializeField] private LocalizationVariant _defaultNetworkErrorLocalizationVariant;
    [SerializeField] private LocalizationVariant _returningMessageLocalizationVariant;
    [SerializeField] private LocalizationVariant _privateSpaceKickMessageLocalizationVariant;
    [SerializeField] private LocalizationVariant _ownerAbsenceKickMessageLocalizationVariant;
    [SerializeField] private LocalizationVariant _pleaseWait;

    public event Action PlayerInited;

    private NetworkPlayer _networkPlayer;
    private float _timer;
    private bool _networkPlayerInited;
    private ISpaceManager _spaceManager = new SpaceManager();
    private bool _isSpaceChangingInProgress;

    private Coroutine _currentBlackScreenFadingCoroutine;
    private IClientData _clientData;

    [Inject]
    public void Init(IClientData clientData,ISpaceManager spaceManager)
    {
        _clientData = clientData;
        _spaceManager = spaceManager;
    }

    public void Init(OnPlayerUI onPlayerUi)
    {
        _onPlayerUi = onPlayerUi;
        _loadingText.enabled = true;
        _loadingText.text = _pleaseWait.Localize().Replace("{0}","");
        _loadingPanel.SetActive(true);
        _loadingAnimation.Play("Slider_rightpart");
    }

    public bool IsNetworkScene()
    {
        return _isNetworkScene;
    }

    //public void SwitchBlackCamera(bool flag)
    //{
    //    _temporaryCamera.gameObject.SetActive(flag);
    //}

    public TMP_Text GetLoadingText()
    {
        return _loadingText;
    }

    public void ChangeSceneWithErrorText(SpaceType type, bool deleteData, string visibleText, string id = null, int messageShowDelay = 5)
    {
        if (_isSpaceChangingInProgress)
        {
            return;
        }

        if (deleteData)
        {
            _clientData.DeleteAllData();
        }
        
        _loadingText.enabled = true; 
        _loadingAnimation.Play("Slider_leftpart");
        _loadingText.text = string.IsNullOrEmpty(visibleText)? _defaultSceneErrorLocalizationVariant.Localize() : visibleText;
        StartBlackScreenFadingCoroutine(ChangeSceneProcess(type, id, messageShowDelay));
    }

    public void ChangeSpace(SpaceType type, string id = null, string sceneName = "")
    {
        if (_isSpaceChangingInProgress)
        {
            return;
        }

        Debug.Log($"change {type} {id} {sceneName}");
        StartBlackScreenFadingCoroutine(ChangeSceneProcess(type, id,0, sceneName:sceneName));
    }

    public void ReloadCurrentSpace(bool keepPlayerPosition)
    {
        if (_isSpaceChangingInProgress)
        {
            return;
        }

        if (_spaceManager == null || _spaceManager.CurrentTransitionTarget == null)
        {
            return;
        }

        if (_isNetworkScene)
        {
            //_metrica.ExitScene();
        }

        _isSpaceChangingInProgress = true;

        if (keepPlayerPosition && _networkPlayer != null)
        {
            ClientDataInSpace clientDataInSpace = new();
            clientDataInSpace.LastSpacePosition = _networkPlayer.transform.position;
            clientDataInSpace.LastSpaceRotation = _networkPlayer.transform.rotation.eulerAngles;
            clientDataInSpace.LastSpaceName = _spaceManager.CurrentTransitionTarget.SceneName;
            clientDataInSpace.LastSpaceID = _spaceManager.CurrentTransitionTarget.Id.ToString();

            PlaceReconnector placeReconnector = FindObjectOfType<PlaceReconnector>();
            if (placeReconnector != null)
            {
                ulong? occupiedPlaceNetworkObjectId = placeReconnector.GetOccupiedPlaceNetworkObjectId();
                if (occupiedPlaceNetworkObjectId != null)
                {
                    clientDataInSpace.OccupiedPlaceNetworkObjectId = occupiedPlaceNetworkObjectId.ToString();
                }
            }
        }

        _spaceManager.ChangeSpace(_spaceManager.CurrentTransitionTarget.SpaceType,
            _spaceManager.CurrentTransitionTarget.Id.ToString(), reload: true);
    }

    private IEnumerator ChangeSceneProcess(SpaceType type, string id, int additionalWaiting, bool reload = false, string sceneName ="")
    {
        _isSpaceChangingInProgress = true;

        if (_isNetworkScene)
        {
            //_metrica.ExitScene();

            UniTask fadingTask = _onPlayerUi.FadeToBlackDefault();
            yield return new WaitUntil(() => fadingTask.Status == UniTaskStatus.Succeeded);

            _temporaryCamera.gameObject.SetActive(true);
            _temporaryObjects.SetActive(true);

            if (additionalWaiting > 0)
                yield return new WaitForSeconds(additionalWaiting);

            _spaceManager.ChangeSpace(type, id, reload);
        }
        else
        {
            UniTask fadingTask = _onPlayerUi.FadeToBlackDefault();
            yield return new WaitUntil(() => fadingTask.Status == UniTaskStatus.Succeeded);

            _temporaryCamera.gameObject.SetActive(true);
            _temporaryObjects.SetActive(true);

            _spaceManager.ChangeSpace(type, id, reload);
        }

        _loadingText.enabled = true;
        _loadingText.text = _pleaseWait.Localize().Replace("{0}",sceneName);
        _loadingPanel.SetActive(true);
        _loadingAnimation.Play("Slider_leftpart");

        _currentBlackScreenFadingCoroutine = null;
    }

    private void ChangeSceneImmediate(SpaceType type, string id, string message, int additionalWaiting = 4)
    {
        if (_isSpaceChangingInProgress)
        {
            return;
        }
        StartCoroutine(ChangeSceneImmediateRoutine(type, id, message, additionalWaiting));
    }

    private IEnumerator ChangeSceneImmediateRoutine(SpaceType type, string id, string message, int additionalWaiting)
    {
        _isSpaceChangingInProgress = true;
        _onPlayerUi.LoadingAnimation.enabled = false;
        _loadingText.enabled = true;
        _loadingText.text = message;
        _loadingAnimation.Play("Slider_leftpart");
        yield return new WaitForSeconds(additionalWaiting);
        _spaceManager.ChangeSpace(type, id);
    }

    private IEnumerator OpenSceneProcess()
    {
        _loadingText.enabled = true;
        _loadingText.text = _pleaseWait.Localize();

        Task fadingTask = _onPlayerUi.FadeToTransparent().AsTask();
        yield return new WaitUntil(() => fadingTask.IsCompleted);

        _loadingText.text = "";
        _currentBlackScreenFadingCoroutine = null;

        //if (_isNetworkScene)_metrica.EnterScene();
    }

    private void Awake()
    {
        if (_isNetworkScene)
        {
            //_metrica = GetComponent<SceneMetrica>();
        }
    }

    private void Start()
    {
        if (!_isNetworkScene) return;

        _onPlayerUi.BlackMask.color = new Color(0, 0, 0, 1);
        Debug.Log(_onPlayerUi.BlackMask.color);
        _onPlayerUi.LoadingAnimation.enabled = true;
    }

    private void FixedUpdate()
    {
#if !UNITY_SERVER
        _timer += Time.fixedDeltaTime;
        if (_isNetworkScene && !_networkPlayerInited && _timer > _denyTime
            && !_isSpaceChangingInProgress)
        {
            ProcessReturningToEnterScene();
        };
#endif
    }

    private void ProcessReturningToEnterScene()
    {
        string messageText = $"{_defaultNetworkErrorLocalizationVariant.Localize()}\n{_returningMessageLocalizationVariant.Localize()}";
        ReturnToEnterScene(messageText);
    }

    private void ReturnToEnterScene(string textValue)
    {
        ChangeSceneImmediate(SpaceType.EnterScene, null, textValue);
        //_isSpaceChangingInProgress = true;
        //_onPlayerUi.LoadingAnimation.enabled = false;
        //_loadingText.enabled = true;
        //_loadingText.text = textValue;
        //yield return new WaitForSeconds(4);
        //_spaceManager.ChangeSpace(SpaceType.EnterScene);
    }

    public async UniTask InitScene(NetworkPlayer networkPlayer)
    {
#if !UNITY_SERVER
        await InitAsync(networkPlayer);
        PlayerInited?.Invoke();
#endif
    }

    private async UniTask InitAsync(NetworkPlayer networkPlayer)
    {
        Transform adminSpawnPoint = null;

        _networkPlayer = networkPlayer;

        _networkPlayer.InitAsOwner();

        _networkPlayerInited = true;
        
        //if (_remoteSceneClientFilling != null
        //    && _clientData.UserName == "kavva")
        //{
        //    adminSpawnPoint = await _remoteSceneClientFilling.GetAdminSpawnPoint();
        //}

        if (!await _networkPlayer.SetCachedPosition())
        {
            if (adminSpawnPoint != null)
            {
                _networkPlayer.transform.SetPositionAndRotation(adminSpawnPoint.position, adminSpawnPoint.rotation);
            }
            else
            {
                await _networkPlayer.SetLastDoorPosition(_spawnPoints);
            }
        }

        _temporaryCamera.gameObject.SetActive(false);
        _temporaryObjects.gameObject.SetActive(false);
        _onPlayerUi.LoadingAnimation.enabled = false;
        _networkPlayer.LostConnection += CloseSceneLostConnection;
        StartBlackScreenFadingCoroutine(OpenSceneProcess());
    }

    private void CloseSceneLostConnection()
    {
        if (!_isSpaceChangingInProgress)
        {
            _onPlayerUi.transform.SetParent(_temporaryCamera.transform);
            _temporaryCamera.gameObject.SetActive(true);
            _temporaryObjects.SetActive(true);

            if (string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason))
            {
                ProcessReturningToEnterScene();
                return;
            }

            var disconnectData = UserDisconnectData.Deserialize(NetworkManager.Singleton.DisconnectReason);

            if (TryGetUserKickReason(disconnectData.DisconnectMessage, out UserKickReason kickReason))
            {
                disconnectData.DisconnectMessage = GetKickReasonMessage(kickReason);
            }
            KickUser(disconnectData);
        }
    }

    private bool TryGetUserKickReason(string disconnectReason, out UserKickReason reason)
    {
        reason = UserKickReason.OwnerIsNotInSpace;
        if (disconnectReason == null)
        {
            return false;
        }

        foreach (UserKickReason kickReason in Enum.GetValues(typeof(UserKickReason)))
        {
            if (disconnectReason == kickReason.ToString())
            {
                reason = kickReason;
                return true;
            }
        }
        return false;
    }

    private string GetKickReasonMessage(UserKickReason userKickReason)
    {
        string errorMessage = "";
        switch (userKickReason)
        {
            case UserKickReason.PrivateSpace:
                errorMessage = _privateSpaceKickMessageLocalizationVariant.Localize();
                break;
            case UserKickReason.OwnerIsNotInSpace:
                errorMessage = _ownerAbsenceKickMessageLocalizationVariant.Localize();
                break;
            default:
                break;
        }
        return errorMessage;
    }

    private void KickUser(UserDisconnectData disconnectData)
    {
        if (disconnectData == null)
        {
            ChangeSceneImmediate(SpaceType.EnterScene, null, _returningMessageLocalizationVariant.Localize());
            return;
        }

        string errorMessage = $"{disconnectData.DisconnectMessage}\n{_returningMessageLocalizationVariant.Localize()}";

        SpaceType destinationSpace;
        if (disconnectData.CanBeInSenseTower)
        {
            destinationSpace = SpaceType.HallScene;
        }
        else
        {
            destinationSpace = SpaceType.EnterScene;
            _clientData.DeleteAllData();
        }

        ChangeSceneImmediate(destinationSpace, null, errorMessage);
    }

    private void StartBlackScreenFadingCoroutine(IEnumerator fadingRoutine)
    {
        StopBlackScreenFadingCoroutine();
        _currentBlackScreenFadingCoroutine = StartCoroutine(fadingRoutine);
    }

    private void StopBlackScreenFadingCoroutine()
    {
        if (_currentBlackScreenFadingCoroutine != null)
        {
            StopCoroutine(_currentBlackScreenFadingCoroutine);
            _currentBlackScreenFadingCoroutine = null;
        }
    }
}
