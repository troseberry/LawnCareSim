using Core.GameFlow;
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
                    CreateJobForLayout(1, layout);
                }
            }
        }

        public void InitializeManager()
        {
            _jobsMap = new Dictionary<Guid, Job>();

            EventRelayer.Instance.LawnGeneratedEvent += LawnGeneratedEventListener;
        }

        private void LawnGeneratedEventListener(object sender, Job job)
        {
            if (_jobsMap.ContainsKey(job.Guid))
            {
                var modJob = job;
                modJob.Tasks = new List<JobTask>();

                if (modJob.Difficulty >= 1)
                {
                    modJob.Tasks.Add(new JobTask(JobTaskType.CutGrass, modJob.GrassArea));
                }

                if (modJob.Difficulty >= 2)
                {
                    modJob.Tasks.Add(new JobTask(JobTaskType.EdgeYard, modJob.Edges));
                }

                if (modJob.Difficulty >= 3)
                {
                    modJob.Tasks.Add(new JobTask(JobTaskType.StripeGrass, modJob.GrassArea));
                }

                _jobsMap[job.Guid] = modJob;
                EventRelayer.Instance.OnJobTasksCreatedEvent(modJob);

                foreach(var task in modJob.Tasks)
                {
                    Debug.Log(task);
                }
            }
        }

        // Job menu is populated with images of the layouts with random difficulties. On load into scene create the job here
        private void CreateJobForLayout(int difficulty, JobLayout layout)
        {
            Job newJob = new Job(difficulty, layout);
            _jobsMap.Add(newJob.Guid, newJob);

            EventRelayer.Instance.OnJobCreated(newJob);
        }
    }
}
