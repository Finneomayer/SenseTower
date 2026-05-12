using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using static Data.Enumenators;

namespace Assets.Blackboard
{
    public class ShapePanel : ViewPanel
    {
        #region Inspector
        [SerializeField] private ShapeButton[] _shapeButtons;

        #endregion

        private BlackBoardZone _blackBoardZone;

        private void OnEnable()
        {
            foreach (var button in _shapeButtons)
            {
                button.InteractElement.onClick.AddListener(() => OnShapeButtonClicked(button));
            }
        }

        private void OnDisable()
        {
            foreach (var button in _shapeButtons)
            {
                button.InteractElement.onClick.RemoveAllListeners();
            }
        }

        public override void ShowPanel()
        {
            if (_blackBoardZone == null || _blackBoardZone.BlackBoard == null)
            {
                return;
            }

            ShapeType currentShape = _blackBoardZone.BlackBoard.GetShapeType();

            foreach (var button in _shapeButtons)
            {
                button.SetButtonActive(button.SelectValue == currentShape);
            }

            base.ShowPanel();
        }
        public override void HidePanel()
        {
            if (_blackBoardZone != null && _blackBoardZone.BlackBoard != null)
                _blackBoardZone.BlackBoard.SetShapeType(ShapeType.Line);
            
            base.HidePanel();
        }
        public void Init(BlackBoardZone blackBoardZone)
        {
            _blackBoardZone = blackBoardZone;
        }

        private void OnShapeButtonClicked(ShapeButton clickedSelectShapeButton)
        {
            if (_blackBoardZone == null || _blackBoardZone.BlackBoard == null)
            {
                return;
            }
            
            _blackBoardZone.BlackBoard.SetShapeType(clickedSelectShapeButton.SelectValue);
            foreach (var button in _shapeButtons)
            {
                button.SetButtonActive(button == clickedSelectShapeButton);
            }
        }
    }
}