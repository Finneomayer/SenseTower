using UnityEngine;
using System;
using System.Collections;

namespace Sense.Interectable.Watchs
{
    public class MechanicWatch : Watch
    {

        private const float hoursrot = 360f / 12f;
        private const float minutesrot = 360f / 60f;
        private const float secondsrot = 360f / 60f;

        [SerializeField]
        private Transform _hours, _minutes, _seconds;

        protected override void UpdateTimeInfo()
        {
            DateTime time = DateTime.Now;
            if (_hours != null )
                _hours.localRotation = Quaternion.Euler(-90.0f, 0f, time.Hour * +hoursrot + 10f);

            if(_minutes != null)
                _minutes.localRotation = Quaternion.Euler(-90.0f, 0f, time.Minute * +minutesrot);

            if (_seconds != null)
                _seconds.localRotation = Quaternion.Euler(-90.0f, 0f, time.Second * +secondsrot);
          
        }
    } 
}