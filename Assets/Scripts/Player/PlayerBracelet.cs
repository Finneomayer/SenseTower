using System;
using System.Collections;
using Assets.Scripts.Space;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Localization;

namespace Assets.Scripts.Player
{
    public enum BraceletType
    {
        Menu = 0,
        Watch = 1
    }

    public class PlayerBracelet : MonoBehaviour
    {
        #region Inspector
        public BraceletType BraceletType => _braceletType;
        [SerializeField] private SampleAvatarEntity _sampleAvatarEntity;
        public UIVisibility BraceleteUI;
        [HideInInspector] public Button MainButton;
        [HideInInspector][SerializeField] private RectTransform _menu;
        [HideInInspector][SerializeField] private RectTransform _watch;
        [HideInInspector][SerializeField] private BraceletType _braceletType = BraceletType.Menu;
        [HideInInspector][SerializeField] private TMP_Text _time;
        [HideInInspector][SerializeField] private TMP_Text _data;
        [HideInInspector][SerializeField] private TMP_Text _day;

        #endregion

        private void Start()
        {
            if (_braceletType == BraceletType.Menu)
            {
                _menu.gameObject.SetActive(true);
            }
            else if (_braceletType == BraceletType.Watch)
            {
                _watch.gameObject.SetActive(true);
                if (_time != null) StartCoroutine(TickTime(_time));
                if (_day != null & _data != null) StartCoroutine(TickData());
            }
        }

        private IEnumerator TickTime(TMP_Text time)
        {
            while (true)
            {
                //time
                string minutes = DateTime.Now.Minute.ToString();
                if ((int)DateTime.Now.Minute < 10) minutes = "0" + DateTime.Now.Minute.ToString();
                string hours = DateTime.Now.Hour.ToString();
                if ((int)DateTime.Now.Hour < 10) hours = "0" + DateTime.Now.Hour.ToString();
                time.text = $"{hours}:{minutes}";
                yield return new WaitForSeconds(1f);
                time.text = $"{hours}<alpha=#00>:</color>{minutes}";
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator TickData()
        {
            while (true)
            {
                //data
                string data = DateTime.Now.Day.ToString();
                string month = DateTime.Now.Month.ToString();
                string year = DateTime.Now.Year.ToString().Substring(2);

                if (data.Length == 1) data = "0" + data;
                if (month.Length == 1) month = "0" + month;

                _data.text = $"{data}.{month}.{year}";

                //day
                _day.text = LocalizationManager.GetCurrentCultureInfo().DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek).ToLower();
                yield return new WaitForSeconds(60f);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
