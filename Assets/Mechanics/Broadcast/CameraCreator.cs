using Assets.Scripts.API;
using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Assets.Scripts.Space;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Infrastructure.Factory;
using Newtonsoft.Json;
using Proyecto26;
using System;
using System.Collections;
using Assets.Localization;
using Assets.Scripts.Client;
using Broadcasting;
using Models;using Mono.CSharp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

[Serializable]
public enum CameraState
{
    Off = 1,
    Starting = 2,
    Streaming = 3,
    Fail = 4,
    Stopping = 5
}

[Serializable]
public class CameraResponseCanStart
{
    public bool canStart;
    public string canNotStartReason;
}

[Serializable]
public class CameraResponseStartReaction
{
    public string id;
    public string name;
    public int state;
    public bool canStart;
}

[Serializable]
public class CameraSendData
{
    public Guid spaceId;
    public CameraVector position;
    public CameraVector rotation;
}

[Serializable]
public class CameraVector
{
    public float x;
    public float y;
    public float z;
}

public class CameraCreator : MonoBehaviour
{
    #region Inspector

    [SerializeField] private Canvas _buttonMenuCanvas;
    [SerializeField] private Collider _cameraCollider;
    [SerializeField] private RecordingCameraGrabbable _cameraGrab;
    [SerializeField] private Canvas _statusCanvas;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private SpriteRenderer _circleSprite;
    [SerializeField] private Camera _previewCamera;
    [SerializeField] private Canvas _previewCameraCanvas;
    [SerializeField] private Button _cameraPlayButton;
    [SerializeField] private Button _cameraStopButton;
    [SerializeField] private LocalizationVariant _statusOff;
    [SerializeField] private LocalizationVariant _statusStarting;
    [SerializeField] private LocalizationVariant _statusStreaming;
    [SerializeField] private LocalizationVariant _statusError;
    [SerializeField] private LocalizationVariant _statusStopping;

    private ISpaceManager _spaceManager;
    private IClientData _clientData;
    public string CameraId;
    private Coroutine _checkStateCoroutine;
    public CameraService _cameraService
    {
        private set;
        get;
    }

    #endregion

    private Coroutine _recVisualizeCoroutine;
    [Inject]
    private void Construct(ISpaceManager spaceManager, IClientData clientData)
    {
        _spaceManager = spaceManager;
        _clientData = clientData;
    }

    private void OnEnable()
    {
        StartState();
        //_statusCanvas.gameObject.SetActive(false);
        _cameraPlayButton.onClick.AddListener(()=> InitAndStartBroadCast());
        _cameraStopButton.onClick.AddListener(()=> StopBroadcast());
        _checkStateCoroutine = StartCoroutine(CheckState());
    }

    private void OnDisable()
    {
        _cameraPlayButton.onClick.RemoveAllListeners();
        _cameraStopButton.onClick.RemoveAllListeners();
        if (_checkStateCoroutine != null) StopCoroutine(_checkStateCoroutine);
    }

    public void IsOwner(bool isOwner)
    {
        _cameraCollider.enabled = isOwner;
        if (!isOwner)
        {
            Destroy(_buttonMenuCanvas.gameObject);
            //Destroy(_previewCameraCanvas.gameObject);
            //Destroy(_statusCanvas.gameObject);
        }
    }

    public GameObject GetCameraGrab()
    {
        return _cameraGrab.gameObject;
    }

    public void RecState()
    {
        _cameraCollider.enabled = false;
        InitialializeState();
        RecVisualize();
    }

    public void SetCameraService(CameraService cameraService) 
    {
        _cameraService = cameraService;
    }

    public void BeforeRecState()
    {
        InitialializeState();
        _recVisualizeCoroutine = StartCoroutine(ShowRecSprite());
    }

    public Camera GetPreviewCamera()
    {
        return _previewCamera;
    }

    private async void InitAndStartBroadCast() 
    {
        _cameraPlayButton.interactable = false;
        _cameraGrab.enabled = false;
        _cameraCollider.enabled = false;
        _cameraService.SaveRecStateOnServer(true);

        await PrepareStreaming();
        await StartBroadcast();
    }

    private async UniTask StartBroadcast() 
    {
        await UniTask.WaitUntil(() => (!string.IsNullOrEmpty(APIService.GetBroadcastingServiceEndPoint)));

        RequestHelper options = new RequestHelper();
        options.Uri = $"{APIService.GetBroadcastingServiceEndPoint}/{CameraId}/start";
        options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
        var body = new CameraSendData()
        {
            spaceId = _spaceManager.CurrentTransitionTarget.Id,
            position = new CameraVector()
            {
                x = _cameraGrab.gameObject.transform.position.x,
                y = _cameraGrab.gameObject.transform.position.y,
                z = _cameraGrab.gameObject.transform.position.z
            },
            rotation = new CameraVector()
            {
                x = _cameraGrab.gameObject.transform.eulerAngles.x,
                y = _cameraGrab.gameObject.transform.eulerAngles.y,
                z = _cameraGrab.gameObject.transform.eulerAngles.z
            }
        };

        options.BodyString = JsonConvert.SerializeObject(body);
        Debug.LogWarning(options.BodyString);

        var result = await WebRequestFunctions.Post(options);

        if (result.ResponseData != null)
        {
            var data = JsonConvert.DeserializeObject<ScResult<CameraResponseStartReaction>>(result.ResponseData);
            Debug.LogWarning(result.ResponseData);
        }

        if (result.ResponseCode != HttpResponse<EmptyResponseData>.SuccessCode)
        {
            _cameraService.SaveRecStateOnServer(false);
            StartState();
        }
    }
    
