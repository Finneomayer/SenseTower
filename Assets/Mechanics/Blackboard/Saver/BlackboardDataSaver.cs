using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Android;
using Zenject;
using Assets.Scripts.Client;
using Assets.Scripts.Space;

namespace Assets.Blackboard
{
    public class BlackboardDataSaver
    {
        private const string SubDirectory = "Blackboards";

        public bool LoadingInProgress { get; private set; }

        private bool _isInterruptionRequested;

        public event Action LoadingFinished;
        public event Action<string, BlackboardSnapshotData> FileLoaded;

        public event Action CloseRequested;

        public static void SaveToFile(BlackboardSnapshotData snapshotData, string filename)
        {
            PerformWithPermissionsCheck(() => WriteToFile(snapshotData, SubDirectory, filename));
        }

        public static void RemoveFile(string fullFilePath)
        {
            PerformWithPermissionsCheck(() => DeleteFile(fullFilePath));
        }

        public void LoadFiles(Guid spaceId, Guid blackboardId, Action<string, BlackboardSnapshotData> fileLoadedCallback)
        {
            PerformWithPermissionsCheck(() => ReadFilesAsync(spaceId, blackboardId, fileLoadedCallback).Forget());
        }

        public void InterruptLoading()
        {
            if (LoadingInProgress)
            {
                _isInterruptionRequested = true;
            }
        }

        private async UniTask ReadFilesAsync(Guid spaceId, Guid blackboardId, Action<string, BlackboardSnapshotData> fileLoadedCallback)
        {
            if (LoadingInProgress)
            {
                return;
            }
            LoadingInProgress = true;

            try
            {
                string fullDirectoryPath = $"{GetSharedFolderDirectory()}{Path.DirectorySeparatorChar}{SubDirectory}";
                if (!Directory.Exists(fullDirectoryPath))
                {
                    LoadingInProgress = false;
                    return;
                }

                DirectoryInfo directory = new DirectoryInfo(fullDirectoryPath);

                FileInfo[] fileInfos = directory.GetFiles().OrderByDescending((f) => f.CreationTime).ToArray();
                fileInfos = fileInfos.OrderByDescending((f) => f.CreationTime).ToArray();

                foreach (var fileInfo in fileInfos)
                {
                    using (StreamReader file = new StreamReader(fileInfo.FullName))
                    {
                        string jsonData = await file.ReadToEndAsync();

                        if (_isInterruptionRequested)
                        {
                            break;
                        }

                        try
                        {
                            BlackboardSnapshotData data = JsonUtility.FromJson<BlackboardSnapshotData>(jsonData);
                            if (data.SpaceId != null && data.SpaceId.Value == spaceId
                                && (data.BlackboardId == null || data.BlackboardId == blackboardId))
                            {
                                fileLoadedCallback?.Invoke(fileInfo.FullName, data);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"BlackboardDataSaver. ReadFilesAsync. {e.Message}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"BlackboardDataSaver. ReadFilesAsync. {e.Message}");
            }
            finally
            {
                _isInterruptionRequested = false;
                LoadingInProgress = false;
            }
        }

        private static void WriteToFile(BlackboardSnapshotData snapshotData, string path, string filename)
        {
            string jsonData = JsonUtility.ToJson(snapshotData);

            try
            {
                string fullDirectoryPath = $"{GetSharedFolderDirectory()}{Path.DirectorySeparatorChar}{path}";
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }

                string fullFilename = $"{fullDirectoryPath}{Path.DirectorySeparatorChar}{filename}.json";
                using (StreamWriter file = File.CreateText(fullFilename))
                {
                    file.Write(jsonData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"BlackboardDataSaver. WriteToFile. {e.Message}");
            }
        }

        private static void DeleteFile(string fullFilePath)
        {
            try
            {
                File.Delete(fullFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"BlackboardDataSaver. DeleteFile. {e.Message}");
            }
        }

        private static string GetSharedFolderDirectory()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return Application.persistentDataPath;
            }

            var jc = new AndroidJavaClass("android.os.Environment");
            var path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory",
                jc.GetStatic<string>("DIRECTORY_DOWNLOADS"))
                .Call<string>("getAbsolutePath");
            return path;
        }

        private static void PerformWithPermissionsCheck(Action callback)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                callback.Invoke();
                return;
            }

            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                callback.Invoke();
            }
            else
            {
                PermissionCallbacks permissionCallbacks = new();
                permissionCallbacks.PermissionGranted += (s) => callback.Invoke();
                permissionCallbacks.PermissionDenied += (s) => callback.Invoke();
                permissionCallbacks.PermissionDeniedAndDontAskAgain += (s) => callback.Invoke();

                Permission.RequestUserPermission(Permission.ExternalStorageWrite, permissionCallbacks);
            }
        }
    }
}
