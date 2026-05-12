using Unity.Netcode;
using UnityEngine;
using TMPro;
using Assets.Scripts.Client;
using System.Collections;
using Oculus.Avatar2;
using Zenject;
using Assets.Scripts.Shared;
using System;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Scripts.API;
using Assets.Scripts.Data;
using Data;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ClientIdView : NetworkBehaviour
{
    [SerializeField] private bool _isEnterScene = false;
    [SerializeField] private Canvas CanvasID;
    [SerializeField] private Transform _signsContainer;
    [SerializeField] private TMP_Text TextID;
    [SerializeField] private SampleAvatarEntity RemoteAvatarEntity;
    [SerializeField] private DiscoveryServiceStaticData ApplicationData;
    [SerializeField] private PlayerSign[] _badges;
    [SerializeField] private PlayerSign _isSellerSign;
    [SerializeField] private XRSimpleInteractable _nameInteractable;
    [SerializeField] private TMP_Text _signDescription;
    [SerializeField] private LocalizationVariant _notFullFledgedDescription;

    [HideInInspector] public bool IsEnterScene => _isEnterScene;

    //private const string UserStatus = "IsFullFledgedUser"; //the same in TenHoursNotificationSpawner.cs & EnterSceneCoinSpawner.cs & CoinInfrastrucure.cs

    private ulong _playerID;
    private string _playerAccountId;
    private string _playerLogin;
    private bool _isFullFledgedUser = true;
    private List<PlayerSign> _signs;
    private Coroutine _descriptionShowCoroutine;

    public bool IsFullFledgedUser => _isFullFledgedUser;
    public int CountSpaces { get; private set; } = 0;
    public bool IsSeller { get; private set; } = false;
    public int WatchID { get; private set; } = 0;
    public int WatchIdOculus { get; private set; } = 0;
    public bool IsCustomAvatarWatch { get; private set; }
    public string PlayerLogin => _playerLogin;
    public string PlayerAccountId => _playerAccountId;

    private IClientData _clientData;
    private IAccountsService _accountsService;

    public event Action OnUpdateClientInfo;

    [Inject]
    public void Construct(IClientData clientData, IAccountsService accountsService)
    {
        _clientData = clientData;
    }

    private void Start()
    {
        if (_isEnterScene)
        {
            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _clientData = commonDIInstaller.Resolve<IClientData>();
            }
            _playerLogin = _clientData.UserName;
            CountSpaces = _clientData.OwnedSpacesNumber;

            WatchID = WatchSessionData.WatchPlayerId;
            WatchIdOculus = WatchSessionData.WatchPlayerIdOculus;
            IsCustomAvatarWatch = WatchSessionData.IsCustomMetaAvatar;
        }

        if (!IsClient)
        {
            return;
        }

        StartCoroutine(RefreshPositionCoroutine());
        _signs = new List<PlayerSign>();
        _nameInteractable.hoverEntered.AddListener((e) => SignOnOnHover(
            _isFullFledgedUser ? 
                "" :
                _notFullFledgedDescription.Localize()
            ));
    }

    private IEnumerator RefreshPositionCoroutine()
    {
        const float ForwardShiftFromCamera = 0.1f;
        const float HeigthAboveHeadCenter = 0.5f;

        WaitForSeconds avatarCheckDelay = new(0.1f);
        WaitForEndOfFrame positionRefreshDelay = new();

        CanvasID.enabled = false;

        Transform avatarHeadTransform = null;
        while (avatarHeadTransform == null)
        {
            avatarHeadTransform = RemoteAvatarEntity.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
            yield return avatarCheckDelay;
        }

        CanvasID.enabled = true;

        while (true)
        {
            yield return positionRefreshDelay;

            Camera camera = Camera.main;
            if (camera == null)
            {
                continue;
            }

            //TODO: Delete, if we are shure that avatarHeadTransform can not become null;
            while (avatarHeadTransform == null)
            {
                Debug.Log($"<color=purple>ClientId.</color>. avatarHeadTransform == null. User: {TextID.text}. Reassigning...");
                avatarHeadTransform = RemoteAvatarEntity.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
                yield return avatarCheckDelay;
            }

            Vector3 camToHeadDirection = (avatarHeadTransform.position - camera.transform.position).normalized;
            camToHeadDirection.y = 0;
            if (camToHeadDirection == Vector3.zero)
            {
                camToHeadDirection = camera.transform.forward;
            }

            Vector3 newCanvasPosition = avatarHeadTransform.position + ForwardShiftFromCamera * camToHeadDirection;
            newCanvasPosition.y = avatarHeadTransform.transform.position.y + HeigthAboveHeadCenter;

            CanvasID.transform.position = newCanvasPosition;

            Vector3 newForward = CanvasID.transform.position - camera.transform.position;
            newForward.y = 0;

            if (newForward == Vector3.zero || Vector3.Dot(newForward, camera.transform.forward) < 0)
            {
                continue;
            }

            CanvasID.transform.forward = newForward;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }

        CanvasID.gameObject.SetActive(true);
     
        _playerID = NetworkObject.OwnerClientId;
        if (IsOwner)
        {
            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _clientData = commonDIInstaller.Resolve<IClientData>();
                _accountsService = commonDIInstaller.Resolve<IAccountsService>();
            }
            _playerLogin = _clientData.UserName;
            _playerAccountId = _clientData.UserId.ToString();
            CountSpaces = _clientData.OwnedSpacesNumber;
            
            if (PlayerPrefs.HasKey(DataExtensions.UserFullStatus + _clientData.UserId))
            {
                _isFullFledgedUser = PlayerPrefs.GetInt(DataExtensions.UserFullStatus + _clientData.UserId) == 1; //1 means is full fledged
            }
            else
            {
                _isFullFledgedUser = false;
            }

            _accountsService.BecameFullFledged += OwnerBecameFullFledged;

            WatchID = WatchSessionData.WatchPlayerId;
            WatchIdOculus = WatchSessionData.WatchPlayerIdOculus;
            IsCustomAvatarWatch = WatchSessionData.IsCustomMetaAvatar;

            RefreshTextAndSigns(true);
        }
        else
        {
            RequestDataServerRPC(NetworkManager.LocalClientId);
        }
    }

    private void OwnerBecameFullFledged()
    {
        if (!IsOwner) return;
        _accountsService.BecameFullFledged -= OwnerBecameFullFledged;
        _isFullFledgedUser = true;
        RefreshTextAndSigns(true);
    }

    private async void RefreshTextAndSigns(bool sendData = false)
    {
        TextID.text = $"{_playerLogin}";
        TextID.color = _isFullFledgedUser ? Color.white : Color.green;

        if (ApplicationData.DebugMode)
        {
            TextID.text = $"{TextID.text}#{_playerID}";
        }

        if (IsOwner) IsSeller = await _accountsService.GetIsThisUserSeller();
        
        if (sendData) TransferDataServerRpc(_playerLogin, _playerAccountId, _isFullFledgedUser, CountSpaces, IsSeller, WatchID, WatchIdOculus, IsCustomAvatarWatch);

        if (_signs.Count > 0) return;

        if (IsSeller) _signs.Add(Instantiate(_isSellerSign, _signsContainer));
        if (CountSpaces == 0) return;
        if (CountSpaces == 1) _signs.Add(Instantiate(_badges[0], _signsContainer));
        else if (CountSpaces > 5) _signs.Add(Instantiate(_badges[3], _signsContainer));
        else _signs.Add(Instantiate(_badges[CountSpaces - 2], _signsContainer));
        if (_signs.Count == 2)
        {
            _signs[0].transform.localPosition += new Vector3(-0.03f, 0, 0);
            _signs[1].transform.localPosition += new Vector3(0.03f, 0, 0);
        }

        foreach (var sign in _signs)
        {
            sign.OnHover += SignOnOnHover;
        }
    }

    private void SignOnOnHover(string text)
    {
        if (_descriptionShowCoroutine != null) StopCoroutine(_descriptionShowCoroutine);
        _descriptionShowCoroutine = StartCoroutine(DescriptionShowCoroutine(text));
    }

    private IEnumerator DescriptionShowCoroutine(string text)
    {
        _signDescription.text = text;
        yield return new WaitForSeconds(2f);
        _signDescription.text = "";
    }

    [ServerRpc(RequireOwnership = false)]
    private void TransferDataServerRpc(string login, string playerAccountId, bool isFullFledgedUser, int countSpaces, bool isSeller, int watchID, int watchIdOculus, bool isCustomMetaAvatar)
    {
        _playerLogin = login;
        _playerAccountId = playerAccountId;
        _isFullFledgedUser = isFullFledgedUser;
        CountSpaces = countSpaces;
        IsSeller = isSeller;
        WatchID = watchID;
        WatchIdOculus = watchIdOculus;
        IsCustomAvatarWatch = isCustomMetaAvatar;
        TransferDataClientRpc(login, _playerAccountId, _isFullFledgedUser, CountSpaces, IsSeller, WatchID, WatchIdOculus, IsCustomAvatarWatch);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDataServerRPC(ulong clientId)
    {
        if (!string.IsNullOrEmpty(_playerLogin))
        {
            TransferDataClientRpc(_playerLogin, _playerAccountId, _isFullFledgedUser, CountSpaces, IsSeller, WatchID, WatchIdOculus, IsCustomAvatarWatch);
        }
    }

    [ClientRpc]
    private void TransferDataClientRpc(string login, string playerAccountId, bool isFullFledgedUser, int countSpaces, bool isSeller, int watchID, int watchIdOculus, bool isCustomMetaAvatar)
    {
        if (!IsOwner)
        {
            CountSpaces = countSpaces;
            IsSeller = isSeller;
            _playerLogin = login;
            _playerAccountId = playerAccountId;
            _isFullFledgedUser = isFullFledgedUser;
            WatchID = watchID;
            WatchIdOculus = watchIdOculus;
            IsCustomAvatarWatch = isCustomMetaAvatar;
            RefreshTextAndSigns(false);
        }

        OnUpdateClientInfo?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        if (_accountsService != null) _accountsService.BecameFullFledged -= OwnerBecameFullFledged;
        if (_signs != null && _signs.Count > 0)
        {
            foreach (var sign in _signs)
            {
                sign.OnHover -= SignOnOnHover;
            }
        }
        _nameInteractable.hoverEntered.RemoveAllListeners();
    }
}
