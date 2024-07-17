using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Grass
{
    internal struct Grass
    {
        public GameObject GameObject;
        public float Height;
        public bool WasCut;
    }

    internal struct GrassEdge
    {
        public GameObject GameObject;
        public bool WasCut;
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
            if (!_grass.TryGetValue(grassName, out var grass))
            {
                // Log
                return false;
            }

            if (!grass.WasCut && height < grass.Height)
            {
                grass.GameObject.transform.localScale = new Vector3(1.0f, height, 1.0f);
                grass.Height = height;
                grass.WasCut = true;

                return true;
            }

            return false;
        }

        public bool CutGrassEdge(string edgeName)
        {
            if (!_grassEdges.TryGetValue(edgeName, out var grassEdge))
            {
                return false;
            }

            if (!grassEdge.WasCut)
            {
                grassEdge.GameObject.SetActive(false);
                grassEdge.WasCut = true;

                return true;
            }

            return false;
        }
    }

    #region Debug
    public partial class GrassManager
    {
        private Dictionary<string, Grass> _grass = new Dictionary<string, Grass>();
        private Dictionary<string, GrassEdge> _grassEdges = new Dictionary<string, GrassEdge>();

        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private GameObject _grassEdgePrefab;
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
            int grassEdgeCount = 0;
            for (int i = 0; i < xRange * 2 + 1; i++)
            {
                for (int j = 0; j < yRange * 2 + 1; j++)
                {
                    #region Spawn Grass
                    Vector3 spawn = new Vector3(xMin + (i * 0.5f), 0.5f, yMin + (j * 0.5f));
                    var grass = Instantiate(_grassPrefab, spawn, Quaternion.identity, _grassParent);
                    grass.name = $"Grass_{grassCount}";

                    _grass.Add(grass.name, new Grass
                    {
                        GameObject = grass,
                        Height = grass.transform.localScale.y,
                        WasCut = false,
                    });
                    grassCount++;
                    #endregion

                    #region Spawn Grass Edge
                    bool canSpawnEdge = i == 0 || i == xRange * 2 || j == 0 || j == yRange * 2;
                    if (canSpawnEdge && Random.Range(0, 2) == 1)
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

                        var edge = Instantiate(_grassEdgePrefab, edgeSpawn, edgeRotation, _grassParent);
                        edge.name = $"GrassEdge_{grassEdgeCount}";

                        _grassEdges.Add(edge.name, new GrassEdge
                        {
                            GameObject = edge,
                            WasCut = false
                        });
                        grassEdgeCount++;
                    }
                    #endregion

                    j++;
                }
                i++;
            }
        }
    }
    #endregion
}
