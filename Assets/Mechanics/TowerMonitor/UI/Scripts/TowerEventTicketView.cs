using Assets.Scripts.Event;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Localization;

namespace UI
{
    public sealed class TowerEventTicketView : MonoBehaviour
    {
        [SerializeField]
        private GameObject BuyTicketPanel;
        [SerializeField]
        private GameObject BuyTicketConfirmPanel;
        [SerializeField]
        private GameObject BoughtTicketPanel;
        [SerializeField]
        private TMP_Text EntranceModeText;
        [SerializeField]
        private TMP_Text TicketsAvailableText;
        [SerializeField]
        private Button BuyTicketButton;
        [SerializeField]
        private Button ConfirmBuyTicketButton;
        [SerializeField]
        private Button CancelBuyTicketButton;
        
        [SerializeField]
        private LocalizationVariant FreeEntranceModeLocalizationVariant;
        [SerializeField]
        private LocalizationVariant EntranceModeByTicketsLocalizationVariant;
        [SerializeField]
        private LocalizationVariant TicketAvailableCountLocalizationVariant;

        private TowerEvent _towerEvent;
        private TowerEventMediator _towerEventMediator;

        private void OnEnable()
        {
            BuyTicketButton.onClick.AddListener(OnBuyTicketButtonClick);
            ConfirmBuyTicketButton.onClick.AddListener(OnConfirmBuyTicketButtonClick);
            CancelBuyTicketButton.onClick.AddListener(OnCancelBuyTicketButtonClick);
        }

        private void OnDisable()
        {
            BuyTicketButton.onClick.RemoveListener(OnBuyTicketButtonClick);
            ConfirmBuyTicketButton.onClick.RemoveListener(OnConfirmBuyTicketButtonClick);
            CancelBuyTicketButton.onClick.RemoveListener(OnCancelBuyTicketButtonClick);
        }

        public void Init(TowerEventMediator towerEventMediator)
        {
            _towerEventMediator = towerEventMediator;
            SetCommonTicketInfoMode();
        }

        public void RefreshTicketInfo(TowerEvent towerEvent, bool bought)
        {
            _towerEvent = towerEvent;

            if (_towerEvent.IsFreeEvent())
            {
                EntranceModeText.text = FreeEntranceModeLocalizationVariant.Localize();
            }
            else
            {
                EntranceModeText.text = EntranceModeByTicketsLocalizationVariant.Localize();
            }

            if (!bought && _towerEvent.TotalTickets > 0)
            {
                string availableText = TicketAvailableCountLocalizationVariant.Localize(
                    _towerEvent.TotalTickets - _towerEvent.Sold, _towerEvent.TotalTickets);
                TicketsAvailableText.text = $"({availableText})";
            }
            else
            {
                TicketsAvailableText.text = "";
            }

            if (bought)
            {
                SetBoughtTicketInfoMode();
            }
            else
            {
                if (BuyTicketConfirmPanel.activeSelf && _towerEvent.TotalTickets > _towerEvent.Sold)
                {
                    SetConfirmBuyTicketInfoMode();
                }
                else
                {
                    SetCommonTicketInfoMode();
                }
            }
        }

        private void SetCommonTicketInfoMode()
        {
            BuyTicketPanel.SetActive(IsAvailableToBuy());
            BoughtTicketPanel.SetActive(false);
            BuyTicketConfirmPanel.SetActive(false);
        }

        private void SetBoughtTicketInfoMode()
        {
            
            BuyTicketPanel.SetActive(false);
            BoughtTicketPanel.SetActive(true);
            BuyTicketConfirmPanel.SetActive(false);
        }

        private void SetConfirmBuyTicketInfoMode()
        {
            BuyTicketPanel.SetActive(false);
            BoughtTicketPanel.SetActive(false);
            BuyTicketConfirmPanel.SetActive(true);
        }

        private void OnCancelBuyTicketButtonClick()
        {
            SetCommonTicketInfoMode();
        }

        private void OnConfirmBuyTicketButtonClick()
        {
            _towerEventMediator.RequestBuyTicket(_towerEvent.Id);
        }

        private void OnBuyTicketButtonClick()
        {
            SetConfirmBuyTicketInfoMode();
        }

        private bool IsAvailableToBuy()
        {
            if (_towerEvent == null)
            {
                return false;
            }
            return _towerEvent.Sold < _towerEvent.TotalTickets;
        }
    }
}
