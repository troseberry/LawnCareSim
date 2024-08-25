

using System;
using System.Collections.Generic;

namespace LawnCareSim.Jobs
{
    public class Job
    {
        private readonly Guid _guid;
        private readonly int _difficulty;
        private readonly JobLayout _layout;

        private int _grassArea;
        private int _edges;
        private float _totalProgress;
        private Dictionary<JobTaskType, JobTask> _tasks;

        public Guid Guid => _guid;
        public int Difficulty => _difficulty;

        public JobLayout Layout => _layout;

        public int GrassArea
        {
            get => _grassArea;
            set => _grassArea = value;
        }

        public int Edges
        {
            get => _edges;
            set => _edges = value;
        }

        public float TotalProgress
        {
            get => _totalProgress;
        }

        public Dictionary<JobTaskType, JobTask> Tasks
        {
            get => _tasks;
            set
            {
                if (value != null)
                {
                    _tasks = value;
                }
            }
        }

        public Job(int difficulty, JobLayout layout)
        {
            _guid = Guid.NewGuid();

            _difficulty = difficulty;
            _layout = layout;

            _totalProgress = 0f;
        }

        private bool GetTaskForType(JobTaskType type, out JobTask task)
        {
            task = null;

            return _tasks.TryGetValue(type, out task);
        }

        internal bool ProgressTask(JobTaskType type)
        {
            if (!GetTaskForType(type, out var foundTask))
            {
                return false;
            }

            if (!foundTask.ProgressTask())
            {
                return false;
            }

            // update total progress
            float total = 0;

            foreach(var task in _tasks)
            {
                total += task.Value.Progress;
            }

            _totalProgress = total / _tasks.Count;

            return true;
        }
    }
}
