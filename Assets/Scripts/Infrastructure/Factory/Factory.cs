using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class Factory : IObjectFactory
    {
        private IResources _resources;

        public Factory(IResources resources) 
        {
            _resources = resources;
        }
        
        public async UniTask<GameObject> CreateObject(string itemName)
        {
            return await _resources.Instantiate(itemName, Vector3.zero);
        }

        public async UniTask<T> LoadObject<T>(string itemName) where T : class
        {
            return await _resources.Load<T>(itemName);
        }

        public void Clean() 
        {
            _resources.Clean();
        }
    }
}
