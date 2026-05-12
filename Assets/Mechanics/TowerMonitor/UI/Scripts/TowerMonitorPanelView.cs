using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace UI
{
    public abstract class TowerMonitorPanelView<T> : MonoBehaviour
    {
        [SerializeField]
        protected RectTransform TowerViewItemsContent;

        protected List<TowerMonitorPanelViewItem> _towerViewItems = new();

        public async UniTask Show(T[] towerDataItems)
        {
            await CreateItems(towerDataItems);
        }

        public void Hide()
        {
            DestroyItems();
        }

        protected abstract TowerMonitorPanelViewItem CreateItem(T item);

        private async UniTask CreateItems(T[] towerDataItems)
        {
            DestroyItems();

            foreach (var towerDataItem in towerDataItems)
            {
                TowerMonitorPanelViewItem itemView = CreateItem(towerDataItem);
                _towerViewItems.Add(itemView);
                await UniTask.DelayFrame(1);
            }
            LayoutRebuilder.MarkLayoutForRebuild(TowerViewItemsContent);

            await UniTask.DelayFrame(1);
        }

        protected virtual void DestroyItems()
        {
            foreach (var item in _towerViewItems)
            {
                Destroy(item.gameObject);
            }
            _towerViewItems.Clear();
        }
    }
}
