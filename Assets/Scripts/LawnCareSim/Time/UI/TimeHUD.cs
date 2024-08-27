
using Core.Utility;
using LawnCareSim.Events;
using System;
using TMPro;
using UnityEngine;

namespace LawnCareSim.Time
{
    public class TimeHUD : MonoBehaviour
    {
        private TextMeshProUGUI _dateText;

        private TextMeshProUGUI _timeOfDayText;
        private TextMeshProUGUI _dayOfTheWeekText;
        private TextMeshProUGUI _dayOfTheMonthText;

        private string _dayOfWeek;
        private string _dayOfMonth;

        private void Start()
        {
            UIHelpers.SetUpUIElement(transform, ref _dateText, "DateText");

            UIHelpers.SetUpUIElement(transform, ref _timeOfDayText, "TimeOfDayText");
            //UIHelpers.SetUpUIElement(transform, ref _dayOfTheWeekText, "DayOfTheWeekText");
            //UIHelpers.SetUpUIElement(transform, ref _dayOfTheMonthText, "DayOfTheMonthText");

            EventRelayer.Instance.TimeOfDayChangedEvent += TimeOfDayChangedEventListener;
            EventRelayer.Instance.DayChangedEvent += DayChangedEventListener;
            EventRelayer.Instance.DayInMonthChangedEvent += DayInMonthChangedEventListener;

            _dayOfWeek = TimeManager.Instance.DayOfTheWeek.ToString();
            _dayOfMonth = TimeManager.Instance.GetDate();

            _dateText.text = $"{_dayOfWeek} the {_dayOfMonth}";

            _timeOfDayText.text = $"{TimeManager.Instance.TimeOfDay}";
            //_dayOfTheWeekText.text = $"{TimeManager.Instance.DayOfTheWeek}";
            //_dayOfTheMonthText.text = $"{TimeManager.Instance.GetDate()}";
        }

        private void TimeOfDayChangedEventListener(object sender, TimeOfDay time)
        {
            _timeOfDayText.text = $"{time}";
        }

        private void DayChangedEventListener(object sender, Day day)
        {
            //_dayOfTheWeekText.text = $"{day}";

            _dayOfWeek = day.ToString();
            _dateText.text = $"{_dayOfWeek} the {_dayOfMonth}";
        }

        private void DayInMonthChangedEventListener(object sender, string date)
        {
            //_dayOfTheMonthText.text = $"{date}";

            _dayOfMonth = date;
            _dateText.text = $"{_dayOfWeek} the {_dayOfMonth}";
        }
    }
}
