using System;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using Infrastructure.Factory;
using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Infrastructure.AssetManagement;
using Assets.Scripts.Event;
using System.Collections.Generic;
using Assets.Scripts.Hall;
using Client;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Assets.Scripts.Models;
using Assets.Scripts.Trading;
using Mechanics.SendPurchaseSpaceRequest;

namespace Assets.Mechanics.Doors
{
    public class ActiveDoor : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] [SerializeField] private bool _isPrivate = true;
        [HideInInspector] [SerializeField] private SpaceType _spaceType = SpaceType.Null;
        [HideInInspector] [SerializeField] private Texture2D _emptyDoorImageTexture;
        [HideInInspector] [SerializeField] private Image _doorImage;
        [Space] 
        [SerializeField] private DoorAccessController _doorAccessController;
        [SerializeField] private SendPurchaseSpaceRequestController _sendPurchaseSpaceRequestController;
        [SerializeField] private int _numberInHall;
        [SerializeField] private TMP_Text _doorText;
        [SerializeField] private TMP_Text _doorNameText;
        [SerializeField] public Transform RespawnPosition;
        [SerializeField] private LoadSceneProgressBar _loadSceneProgressBar;
        [SerializeField] private DoorLike _doorLike;
        [Space] [SerializeField] protected InputActionReference TeleportActivateR = null;
        [SerializeField] protected InputActionReference TeleportActivateL = null;
        [SerializeField] private DoorFrameView _doorFrame;
        
        public bool IsPrivate => _isPrivate;
        public string SpaceId => _spaceId;
        public SpaceType SpaceType => _spaceType;
        public int NumberInHall => _numberInHall;
        public bool IsPrivatePlace => _myPlace != null;

        public bool IsLocalUserOwner => _myPlace != null && _clientData != null
                                                         && _myPlace.SpaceOwner != null && _myPlace.SpaceOwner.UserId ==
                                                         _clientData.UserId;

        public bool IsLocalUserAdmin => _isLocalUserAdmin;
        public AccessResultDto AccessData => _doorAccessData;
        public bool IsPaidDoor => _myPlace.PublicAccessType == SpaceAccessType.Paid;
        private SceneChangerView _sceneChangerView;
        private LocalSpace _myPlace = null;
        private SceneFactory _sceneFactory;
        private string _spaceId;
        private string _spaceName;
        private string _remoteSceneName;
        private IClientData _clientData;
        private ISpaceManager _spaceManager;
        private string _remoteFolderName;
        private string _remoteCatalogName;

        //private bool _isOwnerInside;
        private AccessResultDto _doorAccessData;

        //private EventAccessType _eventAccessType;
        private TowerEvent _currentTowerEvent;
        private bool _isLocalUserAdmin;
        private string _initialDoorText;

        public event Action SpaceAvailabilityChanged;
        public event Action<TowerEvent> SpaceTowerEventChanged;
        MyImageDownloader imageDownloader;
        private AsyncOperationHandle<SceneInstance> operationHandler;

        [Inject]
        public void Construct(IClientData clientData, ISpaceManager spaceManager)
        {
            _clientData = clientData;
            _spaceManager = spaceManager;
        }

        private void Awake()
        {
            _initialDoorText = _doorNameText.text;
            SetAccessData(null);
            //RefreshDoorAccessType();
        }

        private void Start()
        {
            //temp
            if (!_isPrivate) _doorImage.enabled = false;

            imageDownloader = new();
            imageDownloader.Init();

            if (_sceneChangerView == null)
            {
                _sceneChangerView = FindObjectOfType<SceneChangerView>();
            }
        }

        public void Init(SceneChangerView sceneChangerView)
        {
            _sceneChangerView = sceneChangerView;
        }

        public bool IsSpaceAvailable()
        {
            if (AccessData != null)
            {
                return AccessData.CanBeHere;
            }

            if (SpaceType == SpaceType.HallScene)
            {
                return true;
            }

            return false;

            //if (_myPlace.Id == Guid.Empty)
            //{
            //    return false;
            //}

            //if (IsLocalUserAdmin || IsLocalUserOwner)
            //{
            //    return true;
            //}
            //return (_eventAccessType != EventAccessType.NoTicket)
            //    && (_doorAccessType == DoorAccessType.Opened || _doorAccessType == DoorAccessType.OwnerIsInSpace);
        }

        public void SetLocalSpace(LocalSpace localSpace, AccessResultDto accessData)
        {
            _myPlace = null;
            _spaceId = localSpace.Id.ToString();
            _spaceName = localSpace.SpaceName;
            _doorNameText.text = _initialDoorText;
            _remoteSceneName = localSpace.RemoteSceneName;
            _remoteFolderName = localSpace.RemoteFolderName;
            _remoteCatalogName = localSpace.RemoteCatalogName;
            _doorAccessController.SetDoorLocalSpace(localSpace,this);
            //_isOwnerInside = false;
            _sendPurchaseSpaceRequestController.SetLocalSpace(localSpace);
            SetAccessData(accessData);

            SetSpaceToDoorLike(localSpace);
            
        }

        public void SetDoorFrame(DoorFrameType type)
        {
            switch (type)
            {
                case DoorFrameType.Free: 
                    _doorFrame.SetDoorGreen();
                    break;
                case DoorFrameType.Gold:
                    _doorFrame.SetDoorGold();
                    break;
            }
        }

        public void SetTowerEvent(TowerEvent currentEvent)
        {
            //SetAccessData(accessData);
            _currentTowerEvent = currentEvent;
            //_doorAccessData = accessData;
            //RefreshEventAccessType();
            SpaceTowerEventChanged?.Invoke(_currentTowerEvent);

            //RefreshDoorAccessType();
        }

        public void RefreshAdminStateByUserAdminSpaces(List<string> adminSpacesOfLocalUser)
        {
            _isLocalUserAdmin = GetAdminStateByUserAdminSpaces(adminSpacesOfLocalUser);
            //RefreshDoorAccessType();
        }

        public void SetMySpace(LocalSpace myPlace, AccessResultDto accessData) //occupied space
        {
            _myPlace = myPlace;
            _spaceId = _myPlace.Id.ToString();
            _spaceName = _myPlace.SpaceName;
            _doorText.text = $"{_numberInHall}";
            _doorNameText.text = myPlace.SpaceName;
            _remoteSceneName = myPlace?.RemoteSceneName;
            _remoteFolderName = myPlace?.RemoteFolderName;
            _remoteCatalogName = myPlace?.RemoteCatalogName;
            //_isOwnerInside = isOwnerInside;
            _doorAccessController.SetDoorLocalSpace(myPlace,this);
            _sendPurchaseSpaceRequestController.SetLocalSpace(myPlace);

            if (myPlace.DoorImage != null && myPlace.DoorImage.Id != Guid.Empty)
            {
                //SetFullTexture replaced by SetPreviewTexture!!!
                imageDownloader.SetPreviewTexture(_myPlace.DoorImage, _doorImage, _numberInHall.ToString());
                _doorImage.color = Color.white;
            }
            else _doorImage.color = new Color(1, 1, 1, 0);

            SetAccessData(accessData);

            SetSpaceToDoorLike(myPlace);
            
        }

        public void SetEmptyMySpace() //for empty space
        {
            _myPlace = new LocalSpace {Id = Guid.Empty};
            _spaceId = null;
            _spaceName = "";
            //_isOwnerInside = false;

            _doorText.text = $"{_numberInHall}";

            if (_doorImage.sprite.name == "nullImage")
                _doorImage.sprite = Sprite.Create(_emptyDoorImageTexture,
                    new Rect(0, 0, _emptyDoorImageTexture.width, _emptyDoorImageTexture.height), Vector2.one / 2);
            _doorAccessController.SetDoorLocalSpace(_myPlace,this);
            _sendPurchaseSpaceRequestController.SetLocalSpace(_myPlace);

            SetAccessData(null);
        }

        public void SetEmptySpace()
        {
            _myPlace = new LocalSpace {Id = Guid.Empty};
            _spaceId = null;
            _spaceName = "";
            //_isOwnerInside = false;

            _doorText.text = "";
            _doorNameText.text = "";
            _doorAccessController.SetDoorLocalSpace(_myPlace,this);
            _sendPurchaseSpaceRequestController.SetLocalSpace(_myPlace);

            SetAccessData(null);
        }

        public void SetAccessData(AccessResultDto accessData)
        {
            _doorAccessData = accessData;
            RefreshDoorAccessType();
        }

        [ContextMenu("OnEditorSelect")]
        private void OnEditorSelect()
        {
            OnSelect();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelect();
        }
        
        public async void OnSelect()
        {
#if !UNITY_SERVER
            bool result = await _doorAccessController.AllowedDoorEnter();

            if (result)
            {
                TryLoadSpace();
            }
#endif
        }

        private async void TryLoadSpace()
        {
            if (_clientData.LastSpaceDoorData != null && _clientData.LastSpaceDoorData.SpaceType != SpaceType.HallScene)
            {
                ClientDataInSpace clientDataInSpace = new();
                clientDataInSpace.Clear();
            }

            if (TeleportActivateR.action.inProgress || TeleportActivateL.action.inProgress)
            {
                return;
            }

            if (!IsSpaceAvailable())
            {
                return;
            }

            if (_spaceType != SpaceType.HallScene)
            {
                _clientData.SetLastSpaceDoorData(new SpaceDoorData(_isPrivate, _spaceType, _spaceId));
            }

            if (!string.IsNullOrEmpty(_remoteSceneName))
            {
                string catalogUrl = ResourcesLocation.GetRemoteScenePath(_remoteFolderName, _remoteCatalogName);

                StartLoadScene(catalogUrl);
            }
            else
            {
                var currentSceneSpaceType = _myPlace == null ? _spaceType : _myPlace.SpaceType;

                var spaceName = _myPlace != null ? _myPlace.SpaceName : _spaceName;
                if (_spaceType == SpaceType.HallScene)
                {
                    spaceName = await _spaceManager.FindHallNameOfCurrentSpace();
                }

                _sceneChangerView.ChangeSpace(currentSceneSpaceType, _spaceId, spaceName);
            }
        }

        private void StartLoadScene(string catalogUrl)
        {
            TeleportActivateR.action.Disable();
            TeleportActivateL.action.Disable();

            if (_loadSceneProgressBar != null)
                _loadSceneProgressBar.ShowProgressBar();

            _sceneFactory = new SceneFactory(new AddressableResourcesLocation(catalogUrl, LoadSceneAsync));
        }

        private async void LoadSceneAsync()
        {
            if (operationHandler.IsValid()) return;

            operationHandler = _sceneFactory.LoadSceneAsync(_remoteSceneName);
            operationHandler.Completed += handle =>
            {
                TeleportActivateR.action.Enable();
                TeleportActivateL.action.Enable();
            };
            if (_loadSceneProgressBar != null)
                _loadSceneProgressBar.SetAsyncOperation(operationHandler);

            await operationHandler;

            var currentSceneSpaceType = _myPlace == null ? _spaceType : _myPlace.SpaceType;
            Debug.LogError(_myPlace != null ? _myPlace.SpaceName : _spaceName);
            _sceneChangerView.ChangeSpace(currentSceneSpaceType, _spaceId, _myPlace != null ? _myPlace.SpaceName : _spaceName);
        }

        private void RefreshDoorAccessType()
        {
            SpaceAvailabilityChanged?.Invoke();
        }

        private bool GetAdminStateByUserAdminSpaces(List<string> adminSpacesOfLocalUser)
        {
            if (adminSpacesOfLocalUser == null)
            {
                return false;
            }

            string localSpaceId = _myPlace != null && _myPlace != null
                ? _myPlace.Id.ToString()
                : _spaceId;
            if (string.IsNullOrEmpty(localSpaceId))
            {
                return false;
            }

            return adminSpacesOfLocalUser.Contains(localSpaceId);
        }

        private void SetSpaceToDoorLike(LocalSpace space)
        {
            if (_doorLike != null)
            {
                _doorLike.SetSpaceFromHall(space);
            }
        }
    }

    public enum DoorFrameType
    {
        Default,
        Free,
        Gold
    }
}