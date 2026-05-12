using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSeatSelectable : MonoBehaviour
    {
        [SerializeField]
        private PlayerSelectInteractable PlayerSelectInteractable;
        [SerializeField]
        private PlayerSeatSelectVisualizer SelectVisualizer;

        private MafiaEventMediatorClient _eventMediator;
        private bool _canBeSelected;
        private int _seatNumber;

        public bool IsSelected { get; private set; }

        private void Awake()
        {
            PlayerSelectInteractable.SetActive(false);
        }

        private void OnEnable()
        {
            PlayerSelectInteractable.HoverEntered += OnInteractableHoverEntered;
            PlayerSelectInteractable.HoverExited += OnInteractableHoverExited;
            PlayerSelectInteractable.Selected += OnInteractableSelected;
        }

        private void OnDisable()
        {
            PlayerSelectInteractable.HoverEntered -= OnInteractableHoverEntered;
            PlayerSelectInteractable.HoverExited -= OnInteractableHoverExited;
            PlayerSelectInteractable.Selected -= OnInteractableSelected;
        }

        public void Init(MafiaEventMediatorClient eventMediator, int seatNumber, bool canBeSelected)
        {
            _eventMediator = eventMediator;
            _seatNumber = seatNumber;
            _canBeSelected = canBeSelected;
            PlayerSelectInteractable.SetActive(_canBeSelected);

            if (!_canBeSelected)
            {
                UnselectSeat();
            }
        }

        public Color GetSelectedColor()
        {
            return SelectVisualizer.GetSelectedColor();
        }

        public void SelectSeat()
        {
            IsSelected = true;
            SelectVisualizer.SelectVisual();
        }

        public void UnselectSeat()
        {
            IsSelected = false;
            SelectVisualizer.HideVisual();
        }

        private void OnInteractableHoverEntered()
        {
            if (IsSelected)
            {
                return;
            }

            if (_canBeSelected)
            {
                SelectVisualizer.HoverVisual();
            }
        }

        private void OnInteractableHoverExited()
        {
            if (IsSelected)
            {
                return;
            }
            SelectVisualizer.HideVisual();
        }

        private void OnInteractableSelected()
        {
            if (_canBeSelected && _eventMediator != null)
            {
                _eventMediator.RequestSelectPlayer(_seatNumber);
            }
        }
    }
}
