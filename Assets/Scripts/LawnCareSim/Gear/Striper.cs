using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class Striper
    {
        private const string GRASS_TAG = "Grass";

        private GrassManager _grassManager;


        private void Start()
        {
            _grassManager = GrassManager.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive)
            {
                return;
            }

            if (other.tag == GRASS_TAG)
            {
                if (_grassManager.StripeGrass(other.gameObject.name, transform.eulerAngles.y))
                {
                    Use();
                }
            }
        }
    }

    public partial class Striper : BaseGear
    {
        public override GearType GearType => GearType.Striper;

        public override void Use()
        {
            base.Use();
        }
    }
}
