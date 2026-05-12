using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField]
    private AudioSource AudioSource;

    private AgoraVoice _agoraVoice;

    private void Awake()
    {
#if !UNITY_SERVER
        _agoraVoice = FindObjectOfType<AgoraVoice>();
#endif
    }

    private void OnEnable()
    {
        if (_agoraVoice != null)
        {
            _agoraVoice.LocalPlayerJoinChannel += OnAgoraLocalPlayerJoinChannel;
        }
    }

    private void OnDisable()
    {
        if (_agoraVoice != null)
        {
            _agoraVoice.LocalPlayerJoinChannel -= OnAgoraLocalPlayerJoinChannel;
        }
    }

    private void OnAgoraLocalPlayerJoinChannel(uint channelId)
    {
        if (_agoraVoice.CurrentAudioChannelFullName == _agoraVoice.DefaultAudioChannelFullName)
        {
            PlayMusic();
        }
        else
        {
            PauseMusic();
        }
    }

    private void PlayMusic()
    {
        if (!AudioSource.isPlaying)
        {
            AudioSource.Play();
        }
    }

    private void PauseMusic()
    {
        if (AudioSource.isPlaying)
        {
            AudioSource.Pause();
        }
    }
}
