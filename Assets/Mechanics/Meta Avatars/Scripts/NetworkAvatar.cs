using UnityEngine;
using Oculus.Avatar2;
using Unity.Netcode;
using System.Collections;
using ELogLevel = Oculus.Avatar2.OvrAvatarLog.ELogLevel;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Vuplex.WebView;
using Assets.Scripts.Client;
using Assets.Scripts.Shared;
using Assets.Scripts.Space;
using Assets.Mechanics.Meta_Avatars.Scripts;
using Assets.Scripts.API;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public class NetworkAvatar : NetworkBehaviour
    {
        [SerializeField] private SampleAvatarEntity _localAvatarEntity;
        [SerializeField] private SampleAvatarEntity _defaultAssetLocalAvatarEntity;
        [SerializeField] private MetaAvatarNetworkBehaviour _networkAvatarBehaviour;
        [SerializeField] private AvatarSkinSelector _avatarSkinSelector;

        private bool _isCustomUserAvatar;
        private bool? _userAvatarIsAvailable;
        private bool _oculusUserLoggedIn;
        private bool _isCustomUserAvatarLoading;

        private IOculusAuthService _authService;
        private IClientData _clientData;
        private IApiService _apiService;

        public void InitAsOwner()
        {
            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _authService = commonDIInstaller.Resolve<IOculusAuthService>();
                _clientData = commonDIInstaller.Resolve<IClientData>();
                _apiService = commonDIInstaller.Resolve<IApiService>();

                _apiService.AuthSuccess -= OnAuthSuccess;
                _apiService.AuthSuccess += OnAuthSuccess;
            }
            InitAvatar();
        }

        public void SetVisibleToOthers(bool isVisible)
        {
            _networkAvatarBehaviour.SetVisibleToOthers(isVisible);
        }

        private void OnEnable()
        {
            AvatarSessionData.AvatarAssetIdChanged += LoadCurrentAvatarModel;
            _localAvatarEntity.OnDefaultAvatarLoadedEvent.AddListener(OnDefaultLoad);

            if (_apiService != null)
            {
                _apiService.AuthSuccess -= OnAuthSuccess;
                _apiService.AuthSuccess += OnAuthSuccess;
            }
        }

        private void OnDisable()
        {
            AvatarSessionData.AvatarAssetIdChanged -= LoadCurrentAvatarModel;
            _localAvatarEntity.OnDefaultAvatarLoadedEvent.RemoveListener(OnDefaultLoad);

            if (_apiService != null)
            {
                _apiService.AuthSuccess -= OnAuthSuccess;
            }
        }

        private void OnAuthSuccess()
        {
            LoadCurrentAvatarModel();
        }

        private void LoadCurrentAvatarModel()
        {
            if (!IsAvatarChangeAvailable())
            {
                return;
            }
            //_networkAvatarBehaviour.HideDefaultRemoteAvatar();
            StopAllCoroutines();

            //_localAvatarEntity.CreateDefaultAvatar();
            //_networkAvatarBehaviour.InitAsLocal(_localAvatarEntity, false,
            //    AvatarSessionData.DefaultAvatarAssetId, 0);

            StartCoroutine(AvatarLoadingRoutine());
        }

        private void OnDefaultLoad(OvrAvatarEntity avatarEntity)
        {
            _localAvatarEntity.TooggleVisibleDefaultAvatar(true);
        }

        private void InitAvatar()
        {
            //PlayerPrefs.DeleteAll();
            _isCustomUserAvatar = false;

            var localAvatarBodyTrackingBehavior = FindObjectOfType<OvrAvatarBodyTrackingBehavior>();
            var lipSyncBehaviour = FindObjectOfType<OvrAvatarLipSyncBehavior>();
            var faceTracking = FindObjectOfType<AvatarFaceTrackingDataHolder>();

            _localAvatarEntity.SetBodyTracking(localAvatarBodyTrackingBehavior);
            _localAvatarEntity.SetLipSync(lipSyncBehaviour);
            //_localAvatarEntity.SetBodyTracking(faceTracking.InputManager);
            _localAvatarEntity.SetFacePoseProvider(faceTracking.FacePoseBehavior);
            _localAvatarEntity.SetEyePoseProvider(faceTracking.EyePoseBehavior);

            _defaultAssetLocalAvatarEntity.SetBodyTracking(localAvatarBodyTrackingBehavior);
            _defaultAssetLocalAvatarEntity.SetLipSync(lipSyncBehaviour);
            //_defaultAssetLocalAvatarEntity.SetBodyTracking(faceTracking.InputManager);
            _defaultAssetLocalAvatarEntity.SetFacePoseProvider(faceTracking.FacePoseBehavior);
            _defaultAssetLocalAvatarEntity.SetEyePoseProvider(faceTracking.EyePoseBehavior);

            AvatarLODManager.Instance.firstPersonAvatarLod = _localAvatarEntity.AvatarLOD;

            if (!AvatarSessionData.AvatarAssetId.HasValue)
            {
                AvatarSessionData.SetAvatarAssetId(AvatarSessionData.DefaultAvatarAssetId);
            }
            else
            {
                LoadCurrentAvatarModel();
            }

            _localAvatarEntity.CreateDefaultAvatar();
            _networkAvatarBehaviour.InitAsLocal(_localAvatarEntity, false,
                AvatarSessionData.DefaultAvatarAssetId, 0);
        }

        private bool IsAvatarChangeAvailable()
        {
            return _clientData != null && !string.IsNullOrEmpty(_clientData.AccessToken) && !_clientData.IsGuest;
        }

        private IEnumerator LoadLocalAvatar()
        {
            _isCustomUserAvatar = false;

            while (_localAvatarEntity.IsPendingAvatar)
            {
                yield return null;
            }

            if (AvatarSessionData.AvatarAssetId == AvatarSessionData.DefaultAvatarAssetId)
            {
                _localAvatarEntity.CreateDefaultAvatar();
            }
            else
            {
                _localAvatarEntity.ReloadAvatarManually(AvatarSessionData.AvatarAssetId.Value.ToString(),
                    SampleAvatarEntity.AssetSource.StreamingAssets);
            }
        }

        private IEnumerator AvatarLoadingRoutine()
        {
            if (_userAvatarIsAvailable.HasValue && _userAvatarIsAvailable.Value)
            {
                _isCustomUserAvatarLoading = true;
                OnUserAvatarLoaded(_localAvatarEntity);
                yield break;
            }

            ulong userId = 0;

            Task<bool> isCdnAvailableTask = _authService.IsCdnAvailable().AsTask();
            yield return new WaitUntil(() => isCdnAvailableTask.IsCompleted);
            if (isCdnAvailableTask.Result)
            {
                Task<ulong> loginUserTask = _authService.LogInUser().AsTask();
                yield return new WaitUntil(() => loginUserTask.IsCompleted);

                if (loginUserTask.Result != 0)
                {
                    Task<bool> isUserHasAvatar = _authService.IsUserHasAvatar(loginUserTask.Result).AsTask();
                    yield return new WaitUntil(() => isUserHasAvatar.IsCompleted);

                    if (isUserHasAvatar.Result)
                    {
                        userId = loginUserTask.Result;
                   }
                }
            }

            AvatarSessionData.SetUserId(userId);

            Debug.Log($"---------AvatarSessionData.UserId = {AvatarSessionData.UserId}----------");

            _localAvatarEntity.OnLoadFailedEvent.RemoveListener(OnUserAvatarLoadFailed);
            _localAvatarEntity.OnLoadFailedEvent.AddListener(OnUserAvatarLoadFailed);
            _localAvatarEntity.OnUserAvatarLoadedEvent.RemoveListener(OnUserAvatarLoaded);
            _localAvatarEntity.OnUserAvatarLoadedEvent.AddListener(OnUserAvatarLoaded);

            if (AvatarSessionData.UserId.HasValue && AvatarSessionData.UserId.Value != 0)
            {
                Task<bool> checkUserAvatarTask = _authService.IsUserHasAvatar(AvatarSessionData.UserId.Value).AsTask();
                yield return new WaitUntil(() => checkUserAvatarTask.IsCompleted);
                _isCustomUserAvatarLoading = checkUserAvatarTask.Result;
            }
            else
            {
                _isCustomUserAvatarLoading = false;
            }

            if (_isCustomUserAvatarLoading)
            {
                _localAvatarEntity.LoadRemoteUserCdnAvatar(AvatarSessionData.UserId.Value);
            }
            else
            {
                //_networkAvatarBehaviour.HideDefaultRemoteAvatar();
                if (AvatarSessionData.AvatarAssetId.HasValue)
                {
                    if (AvatarSessionData.AvatarAssetId == AvatarSessionData.DefaultAvatarAssetId)
                    {
                        _localAvatarEntity.CreateDefaultAvatar();
                        OnUserAvatarLoaded(_localAvatarEntity);
                    }
                    else
                    {
                        _localAvatarEntity.ReloadAvatarManually(AvatarSessionData.AvatarAssetId.ToString(),
                            SampleAvatarEntity.AssetSource.StreamingAssets);
                    }
                }
            }
        }

        private void OnUserAvatarLoaded(OvrAvatarEntity avatarEntity)
        {
            if (_isCustomUserAvatarLoading)
            {
                _isCustomUserAvatar = true;
                _userAvatarIsAvailable = true;
            }

            _isCustomUserAvatarLoading = false;

            Debug.Log("User avatar loaded");
            _localAvatarEntity.OnUserAvatarLoadedEvent.RemoveListener(OnUserAvatarLoaded);

            _networkAvatarBehaviour.InitAsLocal(_localAvatarEntity, _isCustomUserAvatar,
                AvatarSessionData.AvatarAssetId.Value, AvatarSessionData.UserId.Value);
        }

        private void OnUserAvatarLoadFailed(OvrAvatarEntity avatarEntity,
            CAPI.ovrAvatar2LoadRequestInfo loadRequestInfo)
        {
            Debug.Log(loadRequestInfo.failedReason);
            _userAvatarIsAvailable = false;
            Debug.Log("User avatar not loaded");
            _localAvatarEntity.OnLoadFailedEvent.RemoveListener(OnUserAvatarLoadFailed);
            StartCoroutine(LoadLocalAvatar());
        }

        private void OvrAvatarLog_CustomLogger(ELogLevel level, string msg)
        {
            if (level < OvrAvatarLog.logLevel)
            {
                return;
            }

            if (level >= ELogLevel.Warn)
            {
                Debug.LogWarning(msg);
            }
            else if (level >= ELogLevel.Info)
            {
                Debug.Log(msg);
            }
            else if (level >= ELogLevel.Debug)
            {
                Debug.Log($"[Debug] {msg}");
            }
            else
            {
                Debug.Log($"[Verbose] {msg}");
            }
        }
    }
}