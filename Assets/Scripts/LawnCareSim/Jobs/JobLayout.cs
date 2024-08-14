using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    [Serializable]
    public struct JobLayout
    {
        public string JobName;
        public GameObject LawnPrefab;
        public GameObject HousePrefab;
    }
}
