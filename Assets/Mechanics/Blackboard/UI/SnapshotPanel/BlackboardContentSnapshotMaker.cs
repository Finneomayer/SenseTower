using UnityEngine;
using System;
using Unity.Collections;
using Assets.Scripts.Space;
using Zenject;

namespace Assets.Blackboard
{
    public class BlackboardContentSnapshotMaker : MonoBehaviour
    {
        private const float ThumbnailSizeMultiplier = 0.4f;

        [SerializeField]
        private Camera SnapshotCamera;
        [SerializeField]
        private int ImageWidth;
        [SerializeField]
        private int ImageHeight;
        [SerializeField]
        private string Path;

        public event Action<string> SavedToFile;

        private Texture2D _lastSnapshotTexture;
        private Texture2D _lastSnapshotTextureThumbnail;

        private BlackBoard _blackboard;

        private ISpaceManager _spaceManager;

        public event Action CloseRequested;

        private void OnDestroy()
        {
            DestroyTexture(_lastSnapshotTexture);
            DestroyTexture(_lastSnapshotTextureThumbnail);
        }

        private void Start()
        {
            if (SnapshotCamera is null)
            {
                SnapshotCamera = GameObject.FindWithTag("SnapshotCamera").GetComponent<Camera>();
            }
        }

        public void Init(BlackBoard blackboard, ISpaceManager spaceManager)
        {
            _blackboard = blackboard;
            _spaceManager = spaceManager;
        }

        public void SaveSnapshot()
        {
            MakeSnapshot((int)(ThumbnailSizeMultiplier * ImageWidth), 
                (int)(ThumbnailSizeMultiplier * ImageHeight), OnThumbnailScreenshotReady);
            MakeSnapshot(ImageWidth, ImageHeight, OnFullSizeScreenshotReady);
        }

        private void OnThumbnailScreenshotReady(Texture2D texture)
        {
            DestroyTexture(_lastSnapshotTextureThumbnail);
            _lastSnapshotTextureThumbnail = texture;
            SaveCurrentBlackboardData(_lastSnapshotTextureThumbnail);
        }

        private void OnFullSizeScreenshotReady(Texture2D texture)
        {
            DestroyTexture(_lastSnapshotTexture);
            _lastSnapshotTexture = texture;
            string savedFilePath = SaveSnapshotTextureToFile(_lastSnapshotTexture);

            SavedToFile?.Invoke(savedFilePath);
        }

        private string SaveSnapshotTextureToFile(Texture2D screenshotTexture)
        {
            var appName = Application.productName;
            var fileName = $"{appName} blackboard {GetCurrentDateTimestamp()}";

            return ImageSaver.SaveToFile(Path, fileName, screenshotTexture);
        }

        private void SaveCurrentBlackboardData(Texture2D screenshotTexture)
        {
            if (_spaceManager == null || _spaceManager.CurrentTransitionTarget == null)
            {
                return;
            }

            BlackboardSnapshotData snapshotData = new();
            snapshotData.Data = _blackboard.GetSerializableData();
            snapshotData.BlackboardScreenThumbnailData = screenshotTexture.EncodeToJPG(50);

            snapshotData.SpaceIdData = _spaceManager.CurrentTransitionTarget.Id.ToString();
            snapshotData.BlackboardIdData = _blackboard.BlackboardId.ToString();

            BlackboardDataSaver.SaveToFile(snapshotData, GetCurrentDateTimestamp());
        }

        private void MakeSnapshot(int width, int height, Action<Texture2D> callback)
        {
            RenderTexture screenTexture = new RenderTexture(width, height, 16);
            SnapshotCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            SnapshotCamera.Render();

            UnityEngine.Rendering.AsyncGPUReadback.Request(screenTexture, 0, TextureFormat.ARGB32, (req) =>
            {
                NativeArray<uint> data = req.GetData<uint>();
                Texture2D screenshot = CreateTexture(data, screenTexture.width, screenTexture.height);

                data.Dispose();
                DestroyImmediate(screenTexture);

                callback?.Invoke(screenshot);
            });
        }

        private string GetCurrentDateTimestamp()
        {
            return $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}";
        }

        private Texture2D CreateTexture(NativeArray<uint> data, int width, int height)
        {
            var newTex = new Texture2D
            (
                width,
                height,
                TextureFormat.ARGB32,
                false
            );

            newTex.LoadRawTextureData(data);
            newTex.Apply();
            return newTex;
        }

        private void DestroyTexture(Texture2D texture)
        {
            if (texture != null)
            {
                DestroyImmediate(texture);
            }
        }
    }

    public class AndroidExtensions
    {
        private static AndroidJavaObject _activity;

        private static AndroidJavaObject Activity
        {
            get
            {
                if (_activity != null) return _activity;
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                return _activity;
            }
        }

        private const string MediaStoreImagesMediaClass = "android.provider.MediaStore$Images$Media";

        public static string SaveImageToGallery(Texture2D texture2D, string title, string description)
        {
            using var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass);
            using var cr = Activity.Call<AndroidJavaObject>("getContentResolver");
            var image = Texture2DToAndroidBitmap(texture2D);
            var imageUrl = mediaClass.CallStatic<string>("insertImage", cr, image, title, description);
            return imageUrl;
        }

        private static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D texture2D)
        {
            var encoded = texture2D.EncodeToJPG(100);
            using var bf = new AndroidJavaClass("android.graphics.BitmapFactory");
            return bf.CallStatic<AndroidJavaObject>("decodeByteArray", encoded, 0, encoded.Length);
        }
    }
}
