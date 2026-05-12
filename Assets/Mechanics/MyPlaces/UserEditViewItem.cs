using Assets.Scripts.Space;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserEditViewItem : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    [SerializeField] private Button DeleteButton;

    public UserLookupInfo UserLookupInfo => _userLookupInfo;

    private UserLookupInfo _userLookupInfo;
    private UserAddDeleteEventMediator _userAddDeleteEventMediator;

    public event Action<UserLookupInfo> DeleteRequested;

    private void OnEnable()
    {
        DeleteButton.onClick.AddListener(OnDeleteButtonClick);
    }

    private void OnDisable()
    {
        DeleteButton.onClick.RemoveListener(OnDeleteButtonClick);
    }

    public void Init(UserLookupInfo adminInfo, UserAddDeleteEventMediator userAddDeleteEventMediator)
    {
        _userLookupInfo = adminInfo;
        Name.text = adminInfo.Name;
        _userAddDeleteEventMediator = userAddDeleteEventMediator;
    }

    private void OnDeleteButtonClick()
    {
        if (_userAddDeleteEventMediator != null && _userLookupInfo != null)
        {
            _userAddDeleteEventMediator.RaiseDeleteRequested(_userLookupInfo.Id);
        }
    }

}
