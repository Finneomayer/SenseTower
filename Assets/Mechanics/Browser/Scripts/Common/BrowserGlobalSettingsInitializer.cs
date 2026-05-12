using System;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Browser
{
    public class BrowserGlobalSettingsInitializer : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_SERVER
            gameObject.SetActive(false);
            return;
#endif            
            try
            {
                Web.SetUserAgent("Mozilla/5.0 (Macintosh; Intel Mac OS X 13.4; rv:102.0) Gecko/20100101 Firefox/102.0");
                Web.SetAutoplayEnabled(true);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }
}
