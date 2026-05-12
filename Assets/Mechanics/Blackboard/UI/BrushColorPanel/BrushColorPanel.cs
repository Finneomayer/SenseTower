using UnityEngine;
using UI;

namespace Assets.Blackboard
{
    public class BrushColorPanel : ViewPanel
    {
        [SerializeField]
        private SelectColorButton[] ColorButtons;

        private BlackBoardZone _blackBoardZone;

        private void OnEnable()
        {
            foreach (var button in ColorButtons)
            {
                button.InteractElement.onClick.AddListener(() => OnColorButtonClicked(button));
            }
        }

        private void OnDisable()
        {
            foreach (var button in ColorButtons)
            {
                button.InteractElement.onClick.RemoveAllListeners();
            }
        }

        public void Init(BlackBoardZone blackBoardZone)
        {
            _blackBoardZone = blackBoardZone;
        }

        public override void ShowPanel()
        {
            if (_blackBoardZone == null || _blackBoardZone.Marker == null)
            {
                return;
            }

            Color currentColor = _blackBoardZone.Marker.GetColor();

            foreach (var button in ColorButtons)
            {
                button.SetButtonActive(button.SelectValue == currentColor);
            }

            base.ShowPanel();
        }

        private void OnColorButtonClicked(SelectColorButton clickedSelectColorButton)
        {
            if (_blackBoardZone == null || _blackBoardZone.Marker == null)
            {
                return;
            }

            _blackBoardZone.Marker.SetColor(clickedSelectColorButton.SelectValue);

            foreach (var button in ColorButtons)
            {
                button.SetButtonActive(button == clickedSelectColorButton);
            }
        }
    }
}
