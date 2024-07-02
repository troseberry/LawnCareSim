using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Equipment
{
    public class LawnMower : MonoBehaviour, IGear
    {
        private GrassManager _grassManager;

        private const string GRASS_TAG = "Grass";
        private float _cutHeight = 0.5f;

        private void Start()
        {
            // Later, do this at the start of a job
            _grassManager = GrassManager.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == GRASS_TAG)
            {
                //Destroy(other.gameObject);
                _grassManager.CutGrass(other.gameObject, _cutHeight);
            }

        }
    }
}
