using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets.Mechanics.FriendsList;
using Assets.Mechanics.FriendsList.Models;
using Assets.Mechanics.FriendsList.UI;
using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Scripts.Client;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Mechanics.FriendsList;
using Mechanics.SignalBusModels;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using Zenject;

namespace UI.Panels
{
    public class FriendsPanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private FriendItem _friendItemPrefab;

        [SerializeField] private ViewPanel _emptyPanel;
        [SerializeField] private ViewPanel _contentPanel;

        [SerializeField] private GameObject _contentParentGameObject;

        #endregion

        private IFriendsService _friendsService;
        private IClientData _clientData;
        private SignalBus _signalBus;

        private GetFriendDTO[] _friends;
        private List<GetFriendDTO> _friendsRequest = new();
        private List<FriendItem> _friendItemsInstance = new();
        private Dictionary<int, Sprite> _avatarAssetIdIconMap = new();
        private Coroutine _friendListRefresingCoroutine;

        [Inject]
        private void Construct(SignalBus signalBus, IFriendsService friendsService, IClientData clientData)
        {
            _signalBus = signalBus;
            _clientData = clientData;
            _friendsService = friendsService;
            _signalBus.Subscribe<AddToFriendListRequestSignal>(OnAddToFriendListRequestSignalRaise);
            _signalBus.Subscribe<RemoveFromFriendListRequestSignal>(OnRemoveToFriendListRequestSignalRaise);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<AddToFriendListRequestSignal>(OnAddToFriendListRequestSignalRaise);
            _signalBus.Unsubscribe<RemoveFromFriendListRequestSignal>(OnRemoveToFriendListRequestSignalRaise);

            StopFriendsRefreshing();
        }

        private void Awake()
        {
            Sprite[] loadedIcons = Resources.LoadAll<Sprite>("PresetAvatarsIcons");
            foreach (var item in loadedIcons)
            {
                if (!int.TryParse(Path.GetFileNameWithoutExtension(item.name), out int assetId))
                {
                    continue;
                }
                _avatarAssetIdIconMap[assetId] = item;
            }
        }

        public  override void ShowPanel()
        {
            base.ShowPanel();
            StartFriendsRefreshing();
        }

        public override void HidePanel()
        {
            base.HidePanel();
            StopFriendsRefreshing();
            DeleteAllFriendsInstance();
        }

        private async void FillFriendList()
        {
            _friends = await _friendsService.GetFriendsList(_clientData.UserId.ToString());
            RefreshFriendsListInstances();
        }

        private void RefreshFriendsListInstances()
        {
            DeleteAllFriendsInstance();
            ShowLogic();
        }

        private void ShowLogic()
        {
            if (_friends.Length > 0)
            {
                for (int i = 0; i < _friends.Length; i++)
                {
                    GetFriendDTO tempFriend = _friends[i];
                    InitFriend(tempFriend, false);
                }
            }

            if (_friendsRequest.Count > 0)
            {
                foreach (GetFriendDTO friendRequestDto in _friendsRequest)
                {
                    InitFriend(friendRequestDto, true);
                }
            }

            if (_friendItemsInstance.Count > 0)
            {
                _emptyPanel.HidePanel();
                _contentPanel.ShowPanel();
            }
            else
            {
                _emptyPanel.ShowPanel();
                _contentPanel.ShowPanel();
            }
        }

        private void OnAddToFriendListRequestSignalRaise(AddToFriendListRequestSignal addToFriendListRequestSignal)
        {
            if (addToFriendListRequestSignal == null
                || addToFriendListRequestSignal.UserId == null
                || addToFriendListRequestSignal.UserName == null)
            {
                return;
            }

            bool exist = _friendsRequest.Exists(element => element.UserId.ToString().Equals(addToFriendListRequestSignal.UserId));
            if (exist)
                return;

            if (_friends != null && _friends.Length > 0)
            {
                GetFriendDTO friendInFriendList =
                    _friends.FirstOrDefault(elem => elem.UserId.ToString().Equals(addToFriendListRequestSignal.UserId));

                if (friendInFriendList != null)
                    return;
            }

            GetFriendDTO friend = new();
            friend.Online = false;
            friend.AvatarNumber = addToFriendListRequestSignal.AvatarId;
            friend.UserId = Guid.Parse(addToFriendListRequestSignal.UserId);
            friend.UserName = addToFriendListRequestSignal.UserName;
            friend.ConfirmationSended = false;
            _friendsRequest.Add(friend);

            InitFriend(friend, true);

            _emptyPanel.HidePanel();
            _contentPanel.ShowPanel();
        }

