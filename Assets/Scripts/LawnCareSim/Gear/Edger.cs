using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public class Edger : BaseGear
    {
        [SerializeField] private GameObject _clippingsSpawn;

        private const string GRASS_EDGE_TAG = "GrassEdge";

        private GrassController _grassManager;
        private DefaultGearUsageInfo _gearData = new DefaultGearUsageInfo(null);

        public override GearType GearType => GearType.Edger;

        #region Unity Methods
        private void Start()
        {
            _grassManager = GrassController.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == GRASS_EDGE_TAG)
            {
                _gearData.UsageObject = other.gameObject;
                Use(_gearData);
            }
        }
        #endregion

        #region Gear
        public override void Use(GearUsageInfo data)
        {
            if (!IsActive || data.UsageObject == null)
            {
                return;
            }

            if (!_grassManager.CutGrassEdge(data.UsageObject.name))
            {
                return;
            }

            base.Use(data);
        }
        #endregion
    }
}
