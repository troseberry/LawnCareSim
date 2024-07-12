using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class LawnMower : MonoBehaviour
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
            if (!_isActive)
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

    public partial class LawnMower : IGear
    {
        #region Variables
        private const float DECAY_RATE = 0.0001f;
        private const float ENERGY_DRAIN_RATE = 0.001f;

        private bool _isActive;
        private float _energy = 1.0f;
        private float _durability = 1.0f;
        #endregion

        #region Properties
        public GearType GearType => GearType.Mower;

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public float Durability
        {
            get => _durability;
            set => _durability = Mathf.Clamp(value, 0f, 1.0f);
        }
            
        public float Energy
        {
            get => _energy;
            set => _energy = Mathf.Clamp(value, 0f, 1.0f);
        }
        #endregion

        public void TurnOn()
        {
            if (_energy <= 0 || _durability <= 0)
            {
                return;
            }

            _isActive = true;
        }

        public void TurnOff()
        {
            _isActive = false;
        }

        public void Use()
        {
            Durability -= DECAY_RATE;
            Energy -= ENERGY_DRAIN_RATE;

            if (ShouldSpawnClippings())
            {
                var clippings = Instantiate(_grassClippingsPrefab);
                Vector3 adjustedSpawn = _clippingsSpawn.transform.position;
                adjustedSpawn.y = clippings.transform.position.y;

                clippings.transform.position = adjustedSpawn;
            }

            if (_durability <= 0 || _energy <= 0)
            {
                TurnOff();
            }
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
