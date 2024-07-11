using LawnCareSim.Grass;
using UnityEngine;

namespace LawnCareSim.Equipment
{
    public partial class LawnMower : MonoBehaviour
    {
        private GrassManager _grassManager;

        private const string GRASS_TAG = "Grass";
        private float _cutHeight = 0.5f;

        private void OnGUI()
        {
            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;
            Rect mainRect = new Rect(width * 0.82f, height * 0.04f, 300, 200);

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            GUI.Box(mainRect, GUIContent.none);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y, 250, 30), $"Energy {_energy}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 30, 250, 30), $"Durability: {_durability}", fontStyle);
        }

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
               if (_grassManager.CutGrass(other.gameObject.name, _cutHeight))
                {
                    Use();
                }
                //Cut(other.gameObject);
            }

        }

        private void Cut(GameObject grassObject)
        {
            if (_cutHeight < grassObject.transform.localScale.y)
            {
                grassObject.transform.localScale = new Vector3(1.0f, _cutHeight, 1.0f);
                Use();
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

            if (_durability <= 0 || _energy <= 0)
            {
                TurnOff();
            }
        }
    }
}
