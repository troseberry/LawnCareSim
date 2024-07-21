using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class Edger
    {
        [SerializeField] private GameObject _clippingsSpawn;

        private const string GRASS_EDGE_TAG = "GrassEdge";

        private GrassManager _grassManager;
        private Transform _groundCheckPoint;


        private void Start()
        {
            _grassManager = GrassManager.Instance;
            _groundCheckPoint = transform.Find("GroundCheckPoint");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive)
            {
                return;
            }

            if (other.tag == GRASS_EDGE_TAG)
            {
                if (_grassManager.CutGrassEdge(other.gameObject.name))
                {
                    Use();
                }
            }
        }
    }

    public partial class Edger : BaseGear
    {
        public override GearType GearType => GearType.Edger;

        public override void Use()
        {
            if (ShouldSpawnClippings())
            {
                //_grassManager.SpawnGrassClippings(_clippingsSpawn.transform.position);
            }

            base.Use();
        }

        private bool ShouldSpawnClippings()
        {
            if (Physics.Raycast(_groundCheckPoint.transform.position, Vector3.down, out var hit, 1.0f))
            {
                if (hit.collider.tag == GRASS_EDGE_TAG)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
