using System;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Infrastructure.Factory
{
    public class SceneFactory : ISceneFactory
    {
        private IResources _resources;
        private static SceneInstance currentScene;

        public SceneFactory(IResources resources)
        {
            _resources = resources;
        }

        public async UniTask ActiveSceneAsync()
        {
            if(currentScene.Scene.IsValid())
                await currentScene.ActivateAsync();
        }

        public bool IsThisSceneLoad(string sceneName)
        {
            return currentScene.Scene.name == sceneName && currentScene.Scene.IsValid();
        }

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            var operation =  _resources.LoadSceneAsync(sceneName, loadMode);
            SetCurrentScene(operation);
            
            return operation;
        }

        public void Cleanup()
        {
            if (currentScene.Scene.IsValid() && currentScene.Scene.isLoaded)
                _resources.UnloadScene(currentScene);
            
            _resources.Clean();
        }

        private async void SetCurrentScene(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
        {
            await asyncOperationHandle;
            currentScene = asyncOperationHandle.Result;
        }
    }
}