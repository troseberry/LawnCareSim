using LawnCareSim.Jobs;
using System.Collections.Generic;
using UnityEngine;

namespace LawnCareSim.Grass
{
    internal struct Grass
    {
        public GameObject GameObject;
        public MeshRenderer GrassRenderer;
        public bool WasCut;
        public float Height;
        public bool HasBeenStriped;
        public float StripeValue;
    }

    public partial class GrassController : MonoBehaviour
    {
        public static GrassController Instance;

        #region Serialized Vars

        [SerializeField] private Transform _grassParent;
        [SerializeField] private Transform _lawnSpawn;
        [SerializeField] private Transform _houseSpawn;

        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private GameObject _grassEdgePrefab;
        [SerializeField] private GameObject _grassClippingsPrefab;
        [SerializeField] private Color _baseColor;
        [SerializeField] private Color _darkGrassStripe;
        [SerializeField] private Color _lightGrassStripe;
        #endregion

        private const string GRASS_CLIPPINGS_TAG = "GrassClippings";

        public JobLayout CurrentJobLayout;

        private bool _spawnInitiated;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Instantiate(CurrentJobLayout.HousePrefab, _houseSpawn.position, Quaternion.identity, _houseSpawn);
            var lawn = Instantiate(CurrentJobLayout.LawnPrefab, _lawnSpawn.position, Quaternion.identity, _lawnSpawn);

