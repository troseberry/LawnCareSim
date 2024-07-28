using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public class Striper : BaseGear
    {
        private const string GRASS_TAG = "Grass";

        private GrassController _grassManager;
        private DefaultGearUsageInfo _gearData = new DefaultGearUsageInfo(null);

        public override GearType GearType => GearType.Striper;

        public override bool RequiresEnergy => false;

        #region Unity Methods
        private void Start()
        {
            _grassManager = GrassController.Instance;
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
        public override void Use(GearUsageInfo data)
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