    public void StartState() 
    {
        if (_recVisualizeCoroutine != null)
        {
            StopCoroutine(_recVisualizeCoroutine);
            _recVisualizeCoroutine = null;
        }

        if(_cameraGrab != null)
            _cameraGrab.enabled = true;
        if (_circleSprite != null)
            _circleSprite.color = Color.green;
        if (_previewCamera != null)
            _previewCamera.gameObject.SetActive(true);
        _previewCameraCanvas.gameObject.SetActive(true);
        _cameraCollider.enabled = true;
        //_statusCanvas.gameObject.SetActive(false);
        _cameraPlayButton.interactable = true;
    }

    private void InitialializeState()
    {
        _cameraCollider.enabled = false;
        if(_statusCanvas != null)
            _statusCanvas.gameObject.SetActive(true);
        _previewCamera.gameObject.SetActive(false);
        if(_statusCanvas != null)
            _previewCameraCanvas.gameObject.SetActive(false);
        if(_cameraPlayButton != null)
            _cameraPlayButton.interactable = false;
    }

    private void RecVisualize()
    {
        //if(_statusCanvas != null) _statusCanvas.gameObject.SetActive(false);
        _circleSprite.color = Color.red;
    }

    private async UniTask PrepareStreaming()
    {
        await UniTask.WaitUntil(() => (!string.IsNullOrEmpty(APIService.GetBroadcastingServiceEndPoint)));

        RequestHelper options = new RequestHelper();
        options.Uri = $"{APIService.GetBroadcastingServiceEndPoint}/{CameraId}/canStart";
        Debug.LogWarning($"options.Uri {options.Uri}");

        options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

        var result = await WebRequestFunctions.Get(options);
        Debug.Log(result.ResponseCode);
        if (result.ResponseData != null)
        {
            var data = JsonConvert.DeserializeObject<ScResult<CameraResponseCanStart>>(result.ResponseData);

            if (data != null && data.Result != null)
            {
                if (data.Result.canStart)
                {
                    Debug.LogError("Can start");
                    StartState();
                }
                else Debug.LogError(data.Result.canNotStartReason);
            }
        }


        //options.Uri = $"{APIService.GetBroadcastingServiceEndPoint}/{CameraParameters.CurrentId}/canStart";
        //string position = _cameraGrab.gameObject.transform.position.Vector3ToCustomString();
        //string rotation = _cameraGrab.gameObject.transform.rotation.QuaternionToCustomString();

        //options.Params["cameraId"] = Guid.NewGuid().ToString();
        //options.Params["spaceType"] = _spaceManager.CurrentTransitionTarget.SpaceType.ToString();
        //options.Params["youtubeUrl"] = APIService.GetBroadcastingKey;
        //options.Params["position"] = position;
        //options.Params["rotation"] = rotation;
        //options.Params["spaceId"] = _spaceManager.CurrentTransitionTarget.Id.ToString();
        //_cameraPlayButton.interactable = false;

        //var result = await WebRequestFunctions.Post(options);
        //if (result.ResponseCode != HttpResponse<EmptyResponseData>.SuccessCode)
        //{
        //    StartState();
        //}
    }

    private async void StopBroadcast()
    {
        await _cameraService.StopCameraBroadcast(CameraId);
        StartState();
    }

    private IEnumerator ShowRecSprite() 
    {
        yield return new WaitForSeconds(200);
        RecVisualize();
        _recVisualizeCoroutine = null;
    }

    private IEnumerator CheckState()
    {
        while (this.isActiveAndEnabled)
        {
            if (String.IsNullOrEmpty(CameraId)) yield return null;

            UniTask.WaitUntil(() => (!string.IsNullOrEmpty(APIService.GetBroadcastingServiceEndPoint)));

            RequestHelper options = new RequestHelper();
            options.Uri = $"{APIService.GetBroadcastingServiceEndPoint}/{CameraId}/state";

            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            var task = WebRequestFunctions.Get(options);
            yield return new WaitUntil(() => task.Status == UniTaskStatus.Succeeded);

            var response = task.GetAwaiter().GetResult();

            if (response.ResponseData != null)
            {
                var data = JsonConvert.DeserializeObject<ScResult<CameraState>>(response.ResponseData);

                if (data != null)
                {
                    Debug.Log($"State {data.Result}");
                    string status = "";

                    switch (data.Result)
                    {
                        case CameraState.Off:
                            status = _statusOff.Localize();
                            _cameraStopButton.interactable = false;
                            _circleSprite.color = Color.green;
                            break;
                        case CameraState.Starting:
                            status = _statusStarting.Localize();
                            _cameraStopButton.interactable = false;
                            _circleSprite.color = Color.yellow;
                            break;
                        case CameraState.Streaming:
                            status = _statusStreaming.Localize();
                            _cameraStopButton.interactable = true;
                            _circleSprite.color = Color.red;
                            break;
                        case CameraState.Fail:
                            status = _statusError.Localize();
                            _cameraStopButton.interactable = false;
                            _circleSprite.color = Color.black;
                            break;
                        case CameraState.Stopping:
                            status = _statusStopping.Localize();
                            _cameraStopButton.interactable = false;
                            _circleSprite.color = Color.yellow;
                            break;
                    }

                    _statusText.text = status;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
