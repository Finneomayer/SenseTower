using System;
using System.Collections;
using Assets.Localization;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Client
{
    public class ClientVerification : MonoBehaviour
    {
        [SerializeField]private SceneChangerView _sceneChanger;
        [SerializeField]private ServerVerification _serverVerification;

        [SerializeField]private LocalizationVariant _notAuthorizedMessageLocalizationVariant;
        [SerializeField]private LocalizationVariant _returningMessageLocalizationVariant;

        private IApiService _apiService;
        private IClientData _clientData;
        
        private DateTime expTime;
        private ISpaceService _spaceService;

        [Inject]
        public void Construct(IClientData clientData, IApiService service, ISpaceService spaceService)
        {
            _clientData = clientData;
            _spaceService = spaceService;
            _apiService = service;
            _apiService.ServerInitializedSuccess += OnServerInitialize;
        }

        private async void OnServerInitialize()
        {
            if (!IsTokenValid())
            {
                if (!string.IsNullOrEmpty(_clientData.AccessToken))
                {
                    await CheckUser();
                }
            }
        }

        private void Start()
        {
            #if !UNITY_SERVER
            DontDestroyOnLoad(this);
            #endif
        }

        private void OnDisable()
        {
            _apiService.ServerInitializedSuccess -= OnServerInitialize;
        }

        public void Initialize( SceneChangerView sceneChange, ServerVerification serverVerification)
        {
            _sceneChanger = sceneChange;
            _serverVerification = serverVerification;
            Validate();
            
            StartCoroutine(VerificationCoroutine());
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_clientData.AccessToken))
            {
                OnFail();
            }
        }

        private void OnFail()
        {
            if(_sceneChanger != null && _sceneChanger.IsNetworkScene())
            {
                string errorMessage = $"{_notAuthorizedMessageLocalizationVariant.Localize()}\n{_returningMessageLocalizationVariant}";
                _sceneChanger.ChangeSceneWithErrorText(SpaceType.EnterScene, deleteData: true, errorMessage);
            }
        }

        private async UniTask<bool> CheckUser()
        {
            Validate();
            var utcs = new UniTaskCompletionSource<bool>();
            
            bool result = await _apiService.RefreshToken();
            if(result)
            {
                if(_serverVerification != null)
                    _serverVerification.ChangeTokenOnUserListServerRpc(_clientData.UserId.ToString(),_clientData.AccessToken);
                utcs.TrySetResult(true);
            }
            else
            {
                utcs.TrySetResult(false);
                _clientData.DeleteAllData();
                OnFail();
            }

            return await utcs.Task;
        }

        private IEnumerator VerificationCoroutine()
        {
            while (true)
            {
                if (!IsTokenValid())
                {
                    yield return CheckUser();
                }
                yield return new WaitForSeconds(61f);
            }
        }

        private bool IsTokenValid()
        {
            DateTime now = DateTime.UtcNow;
            expTime = GetExpTime();
            TimeSpan timeDifferent = expTime - now; 

            if (timeDifferent.Days <= 0 && timeDifferent.Hours <= 0 && timeDifferent.Minutes < 5 )
            {
                return false;
            }

            return true;
        }

        private DateTime GetExpTime()
        {
            DateTime expTime = DateTime.UtcNow;
            if (long.TryParse(_clientData.ExpTokenUnixTime, out long time))
            {
                expTime = time.UnixToDateTime();
            }
            else
            {
                OnFail();
            }

            return expTime;
        }
    }
}