
using Core.Utility;
using LawnCareSim.Events;
using LawnCareSim.Jobs.UI;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    public class TaskHUD : MonoBehaviour
    {
        [SerializeField] private Animator _totalProgressRadialAnimator;
        [SerializeField] private Transform _tasksList;
        private TextMeshProUGUI _totalProgressPercentageText;
        

        private void Start()
        {
            EventRelayer.Instance.ActiveJobSetEvent += ActiveJobSetEventListener;
            EventRelayer.Instance.ActiveJobProgressedEvent += ActiveJobProgressedEventListener;

            UIHelpers.SetUpUIElement(transform, ref _totalProgressPercentageText, "TotalProgressPercentageText");

            _totalProgressPercentageText.text = "";
            _totalProgressRadialAnimator.Play("TotalProgressRadial", -1, 0);
        }

        private void ActiveJobSetEventListener(object sender, Job job)
        {
            _totalProgressRadialAnimator.Play("TotalProgressRadial", -1, job.TotalProgress);
            _totalProgressPercentageText.text = $"{Mathf.FloorToInt(job.TotalProgress * 100)}";

            var jobTasks = job.Tasks.Values.ToArray();
            for (int i = 0; i < _tasksList.childCount; i++)
            {
                if (i < jobTasks.Length)
                {
                    var taskGO = _tasksList.GetChild(i).gameObject;
                    taskGO.SetActive(true);
                    taskGO.GetComponent<JobTaskUIComponent>().BackingData = jobTasks[i];
                    taskGO.GetComponent<JobTaskUIComponent>().SubscribeToEvents();
                }
                else
                {
                    _tasksList.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        #region Event Listeners
        private void ActiveJobProgressedEventListener(object sender, Job job)
        {
            SetProgress(job.TotalProgress);
        }
        #endregion

        private void SetProgress(float progress)
        {
            _totalProgressRadialAnimator.Play("TotalProgressRadial", -1, progress);
            _totalProgressPercentageText.text = $"{Mathf.FloorToInt(progress * 100)}";
        }
    }
}
