using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mechanics.LoadSceneObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Office
{
    public class PicturesSpawnerService : MonoBehaviour
    {
        [SerializeField] private PictureInfrastructure pictureInfrastructure;
        [SerializeField] private APIClientSideSceneFunctionalSpawner _apiClientSideRemoteSceneObjectSpawner;
        [SerializeField] private PicturePanel[] _picturePlanes;

        private Dictionary<int, MyImage> _picturesLocation;
        private bool _isOfficeInitialize = false;

        private void OnEnable()
        {
            pictureInfrastructure.SpaceInitialized += OnPictureInitialized;
            _apiClientSideRemoteSceneObjectSpawner.PicturePanelSpawned += OnPicturePlaneSpawned;
        }

        private void OnDisable()
        {
            pictureInfrastructure.SpaceInitialized -= OnPictureInitialized;
            _apiClientSideRemoteSceneObjectSpawner.PicturePanelSpawned -= OnPicturePlaneSpawned;
        }

        private void OnPicturePlaneSpawned(PicturePanel[] picturePanels)
        {
            _picturePlanes = picturePanels;
            FillingPicturesList();
        }

        public void SetPictureFromEditingModeByNumber(PicturePanel panel, int num)
        {
            if (_picturesLocation.Count == 0) return;

            panel.ToogleVisibility(true);
            if (_picturesLocation.ContainsKey(num)) panel.SetTexture(_picturesLocation[num]);
        }

        private async void FillingPicturesList()
        {
            if (_picturePlanes.Length == 0)
                return;

            await UniTask.WaitUntil(() => _isOfficeInitialize);

            _picturesLocation = pictureInfrastructure.GetOfficePicture();

            if (_picturesLocation.Count == 0) return;

            foreach (KeyValuePair<int, MyImage> myImage in _picturesLocation)
            {
                //if (_picturePlanes.Any(element => element.PlaceNumber != myImage.Key))
                //    continue;
                if (myImage.Value == null || string.IsNullOrEmpty(myImage.Value.FileUrl))
                    continue;
                
                PicturePanel tempPicturePlane = _picturePlanes.FirstOrDefault(element => myImage.Key == element.PlaceNumber);

                if (tempPicturePlane != null)
                {
                    tempPicturePlane.ToogleVisibility(true);
                    tempPicturePlane.SetTexture(myImage.Value);
                }
            }
        }

        private void OnPictureInitialized()
        {
            _isOfficeInitialize = true;

            if (_picturePlanes.Length > 0)
                FillingPicturesList();
        }
    }
}