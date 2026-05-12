using Assets.Mechanics.AirLocomotion.Scripts;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Data;
using Mechanics.LoadSceneObjects;
using Mechanics.Network.Scripts.StaticObjectsService;
using Mechanics.SpaceStaticObjectEditing.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mechanics.SpaceStaticObjectEditing
{
    public class SpaceEditingService : NetworkBehaviour, ISpaceEditingService
    {
        #region Inspector

        [SerializeField] private SpaceEditingPanel _spaceEditingPanel;
        [SerializeField] private GameObject _locomotionZone;
        [SerializeField] private AirLocomotionZoneVisualizer _locomotionZoneVizualizer;
        [SerializeField] private Button _enableSpaceAdministrationButton;
        [SerializeField] private APIClientSideSceneFunctionalSpawner _apiClientSideSceneObjectSpawner;
        [SerializeField] private APIServerSideSceneFunctionalSpawner _apiServerSideObjectSpawner;

        #endregion

        private ulong _adminId;
        private IStaticObjectsService _staticObjectService;
        private ISpaceRepository _spaceRepository;

        private bool _isModeEnable = false;
        private string _roomId = string.Empty;

        [Inject]
        private void Construct(IStaticObjectsService staticObjectsService, ISpaceRepository spaceRepository)
        {
            _staticObjectService = staticObjectsService;
            _spaceRepository = spaceRepository;
#if UNITY_SERVER
            _roomId = DataExtensions.GetSpaceID();
#endif
        }

        private void OnEnable()
        {
            _locomotionZone.SetActive(_isModeEnable);
            _spaceEditingPanel.CloseButtonClick += OnCloseButtonClick;
            _enableSpaceAdministrationButton.onClick.AddListener(() => ToogleEditingMode());
        }

        private void OnCloseButtonClick()
        {
            ToogleEditingMode();
        }

        private void OnDisable()
        {
            _spaceEditingPanel.CloseButtonClick -= OnCloseButtonClick;
            _enableSpaceAdministrationButton.onClick.RemoveListener(() => ToogleEditingMode());
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        public void ToogleEditingMode()
        {
            _isModeEnable = !_isModeEnable;

            _locomotionZone.SetActive(_isModeEnable);
            if (!_isModeEnable)
            {
                _locomotionZoneVizualizer.ForseStopLocomotion();
                InvokeSaveOnClient();
            }
            else
            {
                _apiClientSideSceneObjectSpawner.HideVizuale();
            }
        }

        public bool EditingModeIsEnable()
        {
            return _isModeEnable;
        }

        public void InvokeSaveOnClient()
        {
            SaveDataServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SaveDataServerRpc()
        {
            SaveData();
        }

        [ClientRpc]
        private void OnSaveDataClientRpc()
        {
            _apiClientSideSceneObjectSpawner.CheckExistSceneObjects();
        }

        public async void SaveData()
        {
            Debug.LogWarning("SaveData");
            var staticObjects = _spaceRepository.GetSpaceObjectsList();
            
            bool result = await _staticObjectService.SaveSceneStaticObjects(staticObjects,_roomId);
            _apiServerSideObjectSpawner.CheckExistSceneObjects();
            OnSaveDataClientRpc();
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (clientId == _adminId)
                SaveData();
        }
    }
}