            _currentLawn = lawn.GetComponent<LawnData>();

        }
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                if (!_spawnInitiated)
                {
                    _spawnInitiated = true;
                    SetupLawn();
                }

            }
        }

        #region Mowing
        public bool CutGrass(string grassName, float cutHeight)
        {
            if (!_grass.TryGetValue(grassName, out var grass))
            {
                return false;
            }

            if (cutHeight < grass.Height)
            {
                grass.GameObject.transform.localScale = new Vector3(1.0f, cutHeight, 1.0f);
                grass.Height = cutHeight;

                if (grass.HasBeenStriped)
                {
                    grass.HasBeenStriped = false;
                    grass.StripeValue = 0f;
                    grass.GrassRenderer.material.SetColor("_BaseColor", _baseColor);
                }

                _grass[grassName] = grass;

                return true;
            }

            return false;
        }
        #endregion

        #region Edging
        public bool CutGrassEdge(string edgeName)
        {
            if (!_grassEdges.TryGetValue(edgeName, out var grassEdge))
            {
                return false;
            }

            // Cut edge and change remainder to clippings
            grassEdge.transform.GetChild(0).gameObject.SetActive(false);
            grassEdge.tag = GRASS_CLIPPINGS_TAG;

            _grassEdges.Remove(edgeName);

            return true;
        }
        #endregion

        #region Clippings
        public void SpawnGrassClippings(Vector3 spawn)
        {
            Instantiate(_grassClippingsPrefab, spawn, Quaternion.identity, _grassParent);
        }
        #endregion

        #region Striping
        public bool StripeGrass(string grassName, float newRotation)
        {
            if (!_grass.TryGetValue(grassName, out var grass))
            {
                return false;
            }

            float modRotation = ModifyStripeRotation(newRotation);

            //if the new rotation is the exact opposite, just stripe completely over, otherwise, color mix
            bool areRotationsOpposite = CheckForOppositeRotation(grass.StripeValue, modRotation);
            if (grass.HasBeenStriped && !areRotationsOpposite)
            {
                modRotation = Mathf.Round((grass.StripeValue + modRotation) * 0.5f);
            }

            grass.HasBeenStriped = true;
            grass.StripeValue = modRotation;
            grass.GrassRenderer.material.SetColor("_BaseColor", GetColorForRotation(modRotation));

            _grass[grassName] = grass;

            return true;
        }

        public bool ResetStriping(string grassName)
        {
            if (!_grass.TryGetValue(grassName, out var grass))
            {
                return false;
            }

            grass.HasBeenStriped = false;
            grass.StripeValue = 0f;
            grass.GrassRenderer.material.SetColor("_BaseColor", _baseColor);

            _grass[grassName] = grass;

            return true;
        }

        private bool CheckForOppositeRotation(float first, float second)
        {
            float firstRounded = Mathf.Round(first);
            float secondRounded = Mathf.Round(second);
            return Mathf.Abs(firstRounded - secondRounded) == 180f;
        }

        private float ModifyStripeRotation(float rotation)
        {
            float modRotation = rotation;
            if (modRotation < 0f)
            {
                modRotation += 360f;
            }
            modRotation = Mathf.Clamp(modRotation, 0f, 360f);
            modRotation /= 15.0f;
            modRotation = Mathf.Round(modRotation);
            modRotation *= 15.0f;

            return modRotation;
        }

        private Color GetColorForRotation(float rotation)
        {
            float rotationScale = rotation / 360f;
            return Color.Lerp(_darkGrassStripe, _lightGrassStripe, rotationScale);
        }
        #endregion
    }

    public partial class GrassController
    {
        private LawnData _currentLawn;
        private Dictionary<string, Grass> _grass = new Dictionary<string, Grass>();
        private Dictionary<string, GameObject> _grassEdges = new Dictionary<string, GameObject>();

        public void SetupLawn()
        {
            _grass.Clear();
            _grassEdges.Clear();

            // TO-DO: need to set random grass height for grass on spawn. or take it from job template data

            if (_currentLawn == null)
            {
                return;
            }

            Debug.Log($"SetupLawn - {_currentLawn.GrassAreas.Count}");

            foreach (var area in _currentLawn.GrassAreas)
            {
                SpawnGrassInArea(area);
            }
        }

        private void SpawnGrassInArea(Transform area)
        {
            int xScale = (int)area.localScale.x;
            int zScale = (int)area.localScale.z;
            
            float startX = (area.position.x - (xScale / 2));
            if (xScale % 2 == 0) startX += 0.5f;
            float startZ = (area.position.z - (zScale / 2));
            if (zScale % 2 == 0) startZ += 0.5f;

            Debug.Log($"[SpawnGrassInArea] - Size {xScale}, {zScale} | Start {startX}, {zScale}");

            int grassCount = 0;
            int grassEdgeCount = 0;
            for (int i = 0; i < xScale; i++)
            {
                for (int j = 0; j < zScale; j++)
                {
                    #region Grass
                    Vector3 spawn = new Vector3(startX + (i * 1.0f), 0.5f, startZ + (j * 1.0f));
                    Debug.Log(spawn);
                    
                    var grass = Instantiate(_grassPrefab, spawn, Quaternion.identity, _grassParent);

                    if (grass == null)
                    {
                        continue;
                    }

                    grass.name = $"Grass_{area.name}_{grassCount}";

                    if (!_grass.ContainsKey(grass.name))
                    {
                        _grass.Add(grass.name, new Grass
                        {
                            GameObject = grass,
                            GrassRenderer = grass.GetComponentInChildren<MeshRenderer>(),
                            WasCut = false,
                            Height = grass.transform.localScale.y,
                            HasBeenStriped = false,
                            StripeValue = 0,
                        });

                        grassCount++;
                    }

                    #endregion

                    
                    #region Grass Edges
                    bool canSpawnEdge = i == 0 || i == startX + xScale || j == 0 || j == startZ + zScale;
                    if (canSpawnEdge && Random.Range(0, 2) == 1)
                    {
                        Vector3 edgeSpawn = spawn;
                        Quaternion edgeRotation = Quaternion.identity;

                        if (i == 0)
                        {
                            edgeSpawn.x -= 1.0f;
                            edgeRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                        }
                        else if (i == startX + xScale)
                        {
                            edgeSpawn.x += 1.0f;
                        }
                        else if (j == 0)
                        {
                            edgeSpawn.z -= 1.0f;
                            edgeRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                        }
                        else if (j == startZ + zScale)
                        {
                            edgeSpawn.z += 1.0f;
                            edgeRotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                        }

                        var edge = Instantiate(_grassEdgePrefab, edgeSpawn, edgeRotation, _grassParent);
                        if (edge == null)
                        {
                            continue;
                        }
                        edge.name = $"GrassEdge_{area.name}_{grassEdgeCount}";

                        if (!_grass.ContainsKey(grass.name))
                        {
                            _grassEdges.Add(edge.name, edge);
                            grassEdgeCount++;
                        }

                    }
                    #endregion
                    
                    
                }
            }
        }
    }

    /*
    #region Debug
    public partial class GrassController
    {
        private Dictionary<string, Grass> _grass = new Dictionary<string, Grass>();
        private Dictionary<string, GameObject> _grassEdges = new Dictionary<string, GameObject>();

        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private GameObject _grassEdgePrefab;
        [SerializeField] private GameObject _grassClippingsPrefab;
        [SerializeField] private Transform _grassParent;
        [SerializeField] private Vector2 _horizontalRange;
        [SerializeField] private Vector2 _verticalRange;

        private void Start()
        {
            //DebugCreateGrassInArea();
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
                        GrassRenderer = grass.GetComponentInChildren<MeshRenderer>(),
                        WasCut = false,
                        Height = grass.transform.localScale.y,
                        HasBeenStriped = false,
                        StripeValue = 0,
                    }); ;
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
                            edgeSpawn.x -= 1.0f;
                            edgeRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                        }
                        else if (i == xRange * 2)
                        {
                            edgeSpawn.x += 1.0f;
                        }
                        else if (j == 0)
                        {
                            edgeSpawn.z -= 1.0f;
                            edgeRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                        }
                        else if (j == yRange * 2)
                        {
                            edgeSpawn.z += 1.0f;
                            edgeRotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                        }

                        var edge = Instantiate(_grassEdgePrefab, edgeSpawn, edgeRotation, _grassParent);
                        edge.name = $"GrassEdge_{grassEdgeCount}";
                        _grassEdges.Add(edge.name, edge);

                        grassEdgeCount++;
                    }
                    #endregion
                }
            }
        }
    }
    #endregion
    */
}
