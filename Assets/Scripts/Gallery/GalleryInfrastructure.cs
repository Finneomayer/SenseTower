using System;
using System.Collections.Generic;
using Assets.Scripts.Space;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Gallery
{
    public class GalleryInfrastructure : MonoBehaviour
    {
        public event Action GalleryInitialized;
        
        #region PrivateVariables
        
        private Gallery _gallery;
        private IGalleryService _galleryService;
        private ISpaceManager _spaceManager;

        #endregion

        [Inject]
        private void Construct(ISpaceManager spaceManager,IGalleryService galleryService)
        {
            _spaceManager = spaceManager;
            _galleryService = galleryService;
            AsyncInit();
        }

        public GalleryInfoTable GetInfoTable()
        {
            return _gallery?.GalleryInfoTable;
        }

        public Dictionary<int, TowerPicture> GetGalleryPicture() 
        {
            return _gallery?.PicturesLocation;
        }

        private async void AsyncInit()
        {
            _gallery = await _galleryService.GetById(_spaceManager.CurrentTransitionTarget.Id.ToString());
            GalleryInitialized?.Invoke();
        }
    }
}