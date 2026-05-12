using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Assets.Scripts.Data;

namespace Assets.Mechanics.TextToSpeech
{
    public class SimpleTextToSpeechPlayer : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup ControlPanelCanvasGroup;
        [SerializeField]
        private AudioSource AudioSource;
        [SerializeField]
        private Button PlayButton;
        [SerializeField]
        private Button StopButton;

        private string _text;

        private void Awake()
        {
            SetPlayingMode(false);
        }

        private void OnEnable()
        {
            PlayButton.onClick.AddListener(OnPlayButtonClick);
            StopButton.onClick.AddListener(OnStopButtonClick);
        }

        private void OnDisable()
        {
            PlayButton.onClick.RemoveListener(OnPlayButtonClick);
            StopButton.onClick.RemoveListener(OnStopButtonClick);
        }

        public void Init(IEnumerable<string> textBlocks)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendJoin(" sil<[300]> ", textBlocks.StringsConvertionToYandexSpeach());

            Init(stringBuilder.ToString());
        }

        public void Init(string text)
        {
            _text = text.StringConvertionToYandexSpeach();
            SetControlPanelInteractable(!string.IsNullOrEmpty(_text));
        }

        private void OnPlayButtonClick()
        {
            ProcessVoiceActing().Forget();
        }

        private void OnStopButtonClick()
        {
            InterruptVoiceActing();
        }

        private void SetControlPanelInteractable(bool interactable)
        {
            ControlPanelCanvasGroup.alpha = interactable ? 1 : 0.3f;
            PlayButton.interactable = interactable;
            StopButton.interactable = interactable;
        }

        private void InterruptVoiceActing()
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }
        }

        private async UniTask ProcessVoiceActing()
        {
            if (string.IsNullOrEmpty(_text))
            {
                return;
            }

            SetControlPanelInteractable(false);

            AudioClip clip = await TextToSpeechConverter.SynthesizeAudioClip(_text);

            SetControlPanelInteractable(true);

            if (clip != null)
            {
                PlayClip(clip);
                SetPlayingMode(true);

                await UniTask.WaitWhile(() => AudioSource.isPlaying);
            }

            SetPlayingMode(false);
        }

        private void SetPlayingMode(bool isPlaying)
        {
            PlayButton.gameObject.SetActive(!isPlaying);
            StopButton.gameObject.SetActive(isPlaying);
        }

        private void PlayClip(AudioClip clip)
        {
            InterruptVoiceActing();
            AudioSource.clip = clip;
            AudioSource.Play();
        }
    }
}
