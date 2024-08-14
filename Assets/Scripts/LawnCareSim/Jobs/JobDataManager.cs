using Core.Data;
using Core.DataStructures;
using LawnCareSim.Gear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    [CreateAssetMenu(fileName = "JobDataManager", menuName = "Data/Managers/JobDataManager")]
    public class JobDataManager : ScriptableObject, IDataManager, ISerializationCallbackReceiver
    {
        [SerializeField]
        private JobLayout[] _layoutEntries;

        private Dictionary<string, JobLayout> _jobMap;

        public bool GetJobLayout(string jobName, out JobLayout layout)
        {
            if (_jobMap.TryGetValue(jobName, out layout))
            {
                return true;
            }

            return false;
        }

        #region IDataManager
        public void DataArraysToDictionarys()
        {
            _jobMap = new Dictionary<string, JobLayout>();
            
            foreach (JobLayout layout in _layoutEntries)
            {
                if (!_jobMap.ContainsKey(layout.JobName))
                {
                    _jobMap.Add(layout.JobName, layout);
                }
            }
        }
        #endregion

        #region ISerializationCallbackReceiver
        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            DataArraysToDictionarys();
        }
        #endregion
    }
}
