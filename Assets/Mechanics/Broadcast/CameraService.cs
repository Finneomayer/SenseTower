using System;
using System.Collections;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.TowerObjects;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Infrastructure.Factory;
using Oculus.Avatar2;
using Proyecto26;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Broadcasting
{
    //public static class CameraParameters
    //{
    //    public static Guid CurrentId;
    //}
    public class CameraService : NetworkBehaviour
    {
        #region Inspector

        [SerializeField] private AvatarLODManager _avatarManager;

        [SerializeField] private NetworkFactory _networkFactory;

        #endregion

        private string _cameraOwnerId = string.Empty;
        private GameObject _cameraInstance;
        private GameObject _cameraGraphic;

        private Vector3 _cameraPosition = Vector3.zero;
        private Quaternion _cameraRotation = Quaternion.identity;
        public bool _recState { private set; get; } = false;
        private IClientData _clientData;
        private bool _isOwnerCamera = false;

        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        private void Start()
        {
            _networkFactory.CameraDespawn += OnFactoryCameraDespawn;
            _networkFactory.CameraCreate += OnFactoryCameraCreate;
        }

        private void OnDisable()
        {
            _networkFactory.CameraDespawn -= OnFactoryCameraDespawn;
            _networkFactory.CameraCreate -= OnFactoryCameraCreate;
        }

        public override void OnNetworkSpawn()
        {
#if !UNITY_SERVER
            GetCameraDataServerRpc(NetworkManager.Singleton.LocalClientId, _clientData.UserId.ToString());
#endif
#if UNITY_SERVER
            NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
#endif
        }

        public override void OnNetworkDespawn()
        {
#if UNITY_SERVER
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
#endif
        }

        public async UniTask<bool> StopCameraBroadcast(string id)
        {
            RequestHelper options = new RequestHelper();

            await UniTask.WaitUntil(() => (!string.IsNullOrEmpty(APIService.GetBroadcastingServiceEndPoint)));
            
            options.Uri = $"{APIService.GetBroadcastingServiceEndPoint}/{id}/stop";
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            ClearCameraDataServerRpc();
            var result = await WebRequestFunctions.Post(options);

            return result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
        }

        public void SaveRecStateOnServer(bool recState)
        {
            _recState = true;
            _cameraPosition = _cameraGraphic.transform.position;
            _cameraRotation = _cameraGraphic.transform.rotation;

            SaveCameraDataServerRpc(_clientData.UserId.ToString(), _cameraPosition, _cameraRotation);
            SetRecStateServerRpc(recState);
        }

        #region ServerRpc

        [ServerRpc(RequireOwnership = false)]
        private void GetCameraDataServerRpc(ulong userId, string clientId)
        {
            GameObject tempCamera = _networkFactory.GetCamera();
            if (tempCamera != null)
            {
                if (_cameraOwnerId.Equals(clientId))
                {
                    _networkFactory.ChangeOwnerNetworkCamera(userId);
                }
                if(_recState)
                    SetCameraDataClientRpc(userId, _cameraOwnerId, _cameraPosition, _cameraRotation);
                else
                    SetCameraDataClientRpc(userId, _cameraOwnerId, tempCamera.transform.position,
                        tempCamera.transform.rotation);
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void SetRecStateServerRpc(bool recState)
        {
            _recState = recState;
            SetRecStateAllClientRpc(_recState);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetRecStateServerRpc(ulong userId)
        {
            SetRecStateClientRpc(userId, _recState);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ClearCameraDataServerRpc()
        {
            _recState = false;
            _cameraOwnerId = string.Empty;
            _cameraPosition = Vector3.zero;
            _cameraRotation = Quaternion.identity;
            ClearCameraDataClientRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SaveCameraDataServerRpc(string userId, Vector3 position, Quaternion quaternion)
        {
            _cameraOwnerId = userId;
            _cameraPosition = position;
            _cameraRotation = quaternion;
        }

        #endregion

        #region ClientRpc

        [ClientRpc]
        private void SetRecStateClientRpc(ulong userId, bool recState)
        {
            _recState = recState;
            if (userId != NetworkManager.Singleton.LocalClientId) return;
            if (_cameraInstance == null) return;
            if (recState && _cameraInstance.TryGetComponent(out CameraCreator cameraCreator))
            {
                cameraCreator.RecState();
            }
        }
        
        [ClientRpc]
        private void SetRecStateAllClientRpc(bool recState)
        {
            Debug.LogError($"Set rec state {recState}");
            _recState = recState;
            if (_cameraInstance == null) return;
            if (_cameraInstance.TryGetComponent(out CameraCreator cameraCreator))
            {
                if(recState)
                    cameraCreator.BeforeRecState();
                else
                    cameraCreator.StartState();
            }
        }

        [ClientRpc]
        private void ClearCameraDataClientRpc()
        {
            _recState = false;
            _cameraOwnerId = string.Empty;
            _cameraPosition = Vector3.zero;
            _cameraRotation = Quaternion.identity;
            if (!_recState && _cameraInstance != null)
            {
                if (_cameraInstance.TryGetComponent(out CameraCreator cameraCreator))
                {
                    cameraCreator.StartState();
                }
            }
        }

        [ClientRpc]
        private void SetCameraDataClientRpc(ulong serverClientId, string userId, Vector3 position,
            Quaternion quaternion)
        {
            _cameraOwnerId = userId;
            _networkFactory.CreateCamera(position, quaternion);
        }

        #endregion

        private void SaveAndSetCameraData(bool isOwner, Vector3 position, Quaternion rotation)
        {
            if (_cameraInstance.TryGetComponent(out CameraCreator cameraCreator))
            {
                _cameraGraphic = cameraCreator.GetCameraGrab();
                if (_avatarManager != null)
                    _avatarManager.AddExtraCamera(cameraCreator.GetPreviewCamera());

                cameraCreator.SetCameraService(this);
                _isOwnerCamera = isOwner || _cameraOwnerId == _clientData.UserId.ToString();
                
                cameraCreator.IsOwner(_isOwnerCamera);
                _cameraGraphic.transform.SetPositionAndRotation(position, rotation);
            }
        }

        private void OnFactoryCameraCreate(bool isOwner, GameObject cameraInstance)
        {
            if (_cameraInstance != null)
                return;

            _cameraInstance = cameraInstance;
            _cameraInstance = cameraInstance;
            SaveAndSetCameraData(isOwner, _cameraInstance.transform.position, _cameraInstance.transform.rotation);
            GetRecStateServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        private async void OnClientDisconnect(ulong clientId)
        {
            if (!_recState)
                _networkFactory.TryDespawnNetworkCamera(clientId);
        }

        private async void OnFactoryCameraDespawn(string id)
        {
            Debug.LogError($"Despawn {id}");
            _cameraInstance = null;
            if (_recState)
            {
                await StopCameraBroadcast(id);
            }
        }
    }
}