using Assets.Scripts.Event;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Client;
using System;
using System.Linq;
using Assets.Localization;

namespace UI
{
    public sealed class TowerEventsViewPanel : ViewPanel
    {
        [SerializeField]
        private TowerEventsView TowerEventsView;
        [SerializeField]
        private bool UserTicketsOnly;
        [SerializeField]
        private bool AllowFastTravelToEventSpace;
        [SerializeField]
        private LocalizationVariant NotStartedEventLocalizationVariant;

        private ITowerEventService _towerEventService;
        private ITicketService _ticketService;
        private IClientData _clientData;

        private TowerEventMediator _towerEventMediator;
        private bool _isBlockedForInteraction;

        [Inject]
        public void Construct(IClientData clientData, ITowerEventService towerEventService, ITicketService ticketService)
        {
            _towerEventService = towerEventService;
            _ticketService = ticketService;
            _clientData = clientData;

            _towerEventMediator = new();
            TowerEventsView.Init(_towerEventMediator);

            _towerEventMediator.BuyTicketRequested += OnBuyTicketRequested;
            _towerEventMediator.LoadEventSpaceRequested += OnLoadEventSpaceRequested;
        }

        private void OnEnable()
        {
            if (_towerEventMediator != null)
            {
                _towerEventMediator.BuyTicketRequested -= OnBuyTicketRequested;
                _towerEventMediator.BuyTicketRequested += OnBuyTicketRequested;

                _towerEventMediator.LoadEventSpaceRequested -= OnLoadEventSpaceRequested;
                _towerEventMediator.LoadEventSpaceRequested += OnLoadEventSpaceRequested;
            }
        }

        private void OnDisable()
        {
            if (_towerEventMediator != null)
            {
                _towerEventMediator.BuyTicketRequested -= OnBuyTicketRequested;
                _towerEventMediator.LoadEventSpaceRequested -= OnLoadEventSpaceRequested;
            }
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            LoadEvents().Forget();
        }

        public override void HidePanel()
        {
            TowerEventsView.Hide();
            base.HidePanel();
        }

        private void OnBuyTicketRequested(Guid eventId)
        {
            if (!_clientData.UserId.HasValue)
            {
                return;
            }

            BuyTicketAsync(eventId, _clientData.UserId.Value).Forget();
        }

        private void OnLoadEventSpaceRequested(TowerEvent towerEvent)
        {
            if (!AllowFastTravelToEventSpace)
            {
                return;
            }
            TryLoadEventScene(towerEvent).Forget();
        }

        private async UniTask TryLoadEventScene(TowerEvent towerEvent)
        {
            if (towerEvent.Space == null)
            {
                return;
            }

            SetInteractable(false);

            TowerEventsFilter filter = new();
            filter.FromMinusSecondsNow = 300;
            filter.UpToPlusSecondsNow = 900;
            filter.States = new[] { TowerEventState.Planned };
            filter.Spaces = new[] { towerEvent.Space.Id };

            TowerEvent[] currentEvents = await _towerEventService.GetEvents(filter);

            SetInteractable(true);

            if (currentEvents.FirstOrDefault(e => e.Id.Equals(towerEvent.Id)) == null)
            {
                ShowNotification(NotStartedEventLocalizationVariant.Localize());
                return;
            }

            SceneChangerView sceneChangerView = FindObjectOfType<SceneChangerView>();
            if (sceneChangerView == null)
            {
                Debug.LogError("SceneChangerView not found!");
                return;
            }

            sceneChangerView.ChangeSpace(towerEvent.Space.SpaceType, towerEvent.Space.Id.ToString());
        }

        private async UniTask LoadEvents()
        {
            await UniTask.WaitWhile(() => _isBlockedForInteraction);

            SetInteractable(false);

            TowerEvent[] towerEvents = await _towerEventService.GetEvents((int?)null);

            if (towerEvents == null)
            {
                SetInteractable(true);
                return;
            }

            if (UserTicketsOnly)
            {
                if (!_clientData.UserId.HasValue)
                {
                    SetInteractable(true);
                    return;
                }

                towerEvents = towerEvents.Where(e => e.SoldTickets != null 
                    && e.SoldTickets.FirstOrDefault(t => t.UserId != null 
                        && t.UserId.Value.Equals(_clientData.UserId.Value)) != null).ToArray();
            }

            towerEvents = towerEvents.OrderBy(e => e.From).ToArray();

            await TowerEventsView.Show(towerEvents);

            SetInteractable(true);

            if (!_clientData.UserId.HasValue)
            {
                return;
            }

            foreach (var towerEvent in towerEvents)
            {
                Ticket userTicket = null;
                if (towerEvent.SoldTickets != null)
                {
                    userTicket = towerEvent.SoldTickets.FirstOrDefault((t) => t.UserId.Equals(_clientData.UserId.Value));
                }

                TowerEventsView.RefreshEventInfo(towerEvent, userTicket != null);
            }
        }

        private async UniTask BuyTicketAsync(Guid eventId, Guid userId)
        {
            SetInteractable(false);

            bool bought = await _ticketService.BuyTicket(eventId, userId);

            TowerEvent refreshedEvent = await _towerEventService.GetEvent(eventId);

            SetInteractable(true);

            if (refreshedEvent == null)
            {
                Debug.LogWarning($"<color=red>Tower event {eventId} not exists</color>");
                await LoadEvents();
            }
            else
            {
                TowerEventsView.RefreshEventInfo(refreshedEvent, bought);
            }
        }

        private void SetInteractable(bool interactable)
        {
            _isBlockedForInteraction = !interactable;
            CanvasGroup.alpha = interactable ? 1 : 0.5f;
            CanvasGroup.interactable = interactable;
        }

        private void ShowNotification(string text)
        {
            NotificationPanel notificationPanel = NotificationPanel.instance;
            if (notificationPanel != null)
            {
                notificationPanel.SetInfo(text);
                notificationPanel.ShowPanel();
            }
        }
    }
}
