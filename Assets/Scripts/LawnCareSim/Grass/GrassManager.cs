using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Grass
{
    internal struct GrassObject
    {
        public GameObject GameObject;
        public Vector2 Location;
        public float Height;
        public bool WasCut;
        public GameObject OvergrownEdge;
    }

    public partial class GrassManager : MonoBehaviour
    {
        public static GrassManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        // on start of job, would take predefined grass area and generate grass objects

        public bool CutGrass(string grassName, float height)
        {
            if (!_grassObjects.TryGetValue(grassName, out var grassObj))
            {
                // Log
                return false;
            }

            if (!grassObj.WasCut && height < grassObj.Height)
            {
                grassObj.GameObject.transform.localScale = new Vector3(1.0f, height, 1.0f);
                grassObj.Height = height;
                grassObj.WasCut = true;

                return true;
            }

            return false;
        }
    }

    public partial class GrassManager
    {
        private Dictionary<string, GrassObject> _grassObjects = new Dictionary<string, GrassObject>();

        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private GameObject _overgrownGrassPrefab;
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

            int grassCount = 0;
            for (int i = 0; i < xRange * 2 + 1; i++)
            {
                for (int j = 0; j < yRange * 2 + 1; j++)
                {
                    bool canSpawnEdge = i == 0 || i == xRange * 2 || j == 0 || j == yRange * 2;

                    Vector3 spawn = new Vector3(xMin + (i * 0.5f), 0.5f, yMin + (j * 0.5f));
                    var grass = Instantiate(_grassPrefab, spawn, Quaternion.identity, _grassParent);
                    grass.name = $"Grass_{grassCount}";

                    var grassObj = new GrassObject
                    {
                        GameObject = grass,
                        Location = spawn,
                        Height = grass.transform.localScale.y,
                        WasCut = false,
                    };

                    if (canSpawnEdge)
                    {
                        if (Random.Range(0, 2) == 1)
                        {
                            Vector3 edgeSpawn = spawn;
                            Quaternion edgeRotation = Quaternion.identity;

                            if (i == 0)
                            {
                                edgeSpawn.x -= 1;
                                edgeRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                            } 
                            else if (i == xRange * 2)
                            {
                                edgeSpawn.x += 1;
                            }
                            else if (j == 0)
                            {
                                edgeSpawn.z -= 1;
                                edgeRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                            }
                            else if (j == yRange * 2)
                            {
                                edgeSpawn.z += 1;
                                edgeRotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                            }

                            var edge = Instantiate(_overgrownGrassPrefab, edgeSpawn, edgeRotation, grassObj.GameObject.transform);
                            edge.name = "OvergrownEdge";

                            grassObj.OvergrownEdge = edge;
                        }
                    }
                    
                    _grassObjects.Add(grass.name, grassObj);

                    grassCount++;
                    j++;
                }

                i++;
            }
        }

        /*
        private GameObject SpawnEdge(GrassObject grassObj)
        {

        }
        */
    }
}
