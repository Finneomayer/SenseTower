using Assets.Scripts.Event;
using UnityEngine;
using TMPro;
using Assets.Mechanics.Doors;
using UnityEngine.UI;
using Assets.Localization;

namespace UI
{
    public sealed class DoorTowerEventViewItem : TowerMonitorPanelViewItem
    {
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup InfoLayoutGroup;
        [SerializeField]
        private TMP_Text DateTimeText;
        [SerializeField]
        private TMP_Text TitleText;
        [SerializeField]
        private TMP_Text DescriptionText;
        [SerializeField]
        private TMP_Text EntranceAvailabilityText;
        [SerializeField]
        private Color EntranceTextAllowedColor;
        [SerializeField]
        private Color EntranceTextDeniedColor;
        [SerializeField]
        private TowerEventTicketView TicketInfo;

        [SerializeField]
        private LocalizationVariant AdminAccessLocalizationVariant;
        [SerializeField]
        private LocalizationVariant UserWithTicketAccessLocalizationVariant;
        [SerializeField]
        private LocalizationVariant UserWithoutTicketAccessLocalizationVariant;
        [SerializeField]
        private LocalizationVariant GetTicketQuestionLocalizationVariant;

        private TowerEvent _towerEvent;
        private ActiveDoor _activeDoor;

        public void Init(ActiveDoor activeDoor, TowerEventMediator towerEventMediator)
        {
            _activeDoor = activeDoor;
            TicketInfo.Init(towerEventMediator);
        }

        public void RefreshEventInfo(TowerEvent towerEvent, bool isTicketBought)
        {
            TowerEvent oldEvent = _towerEvent;
            _towerEvent = towerEvent;

            DateTimeText.text = GetEventDateTimeString(_towerEvent.Date);
            TitleText.text = _towerEvent.Title;
            DescriptionText.text = _towerEvent.Description;

            if (oldEvent == null || _towerEvent.ImageUrl != oldEvent.ImageUrl)
            {
                ImageSetFinished -= OnImageSetFinished;
                ImageSetFinished += OnImageSetFinished;
                InitImage(_towerEvent.ImageUrl);
            }

            if (_activeDoor.IsLocalUserOwner || _activeDoor.IsLocalUserAdmin)
            {
                TicketInfo.RefreshTicketInfo(_towerEvent, true);
                EntranceAvailabilityText.color = EntranceTextAllowedColor;
                EntranceAvailabilityText.text = AdminAccessLocalizationVariant.Localize();
                EntranceAvailabilityText.gameObject.SetActive(true);
                return;
            }

            TicketInfo.RefreshTicketInfo(_towerEvent, isTicketBought);

            bool needShowEntranceAvailabilityText = !towerEvent.IsFreeEvent();
            if (needShowEntranceAvailabilityText)
            {               
                if (isTicketBought)
                {
                    EntranceAvailabilityText.color = EntranceTextAllowedColor;
                    EntranceAvailabilityText.text = UserWithTicketAccessLocalizationVariant.Localize();                  
                }
                else
                {
                    EntranceAvailabilityText.color = EntranceTextDeniedColor;
                    EntranceAvailabilityText.text = UserWithoutTicketAccessLocalizationVariant.Localize();
                    if (towerEvent.TotalTickets > towerEvent.Sold)
                    {
                        EntranceAvailabilityText.text += $" {GetTicketQuestionLocalizationVariant.Localize()}";
                    }
                }
            }

            EntranceAvailabilityText.gameObject.SetActive(needShowEntranceAvailabilityText);
        }

        private void OnImageSetFinished(bool imageSetted)
        {
            ImageSetFinished -= OnImageSetFinished;

            Image.gameObject.SetActive(imageSetted);
            RefreshInfoAlignment();
        }

        private void RefreshInfoAlignment()
        {
            TextAlignmentOptions textAlignment;
            TextAnchor layoutGroupAlignment;
            if (Image.gameObject.activeSelf)
            {
                textAlignment = TextAlignmentOptions.TopLeft;
                layoutGroupAlignment = TextAnchor.MiddleLeft;
            }
            else
            {
                textAlignment = TextAlignmentOptions.Top;
                layoutGroupAlignment = TextAnchor.MiddleCenter;
            }

            InfoLayoutGroup.childAlignment = layoutGroupAlignment;
            DateTimeText.alignment = textAlignment;
            TitleText.alignment = textAlignment;
            DescriptionText.alignment = textAlignment;
        }
    }
}
