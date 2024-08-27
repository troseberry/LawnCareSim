using Codice.CM.Common.Merge;
using Core.GameFlow;
using LawnCareSim.Events;
using LawnCareSim.Gear;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace LawnCareSim.Time
{
    public partial class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;
        public bool ShowDebugGUI;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeManager();
        }

        private void OnGUI()
        {
            if (!ShowDebugGUI)
            {
                return;
            }

            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            Rect mainRect = new Rect(width * 0.82f, height * 0.04f, 300, 200);
            GUI.Box(mainRect, GUIContent.none);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y, 250, 30), $"Total Days: {_totalDayCount}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 30, 250, 30), $"Time of Day: {_timeOfDay}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 60, 250, 30), $" Day of Week: {_dayOfTheWeek}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 90, 250, 30), $"Day of Month: {_dayInMonth}", fontStyle);
        }

        public void InitializeManager()
        {
            EventRelayer.Instance.TimeProgressingActionPerformedEvent += TimeProgressingActionPerformedEventListener;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                ProgressTime(1);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                ProgressTime(2);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                ProgressTime(3);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                ProgressTime(4);
            }
        }

        #region Event Listeners
        private void TimeProgressingActionPerformedEventListener(object sender, int args)
        {
            ProgressTime(args);
        }
        #endregion
    }

    public partial class TimeManager : IManager
    {
        private TimeOfDay _timeOfDay = TimeOfDay.Morning;
        private Day _dayOfTheWeek = Day.Sunday;
        private int _dayInMonth = 0;
        private int _totalDayCount = 0;

        public TimeOfDay TimeOfDay
        {
            get => _timeOfDay;
            private set
            {
                if (value != TimeOfDay.Invalid)
                {
                    _timeOfDay = value;
                    EventRelayer.Instance.OnTimeOfDayChanged(_timeOfDay);
                }
            }
        }

        public Day DayOfTheWeek
        {
            get => _dayOfTheWeek;
            private set
            {
                if (value != Day.Invalid)
                {
                    _dayOfTheWeek = value;
                    EventRelayer.Instance.OnDayChanged(_dayOfTheWeek);
                }
            }
        }

        public int DayInMonth
        {
            get => _dayInMonth;
            set
            {
                if (value - _dayInMonth < 0)
                {
                    EventRelayer.Instance.OnMonthCycleCompleted();
                }

                _dayInMonth = value;
                EventRelayer.Instance.OnDayInMonthChangedEvent(GetDate());
            }
        }

        public int TotalDayPassed
        {
            get => _totalDayCount;
            private set
            {
                if (value <= 0)
                {
                    return;
                }

                _totalDayCount = value;
            }
        }

        private void ProgressTime(int timeSegments)
        {
            if (timeSegments <= 0)
            {
                return;
            }

            int currentTime = (int)_timeOfDay;

            int newTime = (currentTime + timeSegments) % 4;
            TimeOfDay = (TimeOfDay)newTime;

            int daysPassed = (currentTime + timeSegments) / 4;

            if (daysPassed <= 0)
            {
                return;
            }

            TotalDayPassed += daysPassed;

            int dayOfWeek = (int)_dayOfTheWeek;
            dayOfWeek = (dayOfWeek + daysPassed) % 7;
            DayOfTheWeek = (Day)dayOfWeek;

            int dayOfMonth = _dayInMonth;
            dayOfMonth = (dayOfMonth + daysPassed) % 30;
            DayInMonth = dayOfMonth;
        }

        public string GetDate()
        {
            int number = _dayInMonth + 1;

            string suffix;
            switch (number)
            {
                case 1:
                case 21:
                    suffix = "st";
                    break;

                case 2:
                case 22:
                    suffix = "nd";
                    break;

                case 3:
                case 23:
                    suffix = "rd";
                    break;

                default:
                    suffix = "th";
                    break;
            }

            return $"{number}{suffix}";
        }

        public string GetFullDate()
        {
            // i.e. Sunday the 3rd
            return "";
        }
    }
}
