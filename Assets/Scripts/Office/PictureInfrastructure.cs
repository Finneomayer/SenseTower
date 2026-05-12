using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.MyPlaces;
using Assets.Scripts.API;
using Assets.Scripts.Space;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Office
{
    public class PictureInfrastructure : MonoBehaviour
    {
        [SerializeField] private PlacePictureChooser _pictureChooser;
        public event Action SpaceInitialized;

        private ISpaceManager _spaceManager;
        private Dictionary<int, MyImage> _spaceImages;
        private Coroutine _imageUpdateCoroutine;
        private IApiService _apiService;

        [Inject]
        private void Construct(IApiService apiService, ISpaceManager spaceManager)
        {
            _apiService = apiService;
            MyPlaceService.UpdateMyPlaceDataEvent += RefreshImages; //first runtime image updater
            //if (_pictureChooser != null) _pictureChooser.ImageChanged += PictureChooser_ImageChanged; //second runtime image updater with delay
            _spaceManager = spaceManager;
        }

        private void Awake()
        {
            AsyncInit();
            if (_imageUpdateCoroutine != null)
            {
                StopCoroutine(_imageUpdateCoroutine);
                _imageUpdateCoroutine = null;
            }

            _imageUpdateCoroutine = StartCoroutine(UpdateImageCoroutine());
        }

        private void OnDestroy()
        {
            MyPlaceService.UpdateMyPlaceDataEvent -= RefreshImages;
            //if (_pictureChooser != null) _pictureChooser.ImageChanged -= PictureChooser_ImageChanged;
        }

        public Dictionary<int, MyImage> GetOfficePicture()
        {
            return _spaceImages;
        }

        public void RefreshImages()
        {
            AsyncInit();
        }

        private async void AsyncInit()
        {
#if !UNITY_SERVER

            if (_spaceManager.CurrentTransitionTarget != null)
            {
                _spaceImages = _spaceManager.CurrentTransitionTarget.Images;
                string url =
                    APIService.AddLanguageParameter(
                        $"{APIService.GetSpacesUrl}/{_spaceManager.CurrentTransitionTarget.Id.ToString()}");
                LocalSpace response = await _apiService.GetWithAuthToken<LocalSpace>(url);
                if (response != null)
                {
                    _spaceImages = response.Images;
                    SpaceInitialized?.Invoke();
                }
            }
#endif
        }

        //private void PictureChooser_ImageChanged()
        //{
        //    if (_imageUpdateCoroutine != null) 
        //    {
        //        StopCoroutine(_imageUpdateCoroutine);
        //        _imageUpdateCoroutine = null;
        //    }
//
        //    _imageUpdateCoroutine = StartCoroutine(UpdateImageCoroutine());
        //}

        private IEnumerator UpdateImageCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                RefreshImages();
            }
        }
    }
}