        private void OnRemoveToFriendListRequestSignalRaise(
            RemoveFromFriendListRequestSignal removeToFriendListRequestSignal)
        {
            if (!removeToFriendListRequestSignal.IsRequest)
            {
                TryDeleteFriend(removeToFriendListRequestSignal.UserId);
            }
            else
            {
                for (int i = _friendsRequest.Count - 1; i >= 0; i--)
                {
                    if (_friendsRequest[i].UserId == removeToFriendListRequestSignal.UserId)
                    {
                        _friendsRequest.RemoveAt(i);
                    }
                }
                FillFriendList();
            }
        }

        private void InitFriend(GetFriendDTO friends, bool isRequest)
        {
            FriendItem friendItem = Instantiate(_friendItemPrefab, _contentParentGameObject.transform, false);

            if (isRequest)
            {
                friendItem.FriendRequestConfirmationSended += OnFriendRequestConfirmationSended;
                friendItem.MakeInstanceAsRequest(friends.ConfirmationSended);
            }

            friendItem.InitData(friends.UserName, friends.Online, friends.SpaceName, friends.UserId.ToString());

            int avatarNumber = AvatarSessionData.DefaultAvatarAssetId;
            if (friends.AvatarNumber.HasValue)
            {
                avatarNumber = friends.AvatarNumber.Value;
            }
            friendItem.SetAvatarImage(_avatarAssetIdIconMap[avatarNumber]);
            _friendItemsInstance.Add(friendItem);
        }

        private void DeleteAllFriendsInstance()
        {
            if (_friendItemsInstance == null || _friendItemsInstance.Count == 0)
                return;

            foreach (FriendItem friendItem in _friendItemsInstance)
            {
                friendItem.FriendRequestConfirmationSended -= OnFriendRequestConfirmationSended;
                Destroy(friendItem.gameObject);
            }

            _friendItemsInstance.Clear();
        }

        private async void TryDeleteFriend(Guid userId)
        {
            await UniTask.Delay(600);
            _friends = await _friendsService.GetFriendsList(_clientData.UserId.ToString());
            bool userExistInFriendList = false;
            
            if (_friends != null && _friends.Length > 0)
            {
                foreach (var t in _friends)
                {
                    if (t.UserId == userId)
                    {
                        userExistInFriendList = true;
                        break;
                    }
                }
            }
            
            if(userExistInFriendList)
                await _friendsService.DeleteFriend(_clientData.UserId.ToString(), userId.ToString());

            FillFriendList();
        }

        private void StartFriendsRefreshing()
        {
            if (_friendListRefresingCoroutine != null)
            {
                StopCoroutine(_friendListRefresingCoroutine);
            }
            _friendListRefresingCoroutine = StartCoroutine(FriendListRefresingCoroutine());
        }

        private void StopFriendsRefreshing()
        {
            if (_friendListRefresingCoroutine != null)
            {
                StopCoroutine(_friendListRefresingCoroutine);
                _friendListRefresingCoroutine = null;
            }
        }

        private IEnumerator FriendListRefresingCoroutine()
        {
            WaitForSeconds nextRefreshWait = new WaitForSeconds(1);
            while (true)
            {
                bool needRefresh = false;
                Task<GetFriendDTO[]> getFriendsTask = _friendsService.GetFriendsList(_clientData.UserId.ToString()).AsTask();
                yield return new WaitUntil(() => getFriendsTask.IsCompleted);

                if (getFriendsTask.Result != null)
                {
                    if (_friends == null || _friends.Length != getFriendsTask.Result.Length 
                        || _friendItemsInstance.Count != _friends.Length + _friendsRequest.Count)
                    {
                        needRefresh = true;
                    }
                    else
                    {
                        for (int i = 0; i < _friends.Length; i++)
                        {
                            if (!_friends[i].IsEqual(getFriendsTask.Result[i]))
                            {
                                needRefresh = true;
                                break;
                            }
                        }
                    }
                }

                if (needRefresh)
                {
                    _friends = getFriendsTask.Result;
                    RefreshFriendsListInstances();
                }

                yield return nextRefreshWait;
            }
        }

        private void OnFriendRequestConfirmationSended(string userGuid)
        {
            foreach (var item in _friendsRequest)
            {
                if (item.UserId == Guid.Parse(userGuid))
                {
                    item.ConfirmationSended = true;
                }
            }
        }
    }
}