using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Scripts.API;
using Cysharp.Threading.Tasks;
using Data;
using UnityEditor;
using UnityEngine;
using Zenject;

public class AppVersionChecker : MonoBehaviour
{
    [SerializeField] private NotificationPanel _notificationPanel;
    [SerializeField] private UISwitcherService _uiSwitcherService;

    [SerializeField] private LocalizationVariant _wrongAppVersionLocalizationVariant;

    private string _localVersion;
    private int _localBundle;

    private string _discoveryVersion;
    private int _discoveryBundle;

    private IApiService _apiService;

    [Inject]
    private void Init(IApiService apiService)
    {
        _apiService = apiService;
    }

    private void OnEnable()
    {
        _notificationPanel.PanelHidden += OnNotificationPanelHidden;
    }

    private void OnDisable()
    {
        _notificationPanel.ButtonClicked -= OnNotificationPanelButtonClicked;
        _notificationPanel.PanelHidden -= OnNotificationPanelHidden;
    }

    void Start()
    {
        _apiService.ServerInitializedSuccess += DiscoveryInitialized;
        _uiSwitcherService.PanelTypeClick += HideNotification;

        _localVersion = Application.version;
#if (UNITY_ANDROID && !UNITY_EDITOR)
        _localBundle = GetVersionCode();
#endif
#if (UNITY_EDITOR)
        _localBundle = PlayerSettings.Android.bundleVersionCode;
#endif
    }

    private void OnDestroy()
    {
        _uiSwitcherService.PanelTypeClick -= HideNotification;
        _apiService.ServerInitializedSuccess -= DiscoveryInitialized;
    }

    private void HideNotification(Enumenators.PanelType obj)
    {
        _notificationPanel.HidePanel();
    }

    private void DiscoveryInitialized()
    {
        _discoveryVersion = APIService.AppVersion;
        _discoveryBundle = APIService.BundleVersion;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (_discoveryVersion.IndexOf('.') != -1) _discoveryVersion = _discoveryVersion.Remove(_discoveryVersion.IndexOf('.'));
            else _discoveryVersion = _discoveryVersion.Remove(_discoveryVersion.LastIndexOf('-'));

            _discoveryVersion = _discoveryVersion.Replace('-', '_');

            _localVersion = _localVersion.Substring(_localVersion.IndexOf('_') + 1);
            _localVersion = _localVersion.Remove(_localVersion.LastIndexOf('_'));
        }
        else
        {
            _localVersion = _localVersion.Substring(_localVersion.IndexOf('_') + 1);
            _discoveryVersion = _discoveryVersion.Replace('-', '_');
            _discoveryVersion = _discoveryVersion.Replace('.', '_');
        }

        Debug.LogWarning($"AppverDiscovery:  {_discoveryVersion} AppverLocal:  {_localVersion}");

        if (_discoveryVersion != _localVersion)
        {
            var timerDefault = _notificationPanel.timer;
            _notificationPanel.timer = 60f;
            _notificationPanel.ShowPanel();
            _notificationPanel.timer = timerDefault;
            _notificationPanel.notificationText.text = _wrongAppVersionLocalizationVariant.Localize(_discoveryBundle);

            ShowUpdateButton().Forget();
        }
    }
    public static int GetVersionCode()
    {
        AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
        string packageName = context.Call<string>("getPackageName");
        AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
        return packageInfo.Get<int>("versionCode");
    }

    public async UniTask ShowUpdateButton()
    {
        await UniTask.WaitUntil(() => WinAppUpdating.Instance != null);

        if (!WinAppUpdating.Instance.IsUpdatableApp())
        {
            return;
        }

        if (await WinAppUpdating.Instance.GetUpdates())
        {
            _notificationPanel.ShowButton("Обновить");
            _notificationPanel.ButtonClicked += OnNotificationPanelButtonClicked;
        }
    }

    private void OnNotificationPanelHidden()
    {
        _notificationPanel.ButtonClicked -= OnNotificationPanelButtonClicked;
    }

    private void OnNotificationPanelButtonClicked()
    {
        _notificationPanel.HidePanel();
        WinAppUpdating.Instance.UpdateApp().Forget();
    }
}
