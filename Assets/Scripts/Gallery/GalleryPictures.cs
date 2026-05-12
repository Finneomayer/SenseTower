using System;
using Assets.Scripts.Gallery;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gallery.UI;
using UnityEngine;

public class GalleryPictures : MonoBehaviour
{
    [SerializeField] private GalleryInfrastructure _galleryInfrastructure;

    [SerializeField] private GalleryPicturePanel _galleryPicturePrefab;
    [SerializeField] private SpawnPointPicture[] _spawnPoints;

    public Action DownloadedPictureCallback;

    private Dictionary<int, TowerPicture> _picturesLocation;
    private NetworkStartInvoker _networkStarter;
    private float _picturesCount = 0;
    private float _downloadedPicturesCount;
    private Coroutine _downloadedCountChecking;

    private void Start()
    {
        _networkStarter = GetComponent<NetworkStartInvoker>();

        DownloadedPictureCallback += () =>
        {
            _downloadedPicturesCount++;
        };

        if (_galleryInfrastructure.GetGalleryPicture() == null)
            _galleryInfrastructure.GalleryInitialized += OnGalleryInitialized;
        else
            OnGalleryInitialized();
    }

    private void OnGalleryInitialized()
    {
        _picturesLocation = _galleryInfrastructure.GetGalleryPicture();

        if (_picturesLocation == null)
        {
            _networkStarter.StartNetwork();
            return;
        }

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_picturesLocation.ContainsKey(_spawnPoints[i].PictureNumber))
            {
                _picturesCount++;
                GalleryPicturePanel panel = Instantiate(_galleryPicturePrefab, _spawnPoints[i].transform);
                panel.SetPicture(_picturesLocation[_spawnPoints[i].PictureNumber], DownloadedPictureCallback);
            }
        }

        _downloadedCountChecking = StartCoroutine(DownLoadedCountChecking());
    }

    private IEnumerator DownLoadedCountChecking()
    {
        while (true)
        {
            if (_picturesCount == 0)
            {
                _networkStarter.StartNetwork();
                yield break;
            }
            if (_picturesCount != 0 && _downloadedPicturesCount == _picturesCount)
            {
                _networkStarter.StartNetwork();
                StopCoroutine(_downloadedCountChecking);
            }
            Debug.LogWarning($"download {_downloadedPicturesCount} out of a {_picturesCount} pictures");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(_downloadedCountChecking);
    }
}
