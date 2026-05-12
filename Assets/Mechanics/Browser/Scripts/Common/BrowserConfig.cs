using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public enum BrowserType
    {
        Youtube = 1,
        Common = 2
    }

    [CreateAssetMenu(fileName = "BrowserConfig", menuName = "Browser/Config")]
    public class BrowserConfig : ScriptableObject
    {
        [field: SerializeField]
        public string InitialUrl { get; private set; } = string.Empty;

        [field: SerializeField]
        public string LastUrlPlayerPrefsKey { get; private set; } = "WebBrowser_LastUrl";

        [field: SerializeField]
        public BrowserType BrowserType { get; private set; } = BrowserType.Common;
    }
}
