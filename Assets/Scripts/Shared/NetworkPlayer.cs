using System;
using System.Collections.Generic;
using Assets.Mechanics.Doors;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Assets.Scripts.Player;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Random = UnityEngine.Random;
using Assets.Mechanics.MetaAvatars.Scripts;
using UnityEngine.SceneManagement;
using Assets.Scripts.Client;
using Assets.Scripts.Server;
using Client;
using Oculus.Avatar2;
using Zenject;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Space;
using Sense.Interectable.Teleportation;
using Broadcasting;
using Mechanics.Transactions;
using Mechanics.UserWallet;
using Oculus.Interaction;
using Unity.XR.CoreUtils;
using System.Collections;
using Assets.Mechanics.Lift;
using Assets.Mechanics.Meta_Avatars.Scripts;
using Assets.Scripts.Player.Customize;
using Assets.Scripts.Player.WindowsMovement;
using Unity.VisualScripting;
using UnityEngine.XR.Management;
using Assets.Localization;
using Mechanics.FriendsList;

namespace Assets.Scripts.Shared
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private OBSBroadcasting _broadcasting;
        [SerializeField] private GameObject[] _enabledObjects;
        [SerializeField] private GameObject[] _disabledObjects;
        [SerializeField] private Animation _animation;
        [SerializeField] private InputActionManager _input;
        [SerializeField] private NetworkAvatar _networkAvatar;
        [SerializeField] private SampleAvatarEntity _remoteAvatar;
        [SerializeField] private SampleAvatarEntity _defaultAvatar;
        [SerializeField] private GameObject _localPlayerContent;
        [SerializeField] private SampleAvatarEntity _avatarEntity;
        [SerializeField] private AddingFriendHandler addingFriendHandler;

        [SerializeField] private NetworkVariable<bool> _broadcastClient = new(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> IsWinUser = new();

        private static bool isBroadcastingClient;
        public event Action LostConnection;
        private ISpaceManager _spaceManager;
        private IClientData _clientData;
        private ClientDataInSpace _clientDataInSpace = new();
        private CompositionRootNetworkScene _compositionRoot;
        private bool _hasSpawned = false;
        public bool PlayerIsClient => IsClient;
        public bool PlayerIsOwner => IsOwner;

        private TeleportationProvider _teleportationProvider;
        private TransactionInfrastructure _transactionInfrastructure;

        public event Action RemoteAvatarLoaded;

        [Inject]
        public void Construct(IClientData clientData, ISpaceManager spaceManager)
        {
            _spaceManager = spaceManager;
            _clientData = clientData;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
            {
                return;
            }

            _broadcastClient.Value = isBroadcastingClient;


            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _clientData = commonDIInstaller.Resolve<IClientData>();
                _spaceManager = commonDIInstaller.Resolve<ISpaceManager>();
            }

            DisableClientInput();
            StartCoroutine(SetTeleports());
            SetInput();
            SetPlayerOwnerToScene();
            AddClientOnServerVerification();
            //SetCachedPosition();
            GetTransactionManager();
            CheckIfWinUser();

            _hasSpawned = true;
        }

        public Transform GetRemotePlayerRightHand()
        {
            return _remoteAvatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.RightHandWrist);
        }

        public Transform GetRemotePlayerHead()
        {
            return _remoteAvatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
        }

        public void SetVisibleToOthers(bool isVisible)
        {
            _networkAvatar.SetVisibleToOthers(isVisible);
        }

        public SampleAvatarEntity GetDefaultAvatar()
        {                    
            return _defaultAvatar;
        }

        private async void CheckBroadcasting()
        {
#if UNITY_STANDALONE_WIN
            bool checkBroadcast = await _broadcasting.CheckCapability();

            if (checkBroadcast)
            {
                isBroadcastingClient = true;
                _broadcasting.StartBroadcasting();
            }
#endif
        }

        private async void AddClientOnServerVerification()
        {
            if (!IsOwner) return;

            var serverService = FindObjectOfType<ServerVerification>();
            if (serverService == null) return;
            await UniTask.WaitForEndOfFrame();

            serverService.AddUserToCheckTokenList(_clientData.UserId.ToString(), _clientData.UserName, NetworkManager.Singleton.LocalClientId,
                _clientData.AccessToken, LocalizationManager.CurrentLanguageCode);
        }

        private void GetTransactionManager()
        {
            _transactionInfrastructure = FindObjectOfType<TransactionInfrastructure>();
            if (_transactionInfrastructure != null)
            {
                _transactionInfrastructure.TransactionRecipientEnd += OnEndTransaction;
                _transactionInfrastructure.TransactionInitiatorEnd += OnEndTransaction;
            }
        }

        private void OnEndTransaction(bool success)
        {
            if (success)
                _animation.Play();
        }

        private void SetPlayerOwnerToScene()
        {
            if (IsOwner)
            {
                var playerOwner = GetComponent<PlayerLogic>();
                var ownerId = GetComponent<NetworkObject>().OwnerClientId;

                _compositionRoot = FindObjectOfType<CompositionRootNetworkScene>();
                _compositionRoot?.InitZones(ownerId, playerOwner);
                _compositionRoot?.InitPlayer(playerOwner, this).Forget();
            }
        }

        private void SetInput()
        {
            if (IsClient && IsOwner)
            {
                _input.enabled = true;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner) LostConnection?.Invoke();

            base.OnNetworkDespawn();
            Destroy(this);
        }

        private IEnumerator SetTeleports()
        {
            yield return null;

            if (IsClient && IsOwner)
            {
                _teleportationProvider = GetComponent<TeleportationProvider>();
                _teleportationProvider.endLocomotion += TeleportationProviderOnEndLocomotion;

                CustomTeleportationInteractable[] anchors = FindObjectsOfType<CustomTeleportationInteractable>();
                TeleportationArea[] areas = FindObjectsOfType<TeleportationArea>();
                if (anchors != null && _teleportationProvider != null)
                {
                    foreach (var anchor in anchors)
                    {
                        anchor.teleportationProvider = _teleportationProvider;
                    }
                }

                if (areas != null && _teleportationProvider != null)
                {
                    foreach (var area in areas)
                    {
                        area.teleportationProvider = _teleportationProvider;
                    }
                }
            }
        }

        private void TeleportationProviderOnEndLocomotion(LocomotionSystem locomotionSystem)
        {
            if (_spaceManager == null || _spaceManager.CurrentTransitionTarget == null)
            {
                return;
            }
            if (!_spaceManager.CurrentTransitionTarget.IsPrivate) return;

            _clientDataInSpace.LastSpaceID = _spaceManager.CurrentTransitionTarget.Id.ToString();

            if (!string.IsNullOrEmpty(_spaceManager.CurrentTransitionTarget.RemoteSceneName))
                _clientDataInSpace.LastSpaceName = _spaceManager.CurrentTransitionTarget.RemoteSceneName;

            _clientDataInSpace.LastSpacePosition = locomotionSystem.xrOrigin.Origin.transform.position;
            _clientDataInSpace.LastSpaceRotation = locomotionSystem.xrOrigin.Origin.transform.rotation.eulerAngles;
        }

        private void DisableClientInput()
        {
            if (IsClient && !IsOwner)
            {
                var clientMoveProvider = GetComponent<NetworkMoveProvider>();
                var clientControllers = GetComponentsInChildren<ActionBasedController>();
                var clientTurnProvider = GetComponent<ActionBasedSnapTurnProvider>();
                var clientHead = GetComponentInChildren<TrackedPoseDriver>();

                foreach (var input in clientControllers)
                {
                    input.enabled = false;
                }


                foreach (var item in _enabledObjects)
                {
                    item.SetActive(true);
                }

                foreach (var item in _disabledObjects)
                {
                    item.SetActive(false);
                }

                Destroy(GetComponent<XROrigin>());
                Destroy(GetComponent<InputActionManager>());
                Destroy(GetComponent<LocomotionSystem>());
                Destroy(GetComponent<TeleportationProvider>());
                Destroy(GetComponent<SnapTurnProviderBase>());
            }
        }

        private void OnLocalAvatarLoad(OvrAvatarEntity call)
        {
            ChangeMaterialRenderOnLocalAvatar(call.transform);
        }
        
        private void OnRemoteAvatarLoad(OvrAvatarEntity call)
        {
            var networkPlayerComponent = call.transform.GetComponentInParent<NetworkPlayer>();
            if (networkPlayerComponent != null)
            {
                if (networkPlayerComponent._broadcastClient.Value)
                {
                    call.transform.GetComponentInParent<OBSBroadcasting>().HideCameraRemoteAvatar();
                }
            }

            var chestTransform = _remoteAvatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Chest);
            if (chestTransform != null && !chestTransform.gameObject.TryGetComponent(out CapsuleCollider _))
            {
                CapsuleCollider chestCollider = chestTransform.gameObject.AddComponent<CapsuleCollider>();
                chestCollider.center = Vector3.left * 0.5f;
                chestCollider.height = 1.5f;
                chestCollider.radius = 0.4f;
                chestCollider.direction = 0;
                chestCollider.isTrigger = true;
            }

            var headTransform = _remoteAvatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
            if (headTransform != null && !headTransform.gameObject.TryGetComponent(out SphereCollider _))
            {
                SphereCollider headCollider = headTransform.gameObject.AddComponent<SphereCollider>();
                headCollider.radius = 0.2f;
                headCollider.center = Vector3.right * 0.1f;
                headCollider.isTrigger = true;
            }

            var leftTransform = _remoteAvatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.LeftHandWrist);
            if (leftTransform != null && !leftTransform.gameObject.TryGetComponent(out SphereCollider _))
            {
                SphereCollider leftWristCollider = leftTransform.gameObject.AddComponent<SphereCollider>();
                leftWristCollider.radius = 0.1f;
                leftWristCollider.center = Vector3.right * 0.05f;
                leftWristCollider.isTrigger = true;
            }

            var rightTransform = _remoteAvatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.RightHandWrist);
            if (rightTransform != null && !rightTransform.gameObject.TryGetComponent(out SphereCollider _))
            {
                SphereCollider rightWristCollider = rightTransform.gameObject.AddComponent<SphereCollider>();
                rightWristCollider.radius = 0.1f;
                rightWristCollider.center = Vector3.left * 0.05f;
                rightWristCollider.isTrigger = true;
            }

            if (!IsWinUser.Value && 
                XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                AddFriendingObjects(leftTransform, rightTransform);
            }
            else
            {
                DestroyRemoteFriendingObjects();
            }

            RemoteAvatarLoaded?.Invoke();
            //MafiaRemotePlayerVisual mafiaRemotePlayerVisual = GetComponent<MafiaRemotePlayerVisual>();
            //if (mafiaRemotePlayerVisual == null)
            //{
            //    Debug.LogError("mafiaRemotePlayerVisual == null");
            //}
            //mafiaRemotePlayerVisual.SetMaskVisible(true);

            //var mesh =_remoteAvatar.transform.GetComponentInChildren<SkinnedMeshRenderer>();

            //mesh.AddComponent<OVRFaceExpressions>();
            //var face = mesh.AddComponent<OVRCustomFace>();
            //face.AutoMapBlendshapes();

            //Debug.LogWarning(mesh);
        }

        private void AddFriendingObjects(Transform leftHandTransform, Transform rightHandTransform)
        {
            if (leftHandTransform == null || rightHandTransform == null)
            {
                return;
            }

            if (IsOwner)
            {
                var playerOwner = GetComponent<PlayerLogic>();
                var playerEmoji = GetComponent<PlayerEmoji>();
                AddingFriendCollider[] addingFriendColliders = new AddingFriendCollider[2];
                addingFriendColliders[0] = InitLocalFriendingObj(leftHandTransform, playerOwner.GetLeftArm());
                addingFriendColliders[1] = InitLocalFriendingObj(rightHandTransform, playerOwner.GetRightArm());

                addingFriendHandler.gameObject.SetActive(true);
                addingFriendHandler.Init(addingFriendColliders, playerEmoji);
            }
            else
            {
                InitRemoteFriendingObj(leftHandTransform);
                InitRemoteFriendingObj(rightHandTransform);
                addingFriendHandler.gameObject.SetActive(false);
            }
        }

        private void DestroyRemoteFriendingObjects()
        {
            AddingFriendRemoteCollider[] addingFriendColliders = GetComponentsInChildren<AddingFriendRemoteCollider>();
            foreach (var addingFriendCollider in addingFriendColliders)
            {
                Destroy(addingFriendCollider);
            }
        }

        private AddingFriendCollider InitLocalFriendingObj(Transform handTransform, PlayerController playerController)
        {
            if (!handTransform.TryGetComponent(out AddingFriendCollider addingFriendCollider))
            {
                addingFriendCollider = handTransform.AddComponent<AddingFriendCollider>();
            }
            addingFriendCollider.Init(playerController);
            return addingFriendCollider;
        }

        private void InitRemoteFriendingObj(Transform handTransform)
        {
            if (!handTransform.TryGetComponent(out AddingFriendRemoteCollider addingFriendCollider))
            {
                addingFriendCollider = handTransform.AddComponent<AddingFriendRemoteCollider>();
            }
        }

        private void ChangeMaterialRenderOnLocalAvatar(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var tempRender = parent.GetChild(i).GetComponentInChildren<OvrAvatarSkinnedRenderable>();
                if (tempRender != null)
                {
                    tempRender.gameObject.layer = LayerMask.NameToLayer("Body");
                    if (tempRender.TryGetComponent(out MeshRenderer meshRenderer))
                    {
                        meshRenderer.material.renderQueue = 4000;
                    }
                }
            }
        }

        private void SetPosition(Transform targetTransform)
        {
            if (XRGeneralSettings.Instance.Manager.isInitializationComplete) //is VR client
            {
                transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
            }
            else //is not VR client
            {
                GetComponent<EditorMovementSystem>().SetPosition(targetTransform.position, targetTransform.rotation);
            }
        }

        public async UniTask<bool> SetCachedPosition()
        {
            if (_spaceManager == null || _spaceManager.CurrentTransitionTarget == null
                || _clientDataInSpace == null || string.IsNullOrEmpty(_clientDataInSpace.LastSpaceID))
            {
                _clientDataInSpace.Clear();
                return false;
            }

            if (_spaceManager.CurrentTransitionTarget.Id != Guid.Parse(_clientDataInSpace.LastSpaceID))
            {
                _clientDataInSpace.Clear();
                return false;
            }

            if (string.IsNullOrEmpty(_clientDataInSpace.LastSpaceName))
            {
                _clientDataInSpace.Clear();
                return false;
            }

            if (_clientDataInSpace.LastSpacePosition == Vector3.zero)
            {
                _clientDataInSpace.Clear();
                return false;
            }

            transform.position = _clientDataInSpace.LastSpacePosition;
            transform.rotation = Quaternion.Euler(_clientDataInSpace.LastSpaceRotation);

            if (ulong.TryParse(_clientDataInSpace.OccupiedPlaceNetworkObjectId, out ulong occupiedPlaceNetworkObjectId))
            {
                PlaceReconnector placeReconnector = FindObjectOfType<PlaceReconnector>();
                if (placeReconnector != null)
                {
                    await placeReconnector.RestoreInPlace(this, occupiedPlaceNetworkObjectId);
                }
            }
            
            _clientDataInSpace.Clear();
            return true;
        }

        public async UniTask SetLastDoorPosition(List<Transform> spawnPoints)
        {
            await UniTask.WaitUntil(() => _hasSpawned = true);

            if (SceneManager.GetActiveScene().name is "TheHallScene" or "TheHallScene2")
            {
                ActiveDoorController doorController = FindObjectOfType<ActiveDoorController>();
                if (doorController != null)
                {
                    await UniTask.WaitUntil(() => doorController.DoorsAreInitialized);
                }

                if (PlayerLiftPosition.PutPlayerToTheLift)
                {
                    if (PlayerLiftPosition.PlayerOnFloorIndex == 0)
                    {
                        
                        var points = _compositionRoot.FirstFloorPoints;
                        int i = Random.Range(0, points.Length);
                        SetPosition(points[i]);
                    }
                    if (PlayerLiftPosition.PlayerOnFloorIndex == 1)
                    {
                        
                        var points = _compositionRoot.SecondFloorPoints;
                        int i = Random.Range(0, points.Length);
                        SetPosition(points[i]);
                    }

                    PlayerLiftPosition.PutPlayerToTheLift = false;
                    return;
                }

                if (_clientData.LastSpaceDoorData == null)
                {
                    if (spawnPoints.Count > 0)
                    {
                        int pointNumber = Random.Range(0, spawnPoints.Count);
                        //transform.position = spawnPoints[pointNumber].position;
                        SetPosition(spawnPoints[pointNumber]);
                    }

                    return;
                }

                if (doorController == null)
                {
                    return;
                }

                ActiveDoor door = null;
                if (!string.IsNullOrEmpty(_clientData.LastSpaceDoorData.SpaceId))
                {
                    door = await doorController.FindDoor(_clientData.LastSpaceDoorData.SpaceId);
                }

                if (door == null)
                {
                    door = await doorController.FindDoor(_clientData.LastSpaceDoorData.SpaceType);
                }

                if (door != null)
                {
                    //transform.SetPositionAndRotation(door.RespawnPosition.position, door.RespawnPosition.rotation);
                    SetPosition(door.RespawnPosition);
                }
            }
            else if (spawnPoints.Count > 0)
            {
                int pointNumber = Random.Range(0, spawnPoints.Count);
                //transform.position = spawnPoints[pointNumber].position;
                SetPosition(spawnPoints[pointNumber]);
            }
        }

        public void SetActiveControl(bool active)
        {
            _localPlayerContent.SetActive(active);
        }

        public void InitAsOwner()
        {
            SetActiveControl(true);
            _networkAvatar.InitAsOwner();
        }

        private void Start()
        {
            if (!IsClient && NetworkManager != null)
            {
                return;
            }
            CheckBroadcasting();
            _remoteAvatar.OnUserAvatarLoadedEvent.AddListener(OnRemoteAvatarLoad);
            _remoteAvatar.OnDefaultAvatarLoadedEvent.AddListener(OnRemoteAvatarLoad);
            _avatarEntity.OnUserAvatarLoadedEvent.AddListener(OnLocalAvatarLoad);
            if (NetworkManager == null)
            {
                InitAsOwner();
                //SetActiveControl(true);
                //InitOwnerAvatar();
            }

            //if (IsOwner || NetworkManager == null)
            //{
            //    _networkAvatar.InitAsOwner();
            //}
        }

        //private void OnApplicationFocus(bool hasFocus)
        //{
        //    if (!IsClient || !IsOwner || Application.platform != RuntimePlatform.Android)
        //    {
        //        return;
        //    }
        //    if (!hasFocus)
        //    {
        //        _clientDataInSpace.Clear();
        //    }
        //}

        private void OnDisable()
        {
            if (_teleportationProvider != null)
                _teleportationProvider.endLocomotion -= TeleportationProviderOnEndLocomotion;

            if (_transactionInfrastructure != null)
            {
                _transactionInfrastructure.TransactionRecipientEnd -= OnEndTransaction;
                _transactionInfrastructure.TransactionInitiatorEnd -= OnEndTransaction;
            }

            _avatarEntity.OnUserAvatarLoadedEvent.RemoveAllListeners();
            _remoteAvatar.OnDefaultAvatarLoadedEvent.RemoveAllListeners();
            _remoteAvatar.OnUserAvatarLoadedEvent.RemoveAllListeners();
        }

        private void CheckIfWinUser()
        {
            if (IsClient)
            {
                if (IsOwner)
                {
                    if (Application.platform != RuntimePlatform.Android && !XRGeneralSettings.Instance.Manager.isInitializationComplete)
                    {
                        SetAsWinUserServerRpc();
                    }
                }
            }
        }

        [ServerRpc]
        private void SetAsWinUserServerRpc()
        {
            IsWinUser.Value = true;
            SetAsWinUserClientRpc();
        }

        [ClientRpc]
        private void SetAsWinUserClientRpc()
        {
            DestroyRemoteFriendingObjects();
        }
    }
}