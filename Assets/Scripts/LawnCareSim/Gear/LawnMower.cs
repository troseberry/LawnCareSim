using LawnCareSim.Grass;
using LawnCareSim.Input;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class LawnMower
    {
        
    }

    public partial class LawnMower : BaseGear
    {
        [SerializeField] private GameObject _clippingsSpawn;
        [SerializeField] private Transform _groundCheckPoint;

        private const string GRASS_TAG = "Grass";
        private const float CUT_HEIGHT_INCREMENT = 0.05f;
        private const float CUT_HEIGHT_MIN = 0.1f;
        private const float CUT_HEIGHT_MAX = 1.0f;

        private GrassManager _grassManager;
        private float _cutHeight = 0.5f;
        private DefaultGearUsageData _gearData = new DefaultGearUsageData(null);

        public override GearType GearType => GearType.Mower;

        #region Unity Methods
        private void Start()
        {
            _grassManager = GrassManager.Instance;
            InputController.Instance.AdjustedCutHeightEvent += AdjustedCutHeightEventListener;
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

        #region Event Listeners
        private void AdjustedCutHeightEventListener(object sender, float args)
        {
            AdjustCutHeight(args);
        }
        #endregion

        #region Gear
        public override void Use(GearUsageData data)
        {
            if (!IsActive || data.UsageObject == null)
            {
                return;
            }
            if (!_grassManager.CutGrass(data.UsageObject.name, _cutHeight))
            {
                return;
            }

            if (ShouldSpawnClippings())
            {
                _grassManager.SpawnGrassClippings(_clippingsSpawn.transform.position);
            }

            base.Use(data);
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

        private void AdjustCutHeight(float dir)
        {
            float modValue = CUT_HEIGHT_INCREMENT * dir;
            _cutHeight = Mathf.Clamp(_cutHeight + modValue, CUT_HEIGHT_MIN, CUT_HEIGHT_MAX);

            // Eliminate extra digits from float math
            _cutHeight *= 100f;
            _cutHeight = Mathf.Round(_cutHeight);
            _cutHeight /= 100f;
        }

        public override string DebugUnuiqueStats()
        {
            return $"Cut Height: {_cutHeight}";
        }
        #endregion
    }
}
