using Assets.Localization;
using Assets.Scripts.Space;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPlaceVisitorItem : MonoBehaviour
{
    [SerializeField] private TMP_Text VisitorName;
    [SerializeField] private Button MuteButton;
    [SerializeField] private Button KickButton;
    [SerializeField] private TMP_Text MuteText;
    [SerializeField] private TMP_Text UserDescriptionText;

    [Header("Localization")]
    [SerializeField] private LocalizationVariant OwnerLocalizationVariant;
    [SerializeField] private LocalizationVariant MeLocalizationVariant;

    public UserInSpaceInfoDto Visitor => _visitor;

    private UserInSpaceInfoDto _visitor;
    public event Action<UserPlaceVisitorItem> MuteButtonClicked;
    public event Action<UserPlaceVisitorItem> KickButtonClicked;

    private void Awake()
    {
        MuteButton.gameObject.SetActive(false);
        KickButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        MuteButton.onClick.AddListener(OnMuteButtonClick);
        KickButton.onClick.AddListener(OnKickButtonClick);
    }

    private void OnDisable()
    {
        MuteButton.onClick.RemoveListener(OnMuteButtonClick);
        KickButton.onClick.RemoveListener(OnKickButtonClick);
    }

    public void Init(UserInSpaceInfoDto visitor, UserInSpaceInfoDto localUserVisitor, 
        UserInSpaceInfoDto ownerUserVisitor, bool allowManageUser)
    {
        SetVisitor(visitor);
        RefreshUI(visitor, localUserVisitor, ownerUserVisitor, allowManageUser);
    }

    public void ChangeMuteState(bool muteValue)
    {
        MuteText.enabled = muteValue;
    }

    private void SetVisitor(UserInSpaceInfoDto visitor)
    {
        _visitor = visitor;
        VisitorName.text = _visitor.UserName;
    }

    private void RefreshUI(UserInSpaceInfoDto visitor, UserInSpaceInfoDto localUserVisitor,
        UserInSpaceInfoDto ownerUserVisitor, bool allowManageUser)
    {
        bool activateButtons = allowManageUser && visitor != ownerUserVisitor && visitor != localUserVisitor;
        MuteButton.gameObject.SetActive(activateButtons);
        KickButton.gameObject.SetActive(activateButtons);

        if (visitor == localUserVisitor)
        {
            UserDescriptionText.text = MeLocalizationVariant.Localize().ToLower();
            UserDescriptionText.gameObject.SetActive(true);
        }
        else if (visitor == ownerUserVisitor)
        {
            UserDescriptionText.text = OwnerLocalizationVariant.Localize().ToLower();
            UserDescriptionText.gameObject.SetActive(true);
        }
        else
        {
            UserDescriptionText.gameObject.SetActive(false);
        }
    }

    private void OnMuteButtonClick()
    {
        MuteButtonClicked?.Invoke(this);
    }

    private void OnKickButtonClick()
    {
        KickButtonClicked?.Invoke(this);
    }
}