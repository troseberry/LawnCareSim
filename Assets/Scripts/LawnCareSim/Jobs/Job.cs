

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
        private List<JobTask> _tasks;

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

        public List<JobTask> Tasks
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
        }
    }
}
