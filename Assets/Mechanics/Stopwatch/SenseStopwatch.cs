using Assets.Mechanics.PadKeyboard;
using System;
using System.Collections;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.Stopwatch
{
    public class SenseStopwatch : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] TimeTexts;
        [SerializeField]
        private FingerPhysicButton StartButton;
        [SerializeField]
        private AudioSource ButtonClickSound;
        [SerializeField]
        private Color StopColor = Color.gray;
        [SerializeField]
        private Color RunColor = Color.green;

        private float _startTime;
        private Coroutine _coroutine;

        private void Awake()
        {
            foreach (var timeText in TimeTexts)
            {
                timeText.color = StopColor;
            }
        }

        private void OnEnable()
        {
            StartButton.OnPress += OnStartButtonPress;
        }

        private void OnDisable()
        {
            StartButton.OnPress += OnStartButtonPress;
        }

        private void OnStartButtonPress()
        {
            if (ButtonClickSound.isPlaying)
            {
                ButtonClickSound.Stop();
            }
            ButtonClickSound.Play();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
                foreach (var timeText in TimeTexts)
                {
                    timeText.color = StopColor;
                }
                return;
            }

            _coroutine = StartCoroutine(StopWatchRunningRoutine());
            _startTime = Time.time;
        }

        private IEnumerator StopWatchRunningRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(1);
            _startTime = Time.time;

            foreach (var timeText in TimeTexts)
            {
                timeText.color = RunColor;
            }

            while (true)
            {
                RefreshClock(Time.time - _startTime);
                yield return delay;
            }
        }

        private void RefreshClock(float secondsPassed)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(secondsPassed);
            foreach (var timeText in TimeTexts)
            {
                timeText.text = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";
            }
        }
    }
}
