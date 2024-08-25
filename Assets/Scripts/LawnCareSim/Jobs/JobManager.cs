﻿using Core.GameFlow;
using LawnCareSim.Data;
using LawnCareSim.Events;
using LawnCareSim.Grass;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    public class JobManager : MonoBehaviour, IManager
    {
        public static JobManager Instance;
        public bool ShowDebugGUI;

        private Dictionary<Guid, Job> _jobsMap;
        private Job _activeJob;

        public Job ActiveJob
        {
            get => _activeJob;
            private set
            {
                _activeJob = value;
                if (value != null)
                {
                    EventRelayer.Instance.OnActiveJobSetEvent(_activeJob);
                }
            }
        }

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

            Rect mainRect = new Rect(width * 0.02f, height * 0.04f, 360, 200);
            GUI.Box(mainRect, GUIContent.none);

            if (GUI.Button(new Rect(mainRect.x + 20, mainRect.y + 25, 150, 150), "Start Job"))
            {
                if (MasterDataManager.Instance.JobDataManager.GetJobLayout("JobLayout_01", out var layout))
                {
                    ActiveJob = CreateJobForLayout(1, layout);
                }
            }
        }

        public void InitializeManager()
        {
            _jobsMap = new Dictionary<Guid, Job>();

            EventRelayer.Instance.LawnGeneratedEvent += LawnGeneratedEventListener;
            EventRelayer.Instance.GrassCutEvent += GrassCutEventListener;
            EventRelayer.Instance.GrassEdgedEvent += GrassEdgedEventListener;
            EventRelayer.Instance.GrassStripedEvent += GrassStripedEventListener;
        }

        #region Event Listeners
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
                EventRelayer.Instance.OnJobTasksCreatedEvent(modJob);

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

        // Job menu is populated with images of the layouts with random difficulties. On load into scene create the job here
        private Job CreateJobForLayout(int difficulty, JobLayout layout)
        {
            Job newJob = new Job(difficulty, layout);
            _jobsMap.Add(newJob.Guid, newJob);

            EventRelayer.Instance.OnJobCreated(newJob);

            return newJob;
        }
    }
}
