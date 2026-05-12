using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocationViewElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Inspector

    [SerializeField] private TMP_Text _locationValue;
    [SerializeField] private TMP_Text _statusValue;
    [SerializeField] private TMP_Text _userValue;
    [SerializeField] private TMP_Text _ownerValue;
    [SerializeField] private GameObject _hoverObject;
    #endregion

    public void SetLocationValue(string name)
    {
        _locationValue.text = name;
    }

    public void SetStatusValue(string space)
    {
        _statusValue.text = space;
    }

    public void SetUsersCountValue(string space)
    {
        _userValue.text = space;
    }

    public void SetOwnerValue(string space)
    {
        _ownerValue.text = space;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverObject != null)
            _hoverObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverObject != null)
            _hoverObject.SetActive(false);
    }
}
