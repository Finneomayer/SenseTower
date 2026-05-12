using UnityEngine;
using Assets.Scripts.News;

namespace UI
{
    public sealed class TowerNewsView : TowerMonitorPanelView<TowerNews>
    {
        [SerializeField]
        private TowerNewsViewItem TowerNewsViewItemPrefab;

        protected override TowerMonitorPanelViewItem CreateItem(TowerNews item)
        {
            TowerNewsViewItem towerEventView = Instantiate(TowerNewsViewItemPrefab, TowerViewItemsContent);
            towerEventView.Init(item);
            return towerEventView;
        }
    }
}
