using System;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class PlayerTimerView : MonoBehaviour
    {
        [SerializeField]
        private GameObject TimerViewContainer;
        [SerializeField]
        private TMP_Text TimerText;

        public void SetActiveTimer(bool active)
        {
            TimerViewContainer.SetActive(active);
        }

        public void SetTimeInSeconds(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            TimerText.text = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";
        }
    }
}
