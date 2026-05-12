using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SenseAI.Robot
{
    public class AudioController : MonoBehaviour
    {
        private const float SqrMinPositionShiftToPlaySound = 0.1f;

        [SerializeField] private AudioSource _audio;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _speed;

        private AgoraVoice _agoraVoice;
        private const float VOLUME = 0.3f;

        private void Awake()
        {
#if UNITY_SERVER
                enabled = false;
#endif
            Cashing();
            _agoraVoice = FindObjectOfType<AgoraVoice>();
            if (_agoraVoice != null)
            {
                _agoraVoice.LocalPlayerJoinChannel += OnAgoraLocalPlayerJoinChannel;
            }
        }

        private void OnDestroy()
        {
            if (_agoraVoice != null)
            {
                _agoraVoice.LocalPlayerJoinChannel -= OnAgoraLocalPlayerJoinChannel;
            }
        }

        private void OnAgoraLocalPlayerJoinChannel(uint obj)
        {
            float volume = _agoraVoice.CurrentAudioChannelFullName == _agoraVoice.DefaultAudioChannelFullName ? VOLUME : 0;
            _audio.volume = volume;
        }

        private void Cashing()
        {
            if (_audio == null)
            {
                _audio = GetComponent<AudioSource>();
            }
            if (_agent == null)
            {
                _agent = GetComponentInParent<NavMeshAgent>();
            }
        }

        private void OnEnable() => StartCoroutine(LiteUpdate());
        private void OnDisable() => StopAllCoroutines();

        public void Play()
        {
            if (!_audio.isPlaying)
            {
                _audio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                _audio.Play();
            }
        }
        public void Stop()
        {
            if (_audio.isPlaying)
            {
                _audio.Stop();
            }
        }

        IEnumerator LiteUpdate()
        {
            Vector3 currentPosition = transform.position;
            bool isMoving = false;
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(0.5f);

                if (Vector3.SqrMagnitude(currentPosition - transform.position) > SqrMinPositionShiftToPlaySound)
                {
                    if (!isMoving)
                    {
                        Play();
                        isMoving = true;
                    }
                }
                else
                {
                    isMoving = false;
                    //Stop();
                }

                currentPosition = transform.position;
            }
        }
    }
}