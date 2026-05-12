using System;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Client;
using Data;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class ApplicationBootstrapper : MonoBehaviour
    {
        #region Inspector

        public DiscoveryServiceStaticData DiscoveryService;
        public ClientVerification ClientVerification;

        #endregion

        #region PrivateVariables

        private IApiService _apiService;
        private IServerApiService _serverApiService;
        private IClientData _clientData;
        private DiContainer _diContainer;
        #endregion

        #region Events
        public event Action ProfileChange;

        #endregion
        
        [Inject]
        public void Construct(IClientData clientData, IApiService apiService, IServerApiService serverApiService, DiContainer diContainer)
        {
            _clientData = clientData;
            _apiService = apiService;
            _serverApiService = serverApiService;
            _diContainer = diContainer;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
#if !UNITY_SERVER
            _diContainer.InstantiatePrefab(ClientVerification);
#endif
        }

        public async void InitApplication()
        {
            CheckChangeProfile();
#if UNITY_SERVER
            await _serverApiService.Initialize();
#else
            await _apiService.Initialize(assembly:DiscoveryService.Assembly);
#endif
        }

        private void CheckChangeProfile()
        {
            bool isProfileChange = _clientData.AssemblyType != DiscoveryService.Assembly.AssemblyType.ToString();
            if (isProfileChange)
            {
                ProfileChange?.Invoke();
                _clientData.DeleteAllData();    
            }
        }
    }
}