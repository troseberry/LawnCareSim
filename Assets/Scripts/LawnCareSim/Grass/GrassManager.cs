using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Grass
{
    public partial class GrassManager : MonoBehaviour
    {
        public static GrassManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        // on start of job, would take predefined grass area and generate grass objects

        public void CutGrass(GameObject grassObject, float height)
        {
            if (height < grassObject.transform.localScale.y)
            {
                grassObject.transform.localScale = new Vector3(1.0f, height, 1.0f);
            }
        }
    }

    public partial class GrassManager
    {
        private List<GameObject> _grassObjects = new List<GameObject>();

        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private Transform _grassParent;
        [SerializeField] private Vector2 _horizontalRange;
        [SerializeField] private Vector2 _verticalRange;

        private void Start()
        {

            DebugCreateGrassInArea();
        }

        private void DebugCreateGrassInArea()
        {
            int xMin = Mathf.RoundToInt(_horizontalRange.x);
            int xMax = Mathf.RoundToInt(_horizontalRange.y);
            int yMin = Mathf.RoundToInt(_verticalRange.x);
            int yMax = Mathf.RoundToInt(_verticalRange.y);
            int xRange = xMax - xMin;
            int yRange = yMax - yMin;

            for (int i = 0; i < xRange * 2; i++)
            {
                for (int j = 0; j < yRange * 2; j++)
                {
                    Vector3 spawn = new Vector3(xMin + (i * 0.5f), 0.5f, yMin + (j * 0.5f));
                    _grassObjects.Add(Instantiate(_grassPrefab, spawn, Quaternion.identity, _grassParent));

                    j++;
                }

                i++;
            }
        }
    }
}
