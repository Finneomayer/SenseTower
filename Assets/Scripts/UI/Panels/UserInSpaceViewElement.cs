using Assets.UI.Pad;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI_assets_legacy.Pad
{
    public class UserInSpaceViewElement : VisitorViewElement,IPointerEnterHandler, IPointerExitHandler
    {
        #region Inspector

        [SerializeField] private TMP_Text _nameValue;
        [SerializeField] private TMP_Text _statusValue;
        [SerializeField] private TMP_Text _locationValue;
        [SerializeField] private TMP_Text _karmaValue;
        [SerializeField] private GameObject _hoverObject;
        #endregion

        public void SetNameValue(string name)
        {
            _nameValue.text = name;
        }

        public void SetSpaceValue(string space)
        {
            _locationValue.text = space;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_hoverObject != null)
                _hoverObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_hoverObject != null)
                _hoverObject.SetActive(false);
        }
    }
}