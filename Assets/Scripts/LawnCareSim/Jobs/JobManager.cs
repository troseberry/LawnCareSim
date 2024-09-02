using Core.GameFlow;
using LawnCareSim.Data;
using LawnCareSim.Events;
using LawnCareSim.Grass;
using LawnCareSim.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    public class JobManager : MonoBehaviour, IManager
    {
        #region Singleton
        public static JobManager Instance;
        #endregion

        #region Private Variables
        private bool _needToGenerateDailyJobs;

        private JobDataManager _jobDataManager;
        private Dictionary<Guid, Job> _jobsMap;
        private Job _activeJob;
        private List<Job> _dailyJobs;
        #endregion

        #region Properties
        public Job ActiveJob
        {
            get => _activeJob;
            internal set
            {
                _activeJob = value;

                if (_activeJob != null)
                {
                    MenuManager.Instance.ShowHUDElement(HUDName.ActiveJob);
                    EventRelayer.Instance.OnActiveJobSelected(_activeJob);
                }
                else
                {
                    MenuManager.Instance.HideHUDElement(HUDName.ActiveJob);
                    EventRelayer.Instance.OnActiveJobCleared();
                }
            }
        }

        internal List<Job> DailyJobs
        {
            get
            {
                if (_needToGenerateDailyJobs)
                {
                    GenerateNewDailyJobs();
                }

                return _dailyJobs;
            }
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeManager();
        }
        #endregion

        #region IManager
        public void InitializeManager()
        {
            _needToGenerateDailyJobs = true;
            _jobsMap = new Dictionary<Guid, Job>();
            _jobDataManager = MasterDataManager.Instance.JobDataManager;

            EventRelayer.Instance.LawnGeneratedEvent += LawnGeneratedEventListener;
            EventRelayer.Instance.GrassCutEvent += GrassCutEventListener;
            EventRelayer.Instance.GrassEdgedEvent += GrassEdgedEventListener;
            EventRelayer.Instance.GrassStripedEvent += GrassStripedEventListener;

            EventRelayer.Instance.DayChangedEvent += DayChangedEventListener;
        }
        #endregion

        #region Event Listeners
        private void DayChangedEventListener(object sender, Time.Day args)
        {
            if (!_needToGenerateDailyJobs)
            {
                _needToGenerateDailyJobs = true;
            }
        }
        private void LawnGeneratedEventListener(object sender, Job job)
        {
            if (_jobsMap.ContainsKey(job.Guid))
            {
                var modJob = job;
                modJob.Tasks = new Dictionary<JobTaskType, JobTask>();

                if (modJob.Difficulty >= 1)
                {
                    modJob.Tasks.Add(JobTaskType.CutGrass, new JobTask(JobTaskType.CutGrass, modJob.GrassArea));
                }

                if (modJob.Difficulty >= 2)
                {
                    modJob.Tasks.Add(JobTaskType.EdgeGrass, new JobTask(JobTaskType.EdgeGrass, modJob.Edges));
                }

                if (modJob.Difficulty >= 3)
                {
                    modJob.Tasks.Add(JobTaskType.StripeGrass, new JobTask(JobTaskType.StripeGrass, modJob.GrassArea));
                }

                _jobsMap[job.Guid] = modJob;
                EventRelayer.Instance.OnJobTasksCreated(modJob);

                foreach(var task in modJob.Tasks)
                {
                    Debug.Log(task);
                }
            }
        }

        private void GrassCutEventListener(object sender, EventArgs args)
        {
            if (_activeJob.ProgressTask(JobTaskType.CutGrass, out var task))
            {
                EventRelayer.Instance.OnActiveJobProgressed(_activeJob);
                EventRelayer.Instance.OnActiveJobTaskProgressed(task);
            }
        }

        private void GrassEdgedEventListener(object sender, EventArgs args)
        {
            if (_activeJob.ProgressTask(JobTaskType.EdgeGrass, out var task))
            {
                EventRelayer.Instance.OnActiveJobProgressed(_activeJob);
                EventRelayer.Instance.OnActiveJobTaskProgressed(task);
            }
        }

        private void GrassStripedEventListener(object sender, EventArgs args)
        {
            if (_activeJob.ProgressTask(JobTaskType.StripeGrass, out var task))
            {
                EventRelayer.Instance.OnActiveJobProgressed(_activeJob);
                EventRelayer.Instance.OnActiveJobTaskProgressed(task);
            }
        }
        #endregion

        #region Job Methods
        // Job menu is populated with images of the layouts with random difficulties. On load into scene create the job here
        private Job CreateJobForLayout(int difficulty, JobLayout layout)
        {
            Job newJob = new Job(difficulty, layout);
            _jobsMap.Add(newJob.Guid, newJob);

            EventRelayer.Instance.OnJobCreated(newJob);

            return newJob;
        }

        private void GenerateNewDailyJobs()
        {
            _dailyJobs = new List<Job>();

            // TO-DO: Should get random layouts
            _jobDataManager.GetJobLayout("JobLayout_01", out var layout);

            for (int i = 0; i < 15; i++)
            {
                var newJob = new Job(UnityEngine.Random.Range(1, 5), layout);
                _dailyJobs.Add(newJob);
            }

            _needToGenerateDailyJobs = false;
        }
        #endregion


        #region Debug
        public bool ShowDebugGUI;

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

            Rect mainRect = new Rect(width * 0.02f, height * 0.04f, 360, 200);
            GUI.Box(mainRect, GUIContent.none);

            if (GUI.Button(new Rect(mainRect.x + 20, mainRect.y + 25, 150, 150), "Start Job"))
            {
                if (_jobDataManager.GetJobLayout("JobLayout_01", out var layout))
                {
                    ActiveJob = CreateJobForLayout(1, layout);
                }
            }

            if (GUI.Button(new Rect(mainRect.x + 200, mainRect.y + 25, 150, 150), "Job Board"))
            {
                MenuManager.Instance.ToggleMenu(MenuName.JobBoard);
            }

            if (GUI.Button(new Rect(mainRect.x + 380, mainRect.y + 25, 150, 150), "Clear Active"))
            {
                ActiveJob = null;
            }
        }
        #endregion

    }
}
