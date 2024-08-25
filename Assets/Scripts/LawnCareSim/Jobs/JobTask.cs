using System;
namespace LawnCareSim.Jobs
{
    public enum JobTaskType
    {
        Invalid,
        CutGrass,
        StripeGrass,
        EdgeGrass
    }
    public class JobTask
    {
        private readonly JobTaskType _taskType;
        private readonly int _instances;

        private int _progressCounter;

        public float Progress => (float)_progressCounter / _instances;

        public JobTaskType TaskType => _taskType;

        public JobTask(JobTaskType taskType, int instances)
        {
            _taskType = taskType;
            _instances = instances;
        }

        public string GetName()
        {
            switch (_taskType)
            {
                case JobTaskType.CutGrass:
                    return "Cut Grass";
                case JobTaskType.EdgeGrass:
                    return "Edge Grass";
                case JobTaskType.StripeGrass:
                    return "Stripe Grass";
            }

            return "";
        }

        public override string ToString()
        {
            return $"{_taskType}: {_instances}";
        }

        public bool ProgressTask()
        {
            if (_progressCounter >= _instances)
            {
                return false;
            }

            _progressCounter++;
            return true;
        }
    }
}
