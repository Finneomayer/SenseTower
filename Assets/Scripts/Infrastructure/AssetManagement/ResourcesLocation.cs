using UnityEditor;
using UnityEngine;

namespace Infrastructure.AssetManagement
{
    public class ResourcesLocation
    {
        private static string _basePath = "";
        private static readonly string _catalogName = "catalog_Sense.json";

        public static string GetRemoteObjectCatalogPath() 
        {
            string targetPlatform = GetTargetPlatformString();
            return $"{_basePath}/{targetPlatform}/{_catalogName}";
        }

        public static string GetRemoteObjectCatalogPath(string basePath)
        {
            string targetPlatform = GetTargetPlatformString();
            return $"{basePath}/{targetPlatform}/{_catalogName}";
        }

        public static void SetRemoteBasePath(string basePath)
        {
            _basePath = basePath;
        }

        public static string GetRemoteSceneObjectCatalogPath(string path)
        {
            if (!path.Contains(_catalogName)) path += $"/{_catalogName}";

            int index = path.LastIndexOf("/");

            string targetPlatform = GetTargetPlatformString();

            return path.Insert(index + 1, $"{targetPlatform}/");
        }

        public static string GetRemoteScenePath(string folderName, string catalogName) 
        {
            string targetPlatform = GetTargetPlatformString();
            return $"{_basePath}/{folderName}/{targetPlatform}/{catalogName}";
        }

        private static string GetTargetPlatformString()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return "StandaloneWindows64";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_STANDALONE_LINUX
            return "StandaloneLinux64";
#else
            return "";
#endif
        }
    }
}