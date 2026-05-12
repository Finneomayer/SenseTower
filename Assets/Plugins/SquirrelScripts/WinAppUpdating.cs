using Cysharp.Threading.Tasks;
using Squirrel;
using System.Linq;
using TMPro;
using UnityEngine;

public class WinAppUpdating : MonoBehaviour
{
    [SerializeField]
    public GameObject UpdatePanel;
    [SerializeField]
    public TMP_Text ProgressText;

    public static WinAppUpdating Instance { get; private set; }

    private UpdateManager _updateManager;
    private UpdateInfo _updateInfo;

    private void Awake()
    {
        UpdatePanel.gameObject.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            return;
        }

        _updateManager = new UpdateManager("https://storage.yandexcloud.net/unity-updates/demo");
    }

    public bool IsUpdatableApp()
    {
        return _updateManager != null && _updateManager.IsInstalledApp;
    }

    public async UniTask UpdateApp()
    {
        UpdatePanel.gameObject.SetActive(true);
        await _updateManager.DownloadReleases(_updateInfo.ReleasesToApply, progress: (progress) => ShowProgressInfo("Downloading", progress));
        await _updateManager.ApplyReleases(_updateInfo, (p) =>
        {
            ShowProgressInfo("Applying", p);
        });
        UpdateManager.RestartAppWhenExited().Wait(2000);
        Application.Quit();
    }

    public async UniTask<bool> GetUpdates()
    {
        _updateInfo = await _updateManager.CheckForUpdate();
        return _updateInfo.ReleasesToApply.Any();
    }

    private void ShowProgressInfo(string progressName, int progress)
    {
        ProgressText.text = $"{progressName} - {progress}%";
    }
}