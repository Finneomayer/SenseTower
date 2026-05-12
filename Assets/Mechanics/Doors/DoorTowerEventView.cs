using UnityEngine;
using Assets.Scripts.Event;
using UI;
using Cysharp.Threading.Tasks;
using System;
using Zenject;
using Assets.Scripts.Client;
using System.Linq;
using Assets.Scripts.API;
using Assets.Scripts.Models;

namespace Assets.Mechanics.Doors
{
    public class DoorTowerEventView : MonoBehaviour
    {
        [SerializeField]
        private ActiveDoor ActiveDoor;
        [SerializeField]
        private CanvasGroup TowerEventViewItemContent;
        [SerializeField]
        private DoorTowerEventViewItem TowerEventViewItem;
        [SerializeField]
        private DoorPlayerSensor _doorPlayerSensor;

        private TowerEvent _towerEvent;
        private TowerEventMediator _towerEventMediator;
        private bool _isPlayerInShowCollider;

        private ITowerEventService _towerEventService;
        private ITicketService _ticketService;
        private IClientData _clientData;
        private IRegistrationInSpacesService _registrationInSpacesService;

        [Inject]
        public void Construct(IClientData clientData, ITowerEventService towerEventService, 
            ITicketService ticketService, IRegistrationInSpacesService registrationInSpacesService)
        {
            _towerEventService = towerEventService;
            _ticketService = ticketService;
            _clientData = clientData;
            _registrationInSpacesService = registrationInSpacesService;
        }

        private void Awake()
        {
            _towerEventMediator = new();
            TowerEventViewItem.Init(ActiveDoor, _towerEventMediator);
            _doorPlayerSensor.OnDoorNearEnter += _doorPlayerSensor_OnDoorNearEnter;
            _doorPlayerSensor.OnDoorNearExit += _doorPlayerSensor_OnDoorNearExit;
        }

        private void _doorPlayerSensor_OnDoorNearExit(Collider other)
        {
            if (!other.TryGetComponent(out Camera _))
            {
                return;
            }
            _isPlayerInShowCollider = false;
            RefreshActiveState();
        }

        private void _doorPlayerSensor_OnDoorNearEnter(Collider other)
        {
            if (!other.TryGetComponent(out Camera _))
            {
                return;
            }
            _isPlayerInShowCollider = true;
            RefreshActiveState();
        }

        private void OnEnable()
        {
            ActiveDoor.SpaceTowerEventChanged += OnActiveDoorTowerEventChanged;
            _towerEventMediator.BuyTicketRequested += OnBuyTicketRequested;
        }

        private void OnDisable()
        {
            ActiveDoor.SpaceTowerEventChanged -= OnActiveDoorTowerEventChanged;
            _towerEventMediator.BuyTicketRequested -= OnBuyTicketRequested;
        }

        private void OnDestroy()
        {
            _doorPlayerSensor.OnDoorNearEnter -= _doorPlayerSensor_OnDoorNearEnter;
            _doorPlayerSensor.OnDoorNearExit -= _doorPlayerSensor_OnDoorNearExit;
        }

        private void RefreshActiveState()
        {
            TowerEventViewItemContent.gameObject.SetActive(_towerEvent != null && _isPlayerInShowCollider);
        }

        private void SetInteractable(bool interactable)
        {
            TowerEventViewItemContent.alpha = interactable ? 1 : 0.05f;
            TowerEventViewItemContent.interactable = interactable;
        }

        private async UniTask<bool> BuyTicketAsync(Guid eventId, Guid userId)
        {
            SetInteractable(false);
            
            bool bought = await _ticketService.BuyTicket(eventId, userId);

            if (bought)
            {
                TowerEvent refreshedEvent = await _towerEventService.GetEvent(eventId);
                ActiveDoor.SetTowerEvent(refreshedEvent);

                AccessResultDto accessData = await _registrationInSpacesService.CheckAccess(ActiveDoor.SpaceId);
                ActiveDoor.SetAccessData(accessData);
            }
          
            SetInteractable(true);

            return bought;
        }

        private void OnActiveDoorTowerEventChanged(TowerEvent towerEvent)
        {
            _towerEvent = towerEvent;
            if (_towerEvent != null)
            {
                bool isTicketBought = ActiveDoor.IsLocalUserOwner;
                if (!isTicketBought && towerEvent.SoldTickets != null && _clientData.UserId.HasValue)
                {
                    isTicketBought = towerEvent.SoldTickets.FirstOrDefault(
                        (t) => t.UserId.Equals(_clientData.UserId.Value)) != null;
                }
                TowerEventViewItem.RefreshEventInfo(towerEvent, isTicketBought);
            }
            RefreshActiveState();
        }

        private void OnBuyTicketRequested(Guid eventId)
        {
            if (!_clientData.UserId.HasValue)
            {
                return;
            }
            BuyTicketAsync(eventId, _clientData.UserId.Value).Forget();
        }
    }
}