using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Infrastructure.AssetManagement
{
    public class AddressableResourcesLocation : IResources
    {
        private string _remotecatalogUrl;
        private readonly Dictionary<string, AsyncOperationHandle> _resources = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

        private Action _initializeComplete;

        public AddressableResourcesLocation(string catalogPath = "", Action initializeComplete = default)
        {
            _initializeComplete = initializeComplete;
            _remotecatalogUrl = catalogPath;

            if (string.IsNullOrEmpty(catalogPath))
                GetRemoteCatalogUrl();
            else
                LoadCatalogAsync(_remotecatalogUrl);
        }

        private void Initialize()
        {
            Addressables.InitializeAsync();
        }

        public async void LoadCatalogAsync(string catalogPath)
        {
            var task = Addressables.LoadContentCatalogAsync(catalogPath).Task;
            
            await task;
            Initialize();
            _initializeComplete?.Invoke();
        }

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            var operation = Addressables.LoadSceneAsync(sceneName, loadMode, false);
            
            return operation;
        }

        public UniTask UnloadScene(SceneInstance scene)
        {
            return Addressables.UnloadSceneAsync(scene).ToUniTask();
        }

        public async UniTask<T> Load<T>(string address) where T : class
        {
            if (_resources.TryGetValue(address, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;
           
            return await LoadResources(Addressables.LoadAssetAsync<T>(address),address);
        }

        public UniTask<GameObject> Instantiate(string address, Vector3 at)
        {
            return Addressables.InstantiateAsync(address, at, Quaternion.identity).Task.AsUniTask();
        }

        public void Clean()
        {
            foreach (List<AsyncOperationHandle> resourceHandles in _handles.Values)
                foreach (AsyncOperationHandle handle in resourceHandles)
                    Addressables.Release(handle);
            
            _resources.Clear();
            _handles.Clear();
        }

        private async UniTask<T> LoadResources<T>(AsyncOperationHandle<T> handle, string key) where T : class
        {
            handle.Completed += completeHandle =>
            {
                _resources[key] = completeHandle;
            };

            AddResource<T>(key, handle);

            return await handle.Task.AsUniTask();
        }

        private void AddResource<T>(string key, AsyncOperationHandle handle) where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }

        private void GetRemoteCatalogUrl()
        {
            LoadCatalogAsync(ResourcesLocation.GetRemoteObjectCatalogPath());
        }

        private void OnCatalogDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            var result = e.Result;

            _remotecatalogUrl = Encoding.ASCII.GetString(result);
            LoadCatalogAsync(_remotecatalogUrl);
        }

    }
}