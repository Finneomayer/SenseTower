using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Cinema;
using Assets.Scripts.Environmental.Presentation.Browser;
using Assets.Scripts.Server;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using Zenject;

public class CinemaInfrastructure : MonoBehaviour
{
    #region Inspector
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private string _videoUrl;
    [SerializeField] private GameObject _screen;
    [SerializeField] private GameObject _loading;
    [SerializeField] private AdminPlace _adminPlace;
    #endregion

    private VideoClip _clip;

    private void OnEnable()
    {
#if !UNITY_SERVER
        _adminPlace.AdminChange += OnAdminChange;
#endif
    }

    private void OnDisable()
    {
        _adminPlace.AdminChange -= OnAdminChange;
    }


    private void Start()
    {
#if !UNITY_SERVER
        if (System.IO.File.Exists($"{Application.temporaryCachePath}/cinemaDefaultVideo.mp4")) PlayVideo();
        else StartCoroutine(DownloadVideo(_videoUrl));
#endif
    }

    private void OnAdminChange(ulong previousValue, ulong newValue)
    {
        if (!_adminPlace.IsAdminSet())
        {
            DeactivateBrowser();
        }
        else 
        {
            ActivateBrowser();
        }
    }

    private void DeactivateBrowser()
    {
        if (_videoPlayer != null)
        {
            _videoPlayer.Play();
            _screen.SetActive(true);
        }
    }

    private void ActivateBrowser()
    {
        if (_videoPlayer != null)
        {
            _videoPlayer.Pause();
            _screen.SetActive(false);
        }
    }

    private void PlayVideo()
    {
        try
        {
            _videoPlayer.url = $"{Application.temporaryCachePath}/cinemaDefaultVideo.mp4";

            if (!_adminPlace.IsAdminSet())
            {
                _videoPlayer.Play();
                _screen.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    private IEnumerator DownloadVideo(string url)
    {
        _loading.SetActive(true);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        _loading.SetActive(false);

        if (www.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes($"{Application.temporaryCachePath}/cinemaDefaultVideo.mp4", www.downloadHandler.data);

            PlayVideo();
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

}
