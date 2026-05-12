using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserPlaceBlockListUI : MonoBehaviour
{
    [SerializeField] UserEditingListView UserEditingListView;

    private LocalSpace _myPlace;
    private IMyPlaceService _myPlaceService;
    private IAccountsService _accountsService;

    private void OnEnable()
    {
        UserEditingListView.AddUserRequested += OnAddItemRequested;
        UserEditingListView.DeleteUserRequested += OnDeleteItemRequested;
    }

    private void OnDisable()
    {
        UserEditingListView.AddUserRequested -= OnAddItemRequested;
        UserEditingListView.DeleteUserRequested -= OnDeleteItemRequested;
    }

    public void Init(LocalSpace localSpace, KeyboardScript keyboard, IMyPlaceService myPlaceService, IAccountsService accountsService)
    {
        _myPlace = localSpace;
        _myPlaceService = myPlaceService;
        _accountsService = accountsService;

        UserEditingListView.Init(keyboard);

        SetBlockList(localSpace).Forget();
    }

    private void OnAddItemRequested(string userNickName)
    {
        AddUserToBlockList(userNickName).Forget();
    }

    private void OnDeleteItemRequested(Guid userId)
    {
        DeleteUserFromBlockList(userId).Forget();
    }

    private async UniTask AddUserToBlockList(string userNickName)
    {
        if (_myPlace == null)
        {
            return;
        }

        UserEditingListView.SetInteractable(false);

        UserLookupInfo[] usersLookupInfo = await _accountsService.GetUsersLookupInfo();
        if (usersLookupInfo == null)
        {
            UserEditingListView.SetInteractable(true);
            return;
        }

        UserLookupInfo userInfo = usersLookupInfo.FirstOrDefault(u => u.Name.Equals(userNickName, StringComparison.OrdinalIgnoreCase));

        if (userInfo == null)
        {
            Debug.LogWarning($"User {userNickName} does not exist");
            UserEditingListView.SetInteractable(true);
            return;
        }

        if (_myPlace.SpaceOwner != null && userInfo.Id == _myPlace.SpaceOwner.UserId)
        {
            Debug.LogWarning($"User {userNickName} is owner");
            UserEditingListView.SetInteractable(true);
            return;
        }

        bool success = await _myPlaceService.AddToBlockList(_myPlace.Id, userInfo.Id);

        UserEditingListView.SetInteractable(true);

        if (success)
        {
            UserEditingListView.ResetAddingPanel();
            if (_myPlace != null && _myPlace.BlockList != null)
            {
                if (!_myPlace.BlockList.Contains(userInfo.Id.ToString()))
                {
                    _myPlace.BlockList.Add(userInfo.Id.ToString());
                    UserEditingListView.TryAddUserItem(userInfo);
                }
            }
        }
    }

    private async UniTask DeleteUserFromBlockList(Guid userId)
    {
        if (_myPlace == null || _myPlace.BlockList == null)
        {
            return;
        }

        int userIndexToRemove = _myPlace.BlockList.IndexOf(userId.ToString());
        if (userIndexToRemove == -1)
        {
            return;
        }

        UserEditingListView.SetInteractable(false);

        bool success = await _myPlaceService.DeleteFromBlockList(_myPlace.Id, userId);

        UserEditingListView.SetInteractable(true);

        if (success)
        {
            if (_myPlace != null && _myPlace.BlockList != null)
            {
                _myPlace.BlockList.Remove(userId.ToString());
                UserEditingListView.RemoveUserItem(userId);
            }
        }
    }

    private async UniTask SetBlockList(LocalSpace myPlace)
    {
        UserEditingListView.Clear();
        if (myPlace == null || myPlace.BlockList == null || myPlace.BlockList.Count == 0)
        {
            return;
        }

        UserLookupInfo[] usersLookupInfo = await _accountsService.GetUsersLookupInfo(myPlace.BlockList.ToArray());

        if (usersLookupInfo == null)
        {
            return;
        }

        List<UserLookupInfo> userInfos = new();
        foreach (var userId in myPlace.BlockList)
        {
            UserLookupInfo userInfo = usersLookupInfo.FirstOrDefault((user) => user.Id == Guid.Parse(userId));
            if (userInfo != null)
            {
                userInfos.Add(userInfo);
            }
        }

        foreach (var userInfo in userInfos)
        {
            UserEditingListView.TryAddUserItem(userInfo);
        }
    }
}
