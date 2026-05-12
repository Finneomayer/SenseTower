using System;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Infrastructure.Factory
{
    public interface ISceneFactory
    {
        AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single);
        UniTask ActiveSceneAsync();
        bool IsThisSceneLoad(string sceneName);
        void Cleanup();
    }
}