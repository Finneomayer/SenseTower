using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Assets.Scripts.News;

namespace UI
{
    public class TowerNewsViewPanel : ViewPanel
    {
        [SerializeField]
        private TowerNewsView TowerNewsView;

        private ITowerNewsService _towerNewsService;

        [Inject]
        public void Construct(ITowerNewsService towerNewsService)
        {
            _towerNewsService = towerNewsService;
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            LoadEvents().Forget();
        }

        public override void HidePanel()
        {
            TowerNewsView.Hide();
            base.HidePanel();
        }

        private async UniTask LoadEvents()
        {
            await TowerNewsView.Show(await _towerNewsService.GetNews(10));
        }
    }
}
