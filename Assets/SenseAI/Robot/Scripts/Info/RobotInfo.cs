using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace SenseAI.Robot.UI
{
    public class RobotInfo : MonoBehaviour
    {
        [SerializeField] List<NavigationPoint> _points = new List<NavigationPoint>();
        [SerializeField] RectTransform _content;
        [SerializeField] GameObject _btnPrefab;
        private List<GameObject> _btns = new List<GameObject>();
        private void Awake()
        {
            InnitUI();
        }
        public void InnitUI()
        {
            if (_btnPrefab == null)
                return;
            if (_content == null)
                return;

            float offset = 0;
            foreach(NavigationPoint p in _points)
            {
                GameObject newBtn = GenerateNewButton(offset, p);
                _btns.Add(newBtn);
                offset -= _btnPrefab.GetComponent<RectTransform>().rect.height * 1.5f;
            }
        }

        private GameObject GenerateNewButton(float offset, NavigationPoint p)
        {
            p.InnitAndChek();
            var newBtn = Instantiate(_btnPrefab, _content);
            newBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset);
            newBtn.GetComponentInChildren<TextMeshProUGUI>().text = p._pointName;
            newBtn.GetComponent<RoomButtonController>().ApplyTarget(p._pointTransform);
            return newBtn;
        }
    }
}