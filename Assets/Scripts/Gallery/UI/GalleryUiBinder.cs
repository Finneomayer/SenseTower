using System;
using Assets.Scripts.Environmental;
using Assets.Scripts.Gallery;
using Assets.Scripts.Gallery.UI;
using UI;
using Unity.Netcode;
using UnityEngine;

public class GalleryUiBinder : ViewPanel
{
    #region Inspector

    [SerializeField] private ViewPanel _loadingPanel;
    [SerializeField] private GalleryInfoTablePanel _infoTablePanel;
    [SerializeField] private GalleryInfrastructure _galleryInfrastructure;
    [SerializeField] private NetworkTriggerObserver _networkTriggerObserver;
    #endregion

    private LookAtPlayer _lookAtPlayer;
    
    private void Awake()
    {
        ShowPanel();
        _loadingPanel.ShowPanel();
        _infoTablePanel.HidePanel();
        
        if(_galleryInfrastructure.GetInfoTable() == null)
            _galleryInfrastructure.GalleryInitialized += OnGalleryInitialized;
        else
            OnGalleryInitialized();

        if (_networkTriggerObserver == null) return;
        
        _networkTriggerObserver.LocalClientExitTrigger += OnLocalClientExitTrigger;
        _networkTriggerObserver.LocalClientEnterTrigger += OnLocalClientEnterTrigger;

    }

    private void OnDisable()
    { 
        _galleryInfrastructure.GalleryInitialized -= OnGalleryInitialized;
        
        if (_networkTriggerObserver == null) return;
        
        _networkTriggerObserver.LocalClientExitTrigger -= OnLocalClientExitTrigger;
        _networkTriggerObserver.LocalClientEnterTrigger -= OnLocalClientEnterTrigger;
    }

    private void OnLocalClientExitTrigger()
    {
        if (_lookAtPlayer == null)
            _lookAtPlayer = GetComponent<LookAtPlayer>();
        
        if(_lookAtPlayer != null)
            _lookAtPlayer.SetPlayer(null);
    }

    private void OnLocalClientEnterTrigger(GameObject triggerColliderObject)
    {
        if (_lookAtPlayer == null)
            _lookAtPlayer = GetComponent<LookAtPlayer>();
        
        if(_lookAtPlayer != null)
            _lookAtPlayer.SetPlayer(triggerColliderObject.transform);
    }

    private void OnGalleryInitialized()
    {
        if (_galleryInfrastructure.GetInfoTable() == null || !_galleryInfrastructure.GetInfoTable().ShowInformation)
        {
            HidePanel();
            return;
        }
        
        _loadingPanel.HidePanel();
        _infoTablePanel.ShowPanel();
        _infoTablePanel.SetGalleryInfoTable(_galleryInfrastructure.GetInfoTable()); 
    }
}
