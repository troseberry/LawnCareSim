using LawnCareSim.Data;
using LawnCareSim.Events;
using LawnCareSim.Gear;
using LawnCareSim.Jobs;
using System.Collections.Generic;
using Unity.Jobs;
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

    public class GrassController : MonoBehaviour
    {
        public static GrassController Instance;

        public bool ShowDebugGUI = true;

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

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            EventRelayer.Instance.JobCreatedEvent += JobDataGeneratedEventListener;
        }

        private void JobDataGeneratedEventListener(object sender, Job job)
        {
            if (job == null)
            {
                return;
            }

            StartJob(job);
        }

        /*
        private void OnGUI()
        {
            if (!ShowDebugGUI)
            {
                return;
            }

            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            Rect mainRect = new Rect(width * 0.02f, height * 0.04f, 360, 200);
            GUI.Box(mainRect, GUIContent.none);

            if (GUI.Button(new Rect(mainRect.x + 20, mainRect.y + 25, 150, 150), "Spawn Layout"))
            {
                if (MasterDataManager.Instance.JobDataManager.GetJobLayout("JobLayout_01", out var layout))
                {
                    if (_currentLawn != null)
                    {
                        Destroy(_houseSpawn.GetChild(0).gameObject);
                        Destroy(_lawnSpawn.GetChild(0).gameObject);
                        _currentLawn = null;
                        ClearAllSpawnedGrass();
                    }

                    Instantiate(layout.HousePrefab, _houseSpawn.position, Quaternion.identity, _houseSpawn);
                    var lawn = Instantiate(layout.LawnPrefab, _lawnSpawn.position, Quaternion.identity, _lawnSpawn);
                    _currentLawn = lawn.GetComponent<LawnData>();
                }
            }

            if (GUI.Button(new Rect(mainRect.x + 185, mainRect.y + 25, 150, 150), "Setup Lawn"))
            {
                if (_currentLawn != null && !_grassSpawnInitiated)
                {
                    _grassSpawnInitiated = true;
                    SetupLawn();
                }
            }
        }
        */

        #region Setup
        private void StartJob(Job job)
        {
            var refJob = job;
            if (_currentLawn != null)
            {
                Destroy(_houseSpawn.GetChild(0).gameObject);
                Destroy(_lawnSpawn.GetChild(0).gameObject);
                _currentLawn = null;
                ClearAllSpawnedGrass();
            }

            Instantiate(refJob.Layout.HousePrefab, _houseSpawn.position, Quaternion.identity, _houseSpawn);
            var lawn = Instantiate(refJob.Layout.LawnPrefab, _lawnSpawn.position, Quaternion.identity, _lawnSpawn);
            _currentLawn = lawn.GetComponent<LawnData>();

            if (_currentLawn != null && !_grassSpawnInitiated)
            {
                _grassSpawnInitiated = true;
                var count = SetupLawn();
                refJob.GrassArea = count.Item1;
                refJob.Edges = count.Item2;
            }

            EventRelayer.Instance.OnLawnGenerated(refJob);
        }
        #endregion

        #region Tool Modification Methods
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

                EventRelayer.Instance.OnGrassCut();

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

            EventRelayer.Instance.OnGrassEdged();

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

            EventRelayer.Instance.OnGrassStriped();

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
        #endregion

        #region Lawn Management
        private bool _grassSpawnInitiated;
        private LawnData _currentLawn;
        private Dictionary<string, Grass> _grass = new Dictionary<string, Grass>();
        private Dictionary<string, GameObject> _grassEdges = new Dictionary<string, GameObject>();

        public (int, int) SetupLawn()
        {
            _grass.Clear();
            _grassEdges.Clear();

            if (_currentLawn == null)
            {
                return (-1, -1);
            }

            //Debug.Log($"SetupLawn - {_currentLawn.GrassAreas.Count}");

            int grassCount = 0;
            int edgeCount = 0;
            foreach (var area in _currentLawn.GrassAreas)
            {
                var grass = SpawnGrassInArea(area);
                grassCount += grass.Item1;
                edgeCount += grass.Item2;
            }

            return (grassCount, edgeCount);
        }

        private (int, int) SpawnGrassInArea(Transform area)
        {
            int xScale = (int)area.localScale.x;
            int zScale = (int)area.localScale.z;

            float startX = (area.position.x - (xScale / 2));
            if (xScale % 2 == 0) startX += 0.5f;
            float startZ = (area.position.z - (zScale / 2));
            if (zScale % 2 == 0) startZ += 0.5f;

            //Debug.Log($"[SpawnGrassInArea] - Size {xScale}, {zScale} | Start {startX}, {zScale}");

            int grassCount = 0;
            int grassEdgeCount = 0;
            for (int i = 0; i < xScale; i++)
            {
                for (int j = 0; j < zScale; j++)
                {
                    #region Grass
                    Vector3 spawn = new Vector3(startX + (i * 1.0f), 0.5f, startZ + (j * 1.0f));

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

            return (grassCount, grassEdgeCount);
        }

        private void ClearAllSpawnedGrass()
        {
            foreach (Transform child in _grassParent)
            {
                Destroy(child.gameObject);
            }

            _grassSpawnInitiated = false;
        }
        #endregion
    }
}
