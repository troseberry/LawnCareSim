using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public class Striper : BaseGear
    {
        private const string GRASS_TAG = "Grass";

        private GrassManager _grassManager;
        private DefaultGearUsageData _gearData = new DefaultGearUsageData(null);

        public override GearType GearType => GearType.Striper;

        #region Unity Methods
        private void Start()
        {
            _grassManager = GrassManager.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == GRASS_TAG)
            {
                _gearData.UsageObject = other.gameObject;
                Use(_gearData);
            }
        }
        #endregion

        #region Gear
        public override void Use(GearUsageData data)
        {
            if (!IsActive || data.UsageObject == null)
            {
                return;
            }

            if (!_grassManager.StripeGrass(data.UsageObject.name, transform.eulerAngles.y))
            {
                return;
            }

            base.Use(data);
        }
        #endregion
    }
}
