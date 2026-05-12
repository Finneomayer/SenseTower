using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Infrastructure.AssetManagement
{
    public interface IResources
    {
        UniTask<GameObject> Instantiate(string path, Vector3 at);
        AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single);
        UniTask UnloadScene(SceneInstance scene);
        void Clean();
        UniTask<T> Load<T>(string address) where T : class;
    }
}