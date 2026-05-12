using UnityEngine;
using Oculus.Avatar2;
using Assets.Scripts.Audio;
using agora_gaming_rtc;
using System;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public class MetaAvatarLipsyncInput : MonoBehaviour
    {
        [SerializeField]
        private OvrAvatarLipSyncContext LipSyncContext;

        private AgoraVoice _agoraVoice;

        private void Awake()
        {
            _agoraVoice = FindObjectOfType<AgoraVoice>();
        }

        private void OnEnable()
        {
            if (_agoraVoice != null)
            {
                _agoraVoice.AudioFrameRecorded += OnAgoraAudioFrameRecorded;
            }
        }

        private void OnDisable()
        {
            if (_agoraVoice != null)
            {
                _agoraVoice.AudioFrameRecorded -= OnAgoraAudioFrameRecorded;
            }
        }

        private void OnAgoraAudioFrameRecorded(AudioFrame audioFrame)
        {
            if (audioFrame.buffer == null)
            {
                return;
            }
            float[] floatData = ConvertByteToFloat(audioFrame.buffer);

            LipSyncContext.AudioSampleRate = audioFrame.samplesPerSec;
            LipSyncContext.ProcessAudioSamples(floatData, audioFrame.channels);
        }

        private static float[] ConvertByteToFloat(byte[] array)
        {
            float[] floatArr = new float[array.Length / 2];

            for (int i = 0; i < floatArr.Length; i++)
            {
                floatArr[i] = ((float)BitConverter.ToInt16(array, i * 2)) / 32768.0f;
            }

            return floatArr;
        }
    }
}
