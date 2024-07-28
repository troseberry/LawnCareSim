using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    [CreateAssetMenu(fileName = "JobLayout", menuName = "Jobs/JobLayout")]
    public class JobLayout : ScriptableObject
    {
        public GameObject LawnPrefab;
        public GameObject HousePrefab;
    }
}
