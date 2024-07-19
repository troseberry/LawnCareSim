using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

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

    internal struct GrassEdge
    {
        public GameObject GameObject;
        public bool WasCut;
    }

    public partial class GrassManager : MonoBehaviour
    {
        public static GrassManager Instance;

        [SerializeField] private Color _baseColor;
        [SerializeField] private Color _darkGrassStripe;
        [SerializeField] private Color _lightGrassStripe;

        private void Awake()
        {
            Instance = this;
        }

        // on start of job, would take predefined grass area and generate grass objects

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
                _grass[grassName] = grass;

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

                _grassEdges[edgeName] = grassEdge;

                return true;
            }

            return false;
        }

        public void SpawnGrassClippings(Vector3 spawn)
        {
            Instantiate(_grassClippingsPrefab, spawn, Quaternion.identity, _grassParent);
        }

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

            //Debug.Log($"[GetColorForRotation] - Rotation: {rotation} | Scaled: {rotationScale}");

            return Color.Lerp(_darkGrassStripe, _lightGrassStripe, rotationScale);
        }

        private Color GetColorForRotation(float newRotation, ref Grass toModify)
        {
            float midRotation = Mathf.Round((toModify.StripeValue + newRotation) * 0.5f);

            float rotationScale = midRotation/360f;

            Debug.Log($"[GetColorForRotation] - Prev/New/Mid: {toModify.StripeValue}/{newRotation}/{midRotation} | Scaled: {rotationScale}");

            toModify.StripeValue = midRotation;

            return Color.Lerp(_darkGrassStripe, _lightGrassStripe, rotationScale);
        }
    }

    #region Debug
    public partial class GrassManager
    {
        private Dictionary<string, Grass> _grass = new Dictionary<string, Grass>();
        private Dictionary<string, GrassEdge> _grassEdges = new Dictionary<string, GrassEdge>();

        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private GameObject _grassEdgePrefab;
        [SerializeField] private GameObject _grassClippingsPrefab;
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
