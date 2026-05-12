using Assets.Scripts.Gallery;
using UI;
using UnityEngine;

public class HelperUI : MonoBehaviour
{
    #region Inspector

    [SerializeField] private ViewPanel defaultMeetingPanel;
    [SerializeField] private ViewPanel GalleryViewPanel;
    [SerializeField] private GalleryInfrastructure _galleryInfrastructure;

    #endregion

    private bool _isVisible = false;

    private void Start()
    {
        GalleryViewPanel.HidePanel();
    }

    public void OnSelect()
    {
        _isVisible = !_isVisible;
        
        if (!_isVisible)
        {
            defaultMeetingPanel.HidePanel();
            GalleryViewPanel.HidePanel();
            return;
        }

        if (_galleryInfrastructure.GetInfoTable() == null)
        {
            defaultMeetingPanel.ShowPanel();
            GalleryViewPanel.HidePanel();   
        }
        else
        {
            defaultMeetingPanel.HidePanel();
            GalleryViewPanel.ShowPanel(); 
        }
    }
}
