using LawnCareSim.Gear;
using LawnCareSim.Interaction;
using LawnCareSim.UI;
using UnityEngine;

namespace LawnCareSim.Data
{
    public class MasterDataManager : MonoBehaviour
    {
        public static MasterDataManager Instance;

        [SerializeField] private GearDataManager _gearDataManager;

        public GearDataManager GearDataManager => _gearDataManager;

        private void Awake()
        {
            Instance = this;
        }
    }
}
