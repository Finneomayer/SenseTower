using Assets.Mechanics.PadKeyboard;
using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.Stopwatch
{
    public class ClientSenseStopwatch : MonoBehaviour, INetworkCustomLogicService
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

        private Coroutine _coroutine;

        private StopwatchData _currentStopwatchData;
        private CustomBehaviourNetworkObject _customBehaviourNetworkObject;

        private void Awake()
        {
            foreach (var timeText in TimeTexts)
            {
                timeText.color = StopColor;
            }
        }

        private void OnEnable()
        {
            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
                _customBehaviourNetworkObject.StateChanged += OnStateChanged;
            }

            StartButton.OnPress += OnStartButtonPress;
        }

        private void OnDisable()
        {
            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
            }

            StartButton.OnPress -= OnStartButtonPress;
        }

        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
        {
            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
            }

            _customBehaviourNetworkObject = customBehaviourNetworkObject;

            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged += OnStateChanged;
            }
        }

        private void OnStateChanged()
        {
            string currentState = _customBehaviourNetworkObject.GetState();
            if (string.IsNullOrEmpty(currentState))
            {
                return;
            }

            StopwatchData newStopwatchData;
            try
            {
                newStopwatchData = JsonConvert.DeserializeObject<StopwatchData>(currentState);
            }
            catch (System.Exception)
            {
                Debug.Log($"no state for {gameObject.name}");
                return;
            }

            SetState(newStopwatchData);
        }

        private void SendState(StopwatchData newStopwatchData)
        {
            string jsonString = JsonConvert.SerializeObject(newStopwatchData);
            _customBehaviourNetworkObject.SetState(jsonString);
        }

        private void SendStop()
        {
            //if (_currentStopwatchData == null || !_currentStopwatchData.IsRunning)
            //{
            //    return;
            //}

            //_currentStopwatchData.IsRunning = false;
            StopwatchData stopwatchDataToSend = new();
            stopwatchDataToSend.TotalSeconds = _currentStopwatchData != null ? _currentStopwatchData.TotalSeconds : 0;
            stopwatchDataToSend.IsRunning = false;
            SendState(stopwatchDataToSend);
        }

        private void OnStartButtonPress()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
                SendStop();
                return;
            }

            _coroutine = StartCoroutine(StopWatchRunningRoutine());
        }

        private IEnumerator StopWatchRunningRoutine()
        {
            WaitForSecondsRealtime delay = new(1);
            float startTime = Time.unscaledTime;

            StopwatchData stopwatchDataToSend = new();

            while (true)
            {
                stopwatchDataToSend.TotalSeconds = (int)(Time.unscaledTime - startTime);
                stopwatchDataToSend.IsRunning = true;
                SendState(stopwatchDataToSend);
                yield return delay;
            }
        }

        private void SetState(StopwatchData stopwatchData)
        {
            if (_currentStopwatchData != null && _currentStopwatchData.IsRunning != stopwatchData.IsRunning)
            {
                if (ButtonClickSound.isPlaying)
                {
                    ButtonClickSound.Stop();
                }
                ButtonClickSound.Play();
            }

            _currentStopwatchData = stopwatchData;
            if (!_currentStopwatchData.IsRunning && _coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
                return;
            }

            RefreshView();
        }

        private void RefreshView()
        {
            if (_currentStopwatchData == null)
            {
                return;
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(_currentStopwatchData.TotalSeconds);
            foreach (var timeText in TimeTexts)
            {
                timeText.text = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";
            }

            Color newTextColor = _currentStopwatchData.IsRunning ? RunColor : StopColor;
            foreach (var timeText in TimeTexts)
            {
                if (timeText.color != newTextColor)
                {
                    timeText.color = newTextColor;
                }
            }
        }

        #region Internal class
        private sealed class StopwatchData
        {
            public int TotalSeconds;
            public bool IsRunning;
        }
        #endregion
    }
}
