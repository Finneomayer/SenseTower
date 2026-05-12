using UnityEngine;
using System.IO;

namespace Assets.Blackboard
{
    public static class ImageSaver
    {
        public static string SaveToFile(string path, string filename, Texture2D texture)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            SaveScreenshotWin(path, filename, texture);
            return $"{path}{Path.DirectorySeparatorChar}{filename}";
#elif UNITY_ANDROID
            AndroidExtensions.SaveImageToGallery(texture, filename, "Blackboard content");
            return $"Pictures{Path.DirectorySeparatorChar}{filename}";
#else
            return null;
#endif
        }

        private static void SaveScreenshotWin(string path, string filename, Texture2D texture)
        {
            byte[] byteArray = texture.EncodeToJPG(100);

            string fullDirectoryPath = $"{Application.dataPath}{Path.DirectorySeparatorChar}{path}";
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }
            File.WriteAllBytes($"{fullDirectoryPath}{Path.DirectorySeparatorChar}{filename}.jpg", byteArray);
        }
    }
}
