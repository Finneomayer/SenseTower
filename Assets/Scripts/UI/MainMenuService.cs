using Assets.Scripts.API;
using Assets.Scripts.Client;
using Data;
using UI;
using UnityEditor;
using UnityEngine;
using Zenject;

public class MainMenuService : MenuService
{
    #region Inspector

    [SerializeField] private PreparePanel PreparePanel;
    [SerializeField] private LoginPanel LoginPanel;
    public TMPro.TMP_Text userValue;
    public TMPro.TMP_Text companyTitle;
    public TMPro.TMP_Text productName;
    public TMPro.TMP_Text version;
    public TMPro.TMP_Text bundleVersion;
    public TMPro.TMP_Text contourVersion;
    public ButtonUI[] autenticatedUserButtons;
    public ButtonUI[] guestUserButtons;

    #endregion

    private bool _isLoading = false;
    private bool _isFail = false;
    private IClientData _clientData;

    private Enumenators.PanelType _activePanelType = Enumenators.PanelType.Unknown;

    /// <summary>
    /// Get version works on Android build only. Not Editor!
    /// </summary>
    /// <returns></returns>
    public static int GetVersionCode()
    {
        AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
        string packageName = context.Call<string>("getPackageName");
        AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
        return packageInfo.Get<int>("versionCode");
    }

    [Inject]
    private void Construct(IClientData clientData)
    {
        _clientData = clientData;
    }

    private void Awake()
    {
        companyTitle.text = Application.companyName;
        productName.text = Application.productName;
        version.text = Application.version;
        bundleVersion.text = "-";
#if (UNITY_ANDROID && !UNITY_EDITOR)
        bundleVersion.text = GetVersionCode().ToString();
#endif
#if (UNITY_EDITOR)
        bundleVersion.text = PlayerSettings.Android.bundleVersionCode.ToString();
#endif
        if (string.IsNullOrEmpty(APIService.RefreshTokenUrl))
            OnLoadServerInfo();
        else
            OnSuccessLoadServerInfo();
    }

    public void Init()
    {
        bool visibleResultForAuthUser = !string.IsNullOrEmpty(_clientData.AccessToken);
        bool visibleResultForGuestUser = !_clientData.IsGuest;

        SetButtonsInterractableForAuthUser(visibleResultForAuthUser);
        SetButtonsInterractableForGuestUser(visibleResultForGuestUser);
    }

    private void OnEnable()
    {
        LoginPanel.UserLogInSuccess += OnAuthorization;
        LoginPanel.UserLogOut += OnLogOut;
    }

    private void OnDisable()
    {
        LoginPanel.UserLogInSuccess -= OnAuthorization;
        LoginPanel.UserLogOut -= OnLogOut;
    }

    public void OnFailServerInit()
    {
        _isFail = true;
        PreparePanel.ShowPanel();
        PreparePanel.ShowFailPanel();
    }

    public void OnLoadServerInfo()
    {
        _isLoading = true;
        PreparePanel.ShowPanel();
        PreparePanel.ShowLoadingPanel();
    }

    public void OnSuccessLoadServerInfo()
    {
        _isLoading = false;
        _isFail = false;
        PreparePanel.AllDisable();

        Init();
        if (_activePanelType != Enumenators.PanelType.Unknown)
            OpenPanel(_activePanelType);
    }

    public override void OpenPanel(Enumenators.PanelType panelType, bool hidePreviousPanel = true)
    {
        _activePanelType = panelType;
        if (!_isLoading && !_isFail)
            base.OpenPanel(panelType, hidePreviousPanel);
    }

    private void OnAuthorization(string name)
    {
        SetButtonsInterractableForAuthUser(true);
        SetButtonsInterractableForGuestUser(!_clientData.IsGuest);
    }

    private void OnLogOut()
    {
        SetButtonsInterractableForAuthUser(false);
        SetButtonsInterractableForGuestUser(false);
    }

    private void SetButtonsInterractableForAuthUser(bool interactable)
    {
        foreach (var item in autenticatedUserButtons)
        {
            item.SetButtonInteractable(interactable);
        }
    }

    private void SetButtonsInterractableForGuestUser(bool interactable)
    {
        foreach (var item in guestUserButtons)
        {
            item.SetButtonInteractable(interactable);
        }
    }
}