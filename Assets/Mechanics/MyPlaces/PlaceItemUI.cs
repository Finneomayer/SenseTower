using System;
using Assets.Scripts.Space;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _adminText;
    [SerializeField] private Image _selectedImage;
    public Action OnSelect;

    /// <summary>
    /// for transition from EnterScene to MyPlace when clicked
    /// </summary>
    private string _spaceId;

    private SpaceType _spaceType = SpaceType.Null;

    public void CurrentUserIsAdmin(bool flag)
    {
        _adminText.enabled = flag;
    }

    public void SetName(string name)
    {
        _name.text = name;
    }

    public void SetSpaceId(string id)
    {
        _spaceId = id;
    }

    public string GetSpaceId() 
    {
        return _spaceId;
    }

    public SpaceType GetSpaceType()
    {
        return _spaceType;
    }

    public void SetSpaceType(SpaceType spaceType)
    {
        _spaceType = spaceType;
    }

    public void SetSelected(bool flag)
    {
        if (_selectedImage != null)_selectedImage.enabled = flag;
    }
}
