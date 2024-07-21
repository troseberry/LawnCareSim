using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class Vacuum
    {
        private const string GRASS_CLIPPINGS_TAG = "GrassClippings";

        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive)
            {
                return;
            }

            if (other.tag == GRASS_CLIPPINGS_TAG)
            {
                TryToVacuumClippings(other.gameObject);
            }
        }
    }

    public partial class Vacuum : BaseGear
    {
        private int _maximumBagSpace = 1000;
        private int _bagSpace = 0;

        public override GearType GearType => GearType.Vacuum;

        private void TryToVacuumClippings(GameObject clippings)
        {
            if (IsBagFull())
            {
                return;
            }

            Destroy(clippings);
            _bagSpace++;
            Use();
        }

        public override string DebugUnuiqueStats()
        {
            return $"Bag Space: {_bagSpace}/{_maximumBagSpace}";
        }

        private bool IsBagFull()
        {
            return _bagSpace >= _maximumBagSpace;
        }
    }
}
