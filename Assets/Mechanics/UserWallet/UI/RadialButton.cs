using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mechanics.UserWallet.UI
{
    public class RadialButton : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
    {
        #region Inspector

        public float minValue;
        public float maxValue;

        #endregion

        public float NormalizedValue
        {
            get => _value;
            private set => _value = value;
        }

        public bool Dragged
        {
            get => _dragged;
            private set => _dragged = value;
        }

        private float _value;
        private bool _dragged;
        private RectTransform _currentRectTransform;

        private void Awake()
        {
            _currentRectTransform = GetComponent<RectTransform>();
            TrySetRadioButtonValue(minValue);
        }

        public void TrySetRadioButtonValue(float buttonValue)
        {
            if (buttonValue >= maxValue) NormalizedValue = maxValue;
            else if (buttonValue <= minValue) NormalizedValue = minValue;
            else if (buttonValue< maxValue && buttonValue> minValue) NormalizedValue = buttonValue;
        }

        public void OnPointerMove(PointerEventData data)
        {
            _dragged = true;
            float k = 0;
            float alpha = 0;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_currentRectTransform, data.position,
                    data.pressEventCamera, out Vector2 localRayPosition))
            {
                Vector2 leftMiddle = new Vector2(_currentRectTransform.rect.xMin,
                    (_currentRectTransform.rect.yMax + _currentRectTransform.rect.yMin) / 2);

                alpha = Vector3.SignedAngle(leftMiddle, localRayPosition, Vector2.up);

                if (localRayPosition.y < leftMiddle.y)
                    alpha = 360 - alpha;

                k = alpha / 360;
            }

            TrySetRadioButtonValue(k);

            //float radilValueInPercent = 0;
            //float fullpath = 0;
            //float currentPath = 0;
            //float offset = 0;
            //bool yTop = false;
            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_radialButtonImage.rectTransform, data.position,
            //        data.pressEventCamera, out Vector2 localRayPosition))
            //{
            //    fullpath =_radialButtonImage.rectTransform.rect.width;
            //    currentPath = localRayPosition.x - _radialButtonImage.rectTransform.rect.xMin;
            //}
            //    
            //radilValueInPercent = (currentPath * 100)/fullpath;
            //_radialButtonImage.fillAmount = radilValueInPercent/100;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _dragged = false;
        }
    }
}