using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Assets.Mechanics.MetaAvatars.Input.Scripts
{
    public class XRInitializer : MonoBehaviour
    {
        private static XRInitializer _instance;

        private void Awake()
        {
#if UNITY_SERVER
            gameObject.SetActive(false);
            return;
#endif

            if (_instance == null)
            {
                _instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(this);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
#if UNITY_SERVER
            return;
#endif

            if (_instance == this && XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void AttemptInitializeXRSDKOnLoad()
        {
#if UNITY_SERVER
            return;
#endif
            XRGeneralSettings instance = XRGeneralSettings.Instance;
            if (instance == null || instance.InitManagerOnStart)
                return;

            InitXRSDK();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        internal static void AttemptStartXRSDKOnBeforeSplashScreen()
        {
#if UNITY_SERVER
            return;
#endif
            XRGeneralSettings instance = XRGeneralSettings.Instance;
            if (instance == null || instance.InitManagerOnStart)
                return;

            StartXRSDK();
        }

        private static void InitXRSDK()
        {
            if (XRGeneralSettings.Instance == null)
                return;

            Debug.Log("Initializing XR...");
            XRGeneralSettings.Instance.Manager.automaticLoading = false;
            XRGeneralSettings.Instance.Manager.automaticRunning = false;
            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        }

        private static void StartXRSDK()
        {
            if (XRGeneralSettings.Instance.Manager != null && XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }
    }
}
