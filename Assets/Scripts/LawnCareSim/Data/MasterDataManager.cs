using LawnCareSim.Gear;
using LawnCareSim.Interaction;
using LawnCareSim.Jobs;
using LawnCareSim.UI;
using UnityEngine;

namespace LawnCareSim.Data
{
    public class MasterDataManager : MonoBehaviour
    {
        public static MasterDataManager Instance;

        [SerializeField] private GearDataManager _gearDataManager;
        [SerializeField] private JobDataManager _jobDataManager;

        public GearDataManager GearDataManager => _gearDataManager;
        public JobDataManager JobDataManager => _jobDataManager;

        private void Awake()
        {
            Instance = this;
        }
    }
}
