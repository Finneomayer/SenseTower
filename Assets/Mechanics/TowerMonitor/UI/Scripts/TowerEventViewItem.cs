using Assets.Scripts.Event;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace UI
{
    public sealed class TowerEventViewItem : TowerMonitorPanelViewItem, IPointerClickHandler
    {
        [SerializeField]
        private TMP_Text DateTimeText;
        [SerializeField]
        private TMP_Text TitleText;
        [SerializeField]
        private TMP_Text SpaceText;
        [SerializeField]
        private TMP_Text DescriptionText;
        [SerializeField]
        private TowerEventTicketView TicketInfo;

        private TowerEvent _towerEvent;
        private TowerEventMediator _towerEventMediator;

        public void Init(TowerEventMediator towerEventMediator)
        {
            _towerEventMediator = towerEventMediator;
            TicketInfo.Init(_towerEventMediator);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_towerEventMediator != null)
            {
                _towerEventMediator.RequestLoadEventSpace(_towerEvent);
            }
        }

        public Guid GetEventGuid()
        {
            return _towerEvent.Id;
        }

        public void RefreshEventInfo(TowerEvent towerEvent, bool isTicketBought)
        {
            TowerEvent oldEvent = _towerEvent;
            _towerEvent = towerEvent;

            DateTimeText.text = GetEventDateTimeString(_towerEvent.Date);
            TitleText.text = _towerEvent.Title;
            SpaceText.text = _towerEvent.Space != null ? towerEvent.Space.SpaceName : "Unknown space";
            DescriptionText.text = _towerEvent.Description;

            if (oldEvent == null || _towerEvent.ImageUrl != oldEvent.ImageUrl)
            {
                InitImage(_towerEvent.ImageUrl);
            }
            
            TicketInfo.RefreshTicketInfo(_towerEvent, isTicketBought);
        }
    }
}
