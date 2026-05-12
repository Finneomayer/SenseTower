using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Sense.Interectable.Watchs
{
	public class ModernWatch : Watch
	{
		[SerializeField]
		private TextMeshPro _time;

		[SerializeField]
		private TextMeshPro _day;

		[SerializeField]
		private TextMeshPro _data;

		[SerializeField]
		private TextMeshPro _battary;

		private string[] ShortestDayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

		protected override void UpdateTimeInfo()
		{
			System.DateTime dataInfo = System.DateTime.Now;

			if (_time != null)
			//	_time.text = $"<color=#ADDBF4> {dataInfo.Hour}:<color=#FFFDCD>{dataInfo.Minute}";
				_time.text = dataInfo.ToString("HH:mm"); ;

			if (_day != null)
				_day.text = ShortestDayNames[(int)dataInfo.DayOfWeek];
				//_day.text = dataInfo.DayOfWeek.ToString();


			if (_data != null)
				//_data.text = dataInfo.ToString("dd/MM/yyyy");
				_data.text = dataInfo.ToString("dd");

			if (_battary != null)
				_battary.text = $"{ Mathf.Round(SystemInfo.batteryLevel * 100) } %";

		}
	}
}