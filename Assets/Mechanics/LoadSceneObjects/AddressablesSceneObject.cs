using System;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Mechanics.LoadSceneObjects.Interfaces;
using Mechanics.Tips.TipsInteraction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.LoadSceneObjects
{
    public class AddressablesSceneObject : MonoBehaviour, ILoadSceneObject
    {
        public event Action<bool> OnCreate;
        protected Factory _factory;
        protected bool _isObjectInit = false;
        protected GameObject spawnObject;

        private GameObject _spawnObject;

        private bool _isFactoryInit = false;

        public void Init(string path)
        {
            _factory = new Factory(new AddressableResourcesLocation(path, () => OnInit()));
        }

        public async void Create(string key, Vector3 position = default, Vector3 rotation = default,
            Vector3 scale = default)
        {
            if (_factory == null) return;
            try
            {
                await UniTask.WaitUntil(() => _isFactoryInit);
                _spawnObject = await _factory.LoadObject<GameObject>(key);

                if (_spawnObject == null) return;
                spawnObject = Instantiate(_spawnObject, gameObject.transform);
                spawnObject.transform.localScale = scale;
                spawnObject.transform.position = position;
                spawnObject.transform.rotation = Quaternion.Euler(rotation);
                _isObjectInit = true;
                OnCreate?.Invoke(true);
            }
            catch
            {
                OnCreate?.Invoke(false);
            }
        }

        public async void SetTipsContent(string tipsContent)
        {
            await UniTask.WaitUntil(() => spawnObject != null);
            if (spawnObject == null)
                return;

            if (string.IsNullOrEmpty(tipsContent))
                return;

            XrInteractionTipsObject tips = spawnObject.AddComponent<XrInteractionTipsObject>();
            tips.SetTipsText(tipsContent);
        }

        private void OnInit()
        {
            _isFactoryInit = true;
        }

        private void OnDestroy()
        {
            _factory.Clean();
        }
    }
}