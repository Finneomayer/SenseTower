using System;
using Assets.Mechanics.Tips;
using Assets.Scripts.Shared;
using Assets.Scripts.Space;
using UnityEngine;
using Zenject;

namespace Mechanics.Tips.TipsInteraction
{
    public class TipsObject : MonoBehaviour
    {
        [SerializeField] private string _tipsId;
        [SerializeField] private string _tipsText;

        private ITipsSceneRepository _tipsSceneRepository;
        protected ISpaceModeData _spaceModeData;
        private ITipsSceneContext _tipsSceneContext;

        [Inject]
        private void Construct(ITipsSceneRepository tipsSceneRepository, ITipsSceneContext tipsSceneContext,
            ISpaceModeData spaceModeData)
        {
            _tipsSceneContext = tipsSceneContext;
            _spaceModeData = spaceModeData;
            _tipsSceneRepository = tipsSceneRepository;

            if (!string.IsNullOrEmpty(_tipsId))
                tipsSceneContext.RegisterTipsId(_tipsId);
        }

        private void Awake()
        {
#if !UNITY_SERVER
            if (_tipsSceneContext == null)
            {
                CommonDIInstaller commonDiInstaller = FindObjectOfType<CommonDIInstaller>();
                _tipsSceneContext = commonDiInstaller.Resolve<ITipsSceneContext>();
                _spaceModeData = commonDiInstaller.Resolve<ISpaceModeData>();
                _tipsSceneRepository = commonDiInstaller.Resolve<ITipsSceneRepository>();
            }
#endif
        }

        public void SetTipsText(string tipText)
        {
            _tipsText = tipText;
            _tipsSceneRepository.SetTipsText(_tipsText, _tipsId);
        }

        public async void SeTipsID(string tipsId)
        {
            if (string.IsNullOrEmpty(tipsId))
                return;
            _tipsSceneContext.RegisterTipsId(_tipsId);

            string result = await _tipsSceneContext.GetTipsFromId(tipsId);

            if (!string.IsNullOrEmpty(result))
                _tipsText = result;
        }

        protected virtual void ShowTips()
        {
            if (string.IsNullOrEmpty(_tipsText))
            {
                if (!string.IsNullOrEmpty(_tipsId))
                    SeTipsID(_tipsId);
            }

            _tipsSceneRepository.SetTipsText(_tipsText, _tipsId);
        }

        protected virtual void HideTips()
        {
            _tipsSceneRepository.ClearTipsText(_tipsId);
        }
    }
}