using Assets.Mechanics.PadKeyboard;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Stopwatch
{
    public class NetworkSenseStopwatch : NetworkBehaviour
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

        private Coroutine _stopwatchCoroutine;
        private StopwatchData _currentStopwatchData;

        private void Awake()
        {
            _currentStopwatchData = new()
            {
                IsRunning = false,
                TotalSeconds = 0,
            };

            RefreshView(new StopwatchData());
        }

        private void OnEnable()
        {
            StartButton.OnPress += OnStartButtonPress;
        }

        private void OnDisable()
        {
            StartButton.OnPress -= OnStartButtonPress;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient)
            {
                RequestDataServerRpc(NetworkManager.LocalClientId);
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void RequestDataServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] { clientId };
            SendInitialStopwatchDataClientRpc(_currentStopwatchData, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendButtonPressServerRpc()
        {
            if (_stopwatchCoroutine != null)
            {
                StopCoroutine(_stopwatchCoroutine);
                _stopwatchCoroutine = null;
            }

            if (_currentStopwatchData.IsRunning)
            {
                _currentStopwatchData.IsRunning = false;
                SendStopwatchDataClientRpc(_currentStopwatchData);
            }
            else
            {
                _stopwatchCoroutine = StartCoroutine(StopWatchRunningRoutineServer());
            }
        }

        [ClientRpc]
        private void SendStopwatchDataClientRpc(StopwatchData stopwatchData, ClientRpcParams clientRpcParams = default)
        {
            if (_currentStopwatchData.IsRunning != stopwatchData.IsRunning)
            {
                if (ButtonClickSound.isPlaying)
                {
                    ButtonClickSound.Stop();
                }
                ButtonClickSound.Play();
            }

            _currentStopwatchData = stopwatchData;
            RefreshView(_currentStopwatchData);
        }

        [ClientRpc]
        private void SendInitialStopwatchDataClientRpc(StopwatchData stopwatchData, ClientRpcParams clientRpcParams = default)
        {
            _currentStopwatchData = stopwatchData;
            RefreshView(_currentStopwatchData);
        }

        private IEnumerator StopWatchRunningRoutineServer()
        {
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1);
            float startTime = Time.unscaledTime;
            _currentStopwatchData.IsRunning = true;

            while (true)
            {
                _currentStopwatchData.TotalSeconds = (int)(Time.unscaledTime - startTime);
                SendStopwatchDataClientRpc(_currentStopwatchData);
                yield return delay;
            }
        }

        private void RefreshView(StopwatchData stopwatchData)
        {
            if (stopwatchData == null)
            {
                return;
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(stopwatchData.TotalSeconds);
            foreach (var timeText in TimeTexts)
            {
                timeText.text = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";
            }

            Color newTextColor = stopwatchData.IsRunning ? RunColor : StopColor;
            foreach (var timeText in TimeTexts)
            {
                if (timeText.color != newTextColor)
                {
                    timeText.color = newTextColor;
                }
            }
        }

        private void OnStartButtonPress()
        {
            SendButtonPressServerRpc();
        }

        #region Internal class
        private sealed class StopwatchData : INetworkSerializable
        {
            public int TotalSeconds;
            public bool IsRunning;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref TotalSeconds);
                serializer.SerializeValue(ref IsRunning);
            }
        }
        #endregion
    }
}
