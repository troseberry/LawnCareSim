using LawnCareSim.Grass;
using UnityEngine;
using UnityEngine.UI;

namespace LawnCareSim.Gear
{
    public class Vacuum : BaseGear
    {
        private const string GRASS_CLIPPINGS_TAG = "GrassClippings";

        private DefaultGearUsageData _gearData = new DefaultGearUsageData(null);
        private int _maximumBagSpace = 1000;
        private int _bagSpace = 0;

        public override GearType GearType => GearType.Vacuum;

        #region Unity Methods
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == GRASS_CLIPPINGS_TAG)
            {
                _gearData.UsageObject = other.gameObject;
                Use(_gearData);
            }
        }
        #endregion

        #region Gear
        public override void Use(GearUsageData usageData)
        {
            if (!IsActive || IsBagFull() || usageData.UsageObject == null)
            {
                return;
            }

            Destroy(usageData.UsageObject);
            _bagSpace++;

            base.Use(usageData);
        }

        public override string DebugUnuiqueStats()
        {
            return $"Bag Space: {_bagSpace}/{_maximumBagSpace}";
        }

        private bool IsBagFull()
        {
            return _bagSpace >= _maximumBagSpace;
        }
        #endregion
    }
}
