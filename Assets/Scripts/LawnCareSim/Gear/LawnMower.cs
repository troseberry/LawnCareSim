using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class LawnMower
    {
        [SerializeField] private GameObject _grassClippingsPrefab;
        [SerializeField] private GameObject _clippingsSpawn;

        private GrassManager _grassManager;

        private const string GRASS_TAG = "Grass";
        private float _cutHeight = 0.5f;

        private Transform _groundCheckPoint;

        private void Start()
        {
            // Later, do this at the start of a job
            _grassManager = GrassManager.Instance;
            _groundCheckPoint = transform.Find("GroundCheckPoint");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive)
            {
                return;
            }

            if (other.tag == GRASS_TAG)
            {
                if (_grassManager.CutGrass(other.gameObject.name, _cutHeight))
                {
                    Use();
                }
            }
        }
    }

    public partial class LawnMower : BaseGear
    {
        public override GearType GearType => GearType.Mower;

        public override void Use()
        {
            if (ShouldSpawnClippings())
            {
                var clippings = Instantiate(_grassClippingsPrefab);
                Vector3 adjustedSpawn = _clippingsSpawn.transform.position;
                adjustedSpawn.y = clippings.transform.position.y;

                clippings.transform.position = adjustedSpawn;
            }

            base.Use();
        }

        private bool ShouldSpawnClippings()
        {
            if (Physics.Raycast(_groundCheckPoint.transform.position, Vector3.down, out var hit, 1.0f))
            {
                if (hit.collider.tag == GRASS_TAG)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
