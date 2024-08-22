using System;
namespace LawnCareSim.Jobs
{
    public enum JobTaskType
    {
        Invalid,
        CutGrass,
        StripeGrass,
        EdgeYard
    }
    public class JobTask
    {
        private readonly JobTaskType _type;
        private readonly int _instances;

        private int _progressCounter;

        public float Progress => _progressCounter / _instances;

        public JobTask(JobTaskType type, int instances)
        {
            _type = type;
            _instances = instances;
        }

        public override string ToString()
        {
            return $"{_type}: {_instances}";
        }
    }
}
