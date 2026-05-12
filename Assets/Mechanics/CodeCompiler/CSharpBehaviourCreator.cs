using System;
using Assets.Scripts.WebUtils;
using CSharpCompiler;
using Cysharp.Threading.Tasks;
using Proyecto26;
using UnityEngine;

namespace Mechanics.CodeCompiler
{
    public class CSharpBehaviourCreator : MonoBehaviour, IBehaviourCreator
    {
        [SerializeField] private string _scriptUrl = string.Empty;
        [SerializeField] private bool _loadInBackground = true;

        private DeferredSynchronizeInvoke _synchronizedInvoke;
        private ScriptBundleLoader _loader;
        private string _codeSource = string.Empty;
        
        public async void Start()
        {
            _synchronizedInvoke = new DeferredSynchronizeInvoke();

            _loader = new ScriptBundleLoader(_synchronizedInvoke);
            _loader.logWriter = new UnityLogTextWriter();
            _loader.createInstance = (t) =>
            {
                if (typeof(Component).IsAssignableFrom(t)) return gameObject.AddComponent(t);
                else return Activator.CreateInstance(t);
            };
            _loader.destroyInstance = (instance) =>
            {
                if (instance is Component) Destroy(instance as Component);
            };
            await GetBehaviourSource();
            CreateBehaviour();
        }

        public async UniTask GetBehaviourSource()
        {
           await DownloadBehaviour(_scriptUrl);
        }

        public void CreateBehaviour()
        {
            _loader.LoadScriptFromSource(_codeSource);
            _synchronizedInvoke.ProcessQueue();
        }
        
        private async UniTask DownloadBehaviour(string uri)
        {
            RequestHelper helpers = new RequestHelper();
            helpers.Uri = uri;
            var result = await WebRequestFunctions.Get(helpers);
            Debug.Log(result.ResponseCode);
            _codeSource = result.ResponseData;
        }
    }
}