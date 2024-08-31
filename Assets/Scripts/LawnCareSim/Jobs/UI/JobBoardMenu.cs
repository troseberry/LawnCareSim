using LawnCareSim.Events;
using LawnCareSim.UI;
using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    public class JobBoardMenu : BaseMenu
    {
        public static JobBoardMenu Instance;

        #region Private Vars
        private bool _refreshJobPosts;
        private List<PotentialJobUIComponent> _potentialJobsList;
        #endregion

        #region Serialized Vars
        [SerializeField] private Transform _jobsList;

        #endregion

        private void Awake()
        {
            Instance = this;
        }

        public override void InitializeMenuView()
        {
            base.InitializeMenuView();

            _refreshJobPosts = true;
            SetupObjectPool();

            EventRelayer.Instance.DayChangedEvent += DayChangedEventListener;
        }

        private void DayChangedEventListener(object sender, Time.Day args)
        {
            _refreshJobPosts = true;
        }

        public override void Open()
        {
            base.Open();

            if (_refreshJobPosts)
            {
                CreateJobPosts();
            }
        }

        private void SetupObjectPool()
        {
            _potentialJobsList = new List<PotentialJobUIComponent>();
            foreach(Transform child in _jobsList)
            {
                _potentialJobsList.Add(child.GetComponent<PotentialJobUIComponent>());
            }
        }

        private void CreateJobPosts()
        {
            var dailyJobs = JobManager.Instance.DailyJobs;

            for(int i = 0; i < dailyJobs.Count; i++)
            {
                _potentialJobsList[i].BackingData = dailyJobs[i];
            }

            // Hide unused job items
            if (_potentialJobsList.Count > dailyJobs.Count)
            {
                for (int j = dailyJobs.Count; j < _potentialJobsList.Count; j++)
                {
                    _potentialJobsList[j].gameObject.SetActive(false);
                    _potentialJobsList[j].Clear();
                }
            }

            _refreshJobPosts = false;
        }
    }
}
