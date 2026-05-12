using Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mechanics.Tips.TipsInteraction
{
    public class PointerTipsObject : TipsObject, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                ShowTips();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                HideTips();
        }
    }
}