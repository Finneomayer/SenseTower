using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.API;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserPlaceAdminsUI : MonoBehaviour
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

        SetAdminList(localSpace).Forget();
    }

    private void OnAddItemRequested(string userNickName)
    {
        AddUserToAdminList(userNickName).Forget();
    }

    private void OnDeleteItemRequested(Guid userId)
    {
        DeleteUserFromAdminList(userId).Forget();
    }

    private async UniTask AddUserToAdminList(string userNickName)
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

        bool success = await _myPlaceService.AddAdmin(_myPlace.Id, userInfo.Id);

        UserEditingListView.SetInteractable(true);

        if (success)
        {
            UserEditingListView.ResetAddingPanel();
            if (_myPlace != null && _myPlace.AdminIds != null)
            {
                if (!_myPlace.AdminIds.Contains(userInfo.Id.ToString()))
                {
                    _myPlace.AdminIds.Add(userInfo.Id.ToString());
                    UserEditingListView.TryAddUserItem(userInfo);
                }
            }
        }
    }

    private async UniTask DeleteUserFromAdminList(Guid userId)
    {
        if (_myPlace == null || _myPlace.AdminIds == null)
        {
            return;
        }

        int userIndexToRemove = _myPlace.AdminIds.IndexOf(userId.ToString());
        if (userIndexToRemove == -1)
        {
            return;
        }

        UserEditingListView.SetInteractable(false);
      
        List<string> newUserList = _myPlace.AdminIds.ToList();
        newUserList.RemoveAt(userIndexToRemove);

        bool success = await _myPlaceService.UpdateAdminList(_myPlace.Id, newUserList);

        UserEditingListView.SetInteractable(true);

        if (success)
        {
            if (_myPlace != null && _myPlace.AdminIds != null)
            {
                _myPlace.AdminIds.Remove(userId.ToString());
                UserEditingListView.RemoveUserItem(userId);
            }
        }
    }

    private async UniTask SetAdminList(LocalSpace myPlace)
    {
        UserEditingListView.Clear();
        if (myPlace == null || myPlace.AdminIds == null || myPlace.AdminIds.Count == 0)
        {
            return;
        }

        UserLookupInfo[] usersLookupInfo = await _accountsService.GetUsersLookupInfo(myPlace.AdminIds.ToArray());

        if (usersLookupInfo == null)
        {
            return;
        }

        List<UserLookupInfo> userInfos = new();
        foreach (var userId in myPlace.AdminIds)
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
