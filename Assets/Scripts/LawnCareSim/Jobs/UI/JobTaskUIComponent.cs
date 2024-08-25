using LawnCareSim.Events;
using LawnCareSim.Gear;
using LawnCareSim.UI;
using TMPro;
using Unity.Jobs;
using UnityEngine;

namespace LawnCareSim.Jobs.UI
{
    public class JobTaskUIComponent : BaseMenuUIComponent
    {
        [SerializeField] private TextMeshProUGUI _taskNameText;
        [SerializeField] private TextMeshProUGUI _taskPercentageText;

        private JobTask _backingData;
        private float _taskProgress;

        public override object BackingData
        {
            get => _backingData;
            set
            {
                _backingData = (JobTask)value;
                UpdateInterface();
            }
        }

        public void SubscribeToEvents()
        {
            EventRelayer.Instance.ActiveJobTaskProgressedEvent += OnActiveJobTaskProgressedEventListener;
        }

        public void UnsubscribeFromEvents()
        {
            EventRelayer.Instance.ActiveJobTaskProgressedEvent -= OnActiveJobTaskProgressedEventListener;
        }

        private void OnActiveJobTaskProgressedEventListener(object sender, JobTask task)
        {
            if (task.TaskType == _backingData.TaskType)
            {
                _taskProgress = task.Progress;
                _taskPercentageText.text = $"{Mathf.FloorToInt(_taskProgress * 100)}";
            }
        }

        public override void UpdateInterface()
        {
            _taskNameText.text = _backingData.GetName();
            _taskNameText.enabled = true;

            _taskProgress = _backingData.Progress;
            _taskPercentageText.text = $"{Mathf.FloorToInt(_taskProgress * 100)}";
            _taskPercentageText.enabled = true;
        }

        public override void Clear(bool clearBackingData)
        {
            base.Clear(clearBackingData);

            _taskNameText.enabled = false;
            _taskNameText.text = "";

            _taskPercentageText.enabled = false;
            _taskPercentageText.text = "";
        }
    }
}
