using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.Tips.Model;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Assets.Mechanics.Tips
{
    public class TipsSceneContext : ITipsSceneContext, IInitializable
    {
        private List<TipsItemDto> _allTips = new();
        private List<string> _tipsIds = new();

        private ITipsSceneRepository _tipsSceneRepository;
        private readonly ITipsService _tipsService;

        private TipsSceneContext(ITipsSceneRepository tipsSceneRepository, ITipsService tipsService)
        {
            _tipsService = tipsService;
            _tipsSceneRepository = tipsSceneRepository;
            _tipsSceneRepository.OnTipsChanged += OnTipsChanged;
        }

        public void RegisterTipsId(string tipsId)
        {
            if (!_tipsIds.Contains(tipsId))
            {
                _tipsIds.Add(tipsId);
            }
        }

        public async void GetAllTips()
        {
            _allTips = await _tipsService.GetTipsFromIDs(_tipsIds);
        }

        public async UniTask<string> GetTipsFromId(string tipsId)
        {
            var utcs = new UniTaskCompletionSource<string>();

            TipsItemDto tipsItemDto = _allTips.FirstOrDefault(element => element.Id == tipsId);
            
            if (tipsItemDto != null)
            {
                return tipsItemDto.Content;
            }

            string result = await _tipsService.GetTipsFromID(tipsId);
            _allTips.Add(new TipsItemDto(){Id = tipsId, Content = result});
            
            utcs.TrySetResult(result);

            return await utcs.Task;
        }

        public void ShowTips()
        {
            foreach (TipsViewer tipsViewer in _tipsSceneRepository.GetRegisteredViewers())
            {
                tipsViewer.SetViewText(_tipsSceneRepository.GetTipsText());
            }
        }

        public void CleanUp()
        {
            _tipsSceneRepository.OnTipsChanged -= OnTipsChanged;
        }

        private void OnTipsChanged()
        {
            ShowTips();
        }

        public void Initialize()
        {
#if !UNITY_SERVER
            GetAllTips();
#endif
        }
    }
}