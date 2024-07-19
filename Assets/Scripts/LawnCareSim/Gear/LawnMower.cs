using LawnCareSim.Grass;
using LawnCareSim.Input;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class LawnMower
    {
        [SerializeField] private GameObject _clippingsSpawn;
        [SerializeField] private Transform _groundCheckPoint;

        private const string GRASS_TAG = "Grass";

        private GrassManager _grassManager;

        private void Start()
        {
            _grassManager = GrassManager.Instance;
            InputController.Instance.AdjustedCutHeightEvent += AdjustedCutHeightEventListener;
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

        private void AdjustedCutHeightEventListener(object sender, float args)
        {
            AdjustCutHeight(args);
        }
    }

    public partial class LawnMower : BaseGear
    {
        private const float CUT_HEIGHT_INCREMENT = 0.05f;
        private const float CUT_HEIGHT_MIN = 0.1f;
        private const float CUT_HEIGHT_MAX = 1.0f;

        private float _cutHeight = 0.5f;

        public override GearType GearType => GearType.Mower;

        public override void Use()
        {
            if (ShouldSpawnClippings())
            {
                _grassManager.SpawnGrassClippings(_clippingsSpawn.transform.position);
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
    }
}
