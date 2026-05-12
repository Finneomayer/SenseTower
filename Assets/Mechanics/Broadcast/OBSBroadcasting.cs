using Assets.Scripts.API;
using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Assets.Scripts.News;
using Assets.Scripts.Player;
using Assets.Scripts.Space;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Infrastructure.Factory;
using Newtonsoft.Json;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Player.WindowsMovement;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Broadcasting
{
    public class OBSBroadcasting : MonoBehaviour, IBroadcastingService
    {
        #region Const
        private const string CameraIdKey = "cameraid";
        private const string CameraSpaceTypeKey = "spacetype";
        private const string CameraSpaceIdKey = "spaceid";
        private const string CameraRotationKey = "rotation";
        private const string CameraPositionKey = "position";
        private const string BroadcastYoutubeLinkKey = "youtubeurl";
        private const string GetMetaDataEndPoint = "http://169.254.169.254/computeMetadata/v1/instance/attributes";
        private const string ObsProfilePath = "C:/Users/Administrator/AppData/Roaming/obs-studio/basic/profiles/YoutubeStreaming/service.json";
        private const string OBSApplicationExeFilePath = "C:/Program Files/obs-studio/bin/64bit";

        #endregion

        #region Inspector
        [SerializeField] private LoadingSceneChanger loadingSceneChanger;
        [SerializeField] private Camera _broadcastingCamera;
        [SerializeField] private Camera _avatarCamera;
        [SerializeField] private List<SampleAvatarEntity> _avatars;
        [SerializeField] private GameObject _playerName;
        [SerializeField] private List<GameObject> _showObjects;
        [SerializeField] private TMPro.TMP_Text _text;
        [SerializeField] private EditorMovementSystem _moveSystem;
        #endregion

        private static CameraData _cameraData;
        private ISpaceService _spaceService;
        private IApiService _apiService;
        private ISpaceManager _spaceManager;
        private static bool _broadcasting;
        private CameraService _cameraService;

        private void Start()
        {
            _cameraService = FindObjectOfType<CameraService>();
        }

        private void OnDisable()
        {
            bool localPlayerInvoke = true;
            if (TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                if (NetworkManager.Singleton != null && networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                    localPlayerInvoke = false;
            }

            if (!localPlayerInvoke) return;

            if (_cameraService == null) return;
            if (!_broadcasting) return;
            _cameraService.ClearCameraDataServerRpc();
            StopBroadcast();
        }

        [Inject]
        private void Construct(ISpaceService spaceService, IApiService apiService, ISpaceManager spaceManager)
        {
            _spaceService = spaceService;
            _apiService = apiService;
            _spaceManager = spaceManager;
        }

        public async UniTask<bool> CheckCapability()
        {
            var utcs = new UniTaskCompletionSource<bool>();

            //if (_cameraData == null)
            //{
            //    if (Enum.TryParse("HallScene", true, out SpaceType result))//InfrastructureScene
            //    {
            //        _cameraData = new CameraData()
            //        {
            //            SpaceId = "e81e71b9-e685-40c7-9d4e-49f9a328c1db",
            //            CameraId = "124",
            //            SpaceType = result,
            //            Position = "-6:1:3".StringToVector3(),
            //            Rotation = "0:90:0:1".StringToQuaternion(),
            //            YoutubeUrl = "live_410607875_62D7eaRSsJbiaXhgLXjuZnUZVztwDw"
            //        };
            //    }
            //    utcs.TrySetResult(_cameraData != null);
            //    return await utcs.Task;
            //}
            if (_cameraData!=null) 
            {
                utcs.TrySetResult(_cameraData != null);
                return await utcs.Task;
            }
            
            string resultCameraId = await GetMetadataFormVM(CameraIdKey);

            if (string.IsNullOrEmpty(resultCameraId))
            {
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            string resultSpaceType = await GetMetadataFormVM(CameraSpaceTypeKey);
            string resultSpaceId = await GetMetadataFormVM(CameraSpaceIdKey);

            string resultCameraRotation = await GetMetadataFormVM(CameraRotationKey);
            string resultCameraPosition = await GetMetadataFormVM(CameraPositionKey);
            string resultBroadcastYoutubeLink = await GetMetadataFormVM(BroadcastYoutubeLinkKey);
            
            if (!string.IsNullOrEmpty(resultCameraId) &&
                !string.IsNullOrEmpty(resultSpaceType) &&
                !string.IsNullOrEmpty(resultSpaceId) &&
                !string.IsNullOrEmpty(resultCameraRotation) &&
                !string.IsNullOrEmpty(resultCameraPosition) &&
                !string.IsNullOrEmpty(resultBroadcastYoutubeLink))
            {
                if (Enum.TryParse(resultSpaceType, true, out SpaceType result))
                {
                    _cameraData = new CameraData() {
                        SpaceId = resultSpaceId,
                        CameraId = resultCameraId, 
                        SpaceType = result,
                        Position = resultCameraPosition.StringToVector3(),
                        Rotation = resultCameraRotation.StringToQuaternion(),
                        YoutubeUrl = resultBroadcastYoutubeLink
                    };
                }
            }
            Debug.Log($"{_cameraData.Rotation}:::: {_cameraData.Position}");
            utcs.TrySetResult(_cameraData!= null);
            return await utcs.Task;
        }

        public void StartBroadcasting()
        {
            BroadcastAuth();
            SettingBroadcastConditions();
            LoadObsProfile(_cameraData.YoutubeUrl);
            StartOBS();
            StartCoroutine(DisableAfterTimer());
        }

        public async void BroadcastAuth()
        {
            if (_broadcasting) return;
            await UniTask.WaitUntil(() => (!string.IsNullOrEmpty(APIService.AuthUrl)));

            string login = "camera";
            string password = "Camera12345!";
            var form = new WWWForm();
            form.AddField("login", login);
            form.AddField("password", password);
            if (_apiService == null)
                return;
            var success = await _apiService.Auth(form);
            if (success)
            {
                LoadSpace();
            }
        }

        public void HideCameraAvatar() 
        {
            _showObjects.ForEach(element => element.SetActive(true));
            _moveSystem.enabled = false;
            _avatarCamera.gameObject.SetActive(false);
            HideCameraRemoteAvatar();
        }

        public void HideCameraRemoteAvatar() 
        {
            _avatars.ForEach(element => element.Hidden = true);
            _playerName.gameObject.SetActive(false);
        }

        private void StartOBS() 
        {
            if (_broadcasting) return;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
            _broadcasting = true;
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.Arguments = "--disable-shutdown-check --startstreaming";
            myProcess.StartInfo.WorkingDirectory = OBSApplicationExeFilePath;
            myProcess.StartInfo.FileName = OBSApplicationExeFilePath + "/obs64";
            myProcess.Start();
        }

        private void LoadObsProfile(string streamKey)
        {
            if (_broadcasting) return;
            if (!Directory.Exists(ObsProfilePath))
                return;
            string resultRead = File.ReadAllText(ObsProfilePath);
            ObsProfile profile = JsonConvert.DeserializeObject<ObsProfile>(resultRead);
            profile.settings.key = streamKey;

            File.WriteAllText(ObsProfilePath, JsonConvert.SerializeObject(profile));
        }

        private void SettingBroadcastConditions()
        {
            if (!_broadcasting) return;
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
            HideCameraAvatar();
            _broadcastingCamera.transform.SetParent(null);
            _broadcastingCamera.transform.SetPositionAndRotation(_cameraData.Position, _cameraData.Rotation);
        }

        private void LoadSpace() 
        {
            StartCoroutine(LoadBroadcastSpace());
        }

        private async UniTask<string> GetMetadataFormVM(string metadataKey)
        {
            string result = string.Empty;
            var utcs = new UniTaskCompletionSource<string>();

            RequestHelper options = new RequestHelper();
            options.Headers.Add("Metadata-Flavor", "Google");
            options.Uri = $"{GetMetaDataEndPoint}/{metadataKey}";

            var resultRequest = await WebRequestFunctions.Get(options);
            bool success = resultRequest.ResponseCode == HttpResponse<string>.SuccessCode;

            if (success)
            {
                result = resultRequest.ResponseData;
            }

            utcs.TrySetResult(result);

            return await utcs.Task;
        }
        private async void StopBroadcast()
        {
            await UniTask.WaitUntil(() => (!string.IsNullOrEmpty(APIService.GetBroadcastingServiceEndPoint)));

            RequestHelper options = new RequestHelper();
            options.Uri = $"{APIService.GetBroadcastingServiceEndPoint}/stop";

            var result = await WebRequestFunctions.Post(options);
        }

        private IEnumerator LoadBroadcastSpace() 
        {
            yield return new WaitUntil(()=> _spaceService.GetAllSpaces().Length>3);
            if (loadingSceneChanger != null)
            {
                loadingSceneChanger.LoadScene(_cameraData.SpaceType, _cameraData.SpaceId);
            }
            else 
            {
                _spaceManager.ChangeSpace(_cameraData.SpaceType, _cameraData.SpaceId);
            }
        }
        private IEnumerator DisableAfterTimer()
        {
            yield return new WaitForSeconds(10800);

            StopBroadcast();
            if (_cameraService != null) 
            {
                _cameraService.ClearCameraDataServerRpc();
            }

        }

        #region InnerClass
        public class ObsProfile 
        {
            public Settings settings;
            public string type;
        }

        public class Settings 
        {
            public bool bwtest;
            public string key;
            public string server;
            public string service;
        }

        #endregion
    }
}