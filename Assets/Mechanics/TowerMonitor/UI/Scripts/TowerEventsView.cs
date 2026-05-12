using Assets.Scripts.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public sealed class TowerEventsView : TowerMonitorPanelView<TowerEvent>
    {
        [SerializeField]
        private TowerEventViewItem TowerEventViewItemPrefab;

        private TowerEventMediator _towerEventMediator;
        private List<TowerEventViewItem> _towerEventViewItems;

        public void Init(TowerEventMediator towerEventMediator)
        {
            _towerEventMediator = towerEventMediator;
            _towerEventViewItems = new();
        }

        public void RefreshEventInfo(TowerEvent towerEvent, bool isTicketBought)
        {
            if (towerEvent == null)
            {
                return;
            }
            TowerEventViewItem eventItem = _towerEventViewItems.FirstOrDefault((e) => towerEvent.Id.Equals(e.GetEventGuid()));
            if (eventItem == null)
            {
                return;
            }
            eventItem.RefreshEventInfo(towerEvent, isTicketBought);
        }

        protected override TowerMonitorPanelViewItem CreateItem(TowerEvent item)
        {
            TowerEventViewItem towerEventView = Instantiate(TowerEventViewItemPrefab, TowerViewItemsContent);
            towerEventView.Init(_towerEventMediator);
            towerEventView.RefreshEventInfo(item, false);
            _towerEventViewItems.Add(towerEventView);

            return towerEventView;
        }

        protected override void DestroyItems()
        {
            base.DestroyItems();
            _towerEventViewItems.Clear();
        }
    }
}
