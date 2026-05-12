using Assets.Mechanics.Keyboard.Scripts;
using Assets.Scripts.Space;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserEditingListView : MonoBehaviour
{
    [SerializeField] private CanvasGroup Content;
    [SerializeField] private Transform ItemsContent;
    [SerializeField] private Transform NoItemsPanel;
    [SerializeField] private UserEditViewItem ItemPrefab;
    [SerializeField] private InputWithButtonPanel AddingPanel;

    private UserAddDeleteEventMediator _userAddDeleteEventMediator = new();
    private List<UserEditViewItem> _items = new();

    public Action<string> AddUserRequested;
    public Action<Guid> DeleteUserRequested;

    private void OnEnable()
    {
        AddingPanel.SubmitRequested += OnAddItemRequested;
        _userAddDeleteEventMediator.DeleteRequested += OnDeleteItemRequested;
    }

    private void OnDisable()
    {
        AddingPanel.SubmitRequested -= OnAddItemRequested;
        _userAddDeleteEventMediator.DeleteRequested -= OnDeleteItemRequested;
    }

    public void Init(KeyboardScript keyboard)
    {
        AddingPanel.Init(keyboard);
    }

    public void Clear()
    {
        foreach (var visitorItem in _items)
        {
            if (visitorItem != null)
            {
                Destroy(visitorItem.gameObject);
            }
        }

        _items.Clear();
        RefreshAdditionalInfo();
    }

    public void ResetAddingPanel()
    {
        AddingPanel.Clear();
        AddingPanel.CloseKeyboard();
    }

    public void SetInteractable(bool interactable)
    {
        Content.alpha = interactable ? 1 : 0.05f;
        Content.interactable = interactable;
    }

    private void OnAddItemRequested(string userNickName)
    {
        AddUserRequested?.Invoke(userNickName);
    }

    private void OnDeleteItemRequested(Guid userId)
    {
        DeleteUserRequested?.Invoke(userId);
    }

    public bool TryAddUserItem(UserLookupInfo userInfo)
    {
        if (userInfo == null)
        {
            return false;
        }

        UserEditViewItem existingItem =
            _items.FirstOrDefault(item => item.UserLookupInfo != null && item.UserLookupInfo.Id == userInfo.Id);
        if (existingItem != null)
        {
            return false;
        }

        AddUserItem(userInfo);

        return true;
    }

    public void RemoveUserItem(Guid userId)
    {
        foreach (var item in _items)
        {
            if (item.UserLookupInfo.Id == userId)
            {
                _items.Remove(item);
                Destroy(item.gameObject);
                break;
            }
        }
        RefreshAdditionalInfo();
    }

    private void AddUserItem(UserLookupInfo adminInfo)
    {
        UserEditViewItem newItem = Instantiate(ItemPrefab, ItemsContent);

        newItem.Init(adminInfo, _userAddDeleteEventMediator);
        _items.Add(newItem);
        RefreshAdditionalInfo();
    }

    private void RefreshAdditionalInfo()
    {
        NoItemsPanel.gameObject.SetActive(_items.Count == 0);
    }
}
