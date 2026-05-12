using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Mechanics.GuestEnter
{
    public class GuestEnterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Inspector

        [SerializeField] private ViewPanel _guestInfoPanel;

        #endregion
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_guestInfoPanel != null)
                _guestInfoPanel.ShowPanel();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_guestInfoPanel != null)
                _guestInfoPanel.HidePanel();
        }
    }
}