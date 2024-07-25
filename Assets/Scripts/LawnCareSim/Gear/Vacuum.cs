using LawnCareSim.Grass;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LawnCareSim.Gear
{
    public class Vacuum : BaseGear
    {
        private const string GRASS_CLIPPINGS_TAG = "GrassClippings";
        protected const float SPACE_DECAY_RATE = 0.01f;

        private DefaultGearUsageInfo _gearData = new DefaultGearUsageInfo(null);
        private GearStat _spaceStat;

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
        public override void Initialize(EnergyType energyType)
        {
            base.Initialize(energyType);

            if (!_stats.ContainsKey(GearStatName.Space))
            {
                _spaceStat = new GearStat(GearStatName.Space, 1.0f);
                _stats.Add(GearStatName.Space, _spaceStat);
            }
        }
        public override void Use(GearUsageInfo usageData)
        {
            base.Use(usageData);
            Destroy(usageData.UsageObject);
        }

        protected override bool CanUse()
        {
            return _spaceStat.Value <= 0 && base.CanUse();
        }

        protected override void DecayUsageStat()
        {
            base.DecayUsageStat();

            _spaceStat.Value -= SPACE_DECAY_RATE;
        }

        public override string DebugUnuiqueStats()
        {
            return $"{_spaceStat}";
        }
        #endregion
    }
}
