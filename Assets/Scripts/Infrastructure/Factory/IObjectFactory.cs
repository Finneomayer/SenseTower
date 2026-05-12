using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Factory
{
    public interface IObjectFactory
    {
        UniTask<GameObject> CreateObject(string itemName);
        UniTask<T> LoadObject<T>(string itemName) where T : class;
    }
}
