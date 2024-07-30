using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace LawnCareSim.Grass
{
    public class LawnGenerator : MonoBehaviour
    {
        private struct DrawCube
        {
            public Vector3 Position;
        }

        //public Transform GeneratePosition;
        [SerializeField] private Transform _cubePrefab;
        [SerializeField] private ComputeShader _cubeShader;
        [SerializeField] private float _generatedCubeSize;
        [SerializeField] private Vector2 _gridSize;
        [SerializeField] private float _gridStartX;
        [SerializeField] private float _gridStartZ;
        //[SerializeField] private Material _material = default;

        private ComputeBuffer _drawCubesBuffer;
        //private DrawCube[] _cubePositions;
        private NativeArray<Vector3> _cubePositions;

        private int _allowedBladesX;
        private int _allowedBladesZ;
        private int _totalBlades;

        private Transform[] _cubes;

        private bool _dispatched;
        private bool _initialized;
        private int _idCubeKernal;
        private int _dispatchSize;
        //private Bounds _localBounds;

        private float _timer;

        private const int DRAW_STRIDE = sizeof(float) * 3;
        private void Start()
        {
            //ComposeCubeMesh();
            //ComposeTriangleMesh();
        }
        
        /*
        private void ComposeCubeMesh()
        {
            var pos = GeneratePosition.position;

            Mesh mesh = new Mesh();

            Vector3[] vertices = 
            {
                pos,
                new Vector3(pos.x + 1, pos.y, pos.z),
                new Vector3(pos.x + 1, pos.y, pos.z - 1),
                new Vector3(pos.x, pos.y, pos.z - 1),

                
                new Vector3(pos.x, pos.y - 1, pos.z),
                new Vector3(pos.x + 1, pos.y - 1, pos.z),
                new Vector3(pos.x + 1, pos.y - 1, pos.z - 1),
                new Vector3(pos.x, pos.y - 1, pos.z - 1)
            };

            Vector2[] uvs =
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),

                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0)
            };

            int[] triangles =
            {
                0, 1, 2,
                0, 2, 3,

                3, 2, 6,
                3, 6, 7,

                0, 3, 7,
                0, 7, 4,

                1, 0, 4,
                1, 4, 5,

                2, 1, 5,
                2, 5, 6,

                5, 4, 7,
                5, 7, 6
                
            };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            mesh.Optimize();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        private void ComposeTriangleMesh()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices =
            {
                new Vector3(0, 3, 0),
                new Vector3(0, 3, 1),
                new Vector3(1, 3, 1)
            };

            Vector2[] uv =
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            int[] tris =
            {
                0, 1, 2
            };

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = tris;

            GetComponent<MeshFilter>().mesh = mesh;
        }
        */

        private void OnEnable()
        {
            if (_initialized)
            {
                OnDisable();
            }

            _initialized = true;

            _allowedBladesX = (int)(_gridSize.x / _generatedCubeSize);
            _allowedBladesZ = (int)(_gridSize.y / _generatedCubeSize);
            _totalBlades = _allowedBladesX * _allowedBladesZ;

            //_cubePositions = new DrawCube[_totalBlades];
            // _cubePositions = new NativeArray<Vector3>()

            _drawCubesBuffer = new ComputeBuffer(_totalBlades, DRAW_STRIDE, ComputeBufferType.Append);
            _drawCubesBuffer.SetCounterValue(0);

            _idCubeKernal = _cubeShader.FindKernel("CSMain");

            _cubeShader.SetBuffer(_idCubeKernal, "_drawCubes", _drawCubesBuffer);
            _cubeShader.SetFloat("_allowedBladesX", _allowedBladesX);
            _cubeShader.SetFloat("_allowedBladesZ", _allowedBladesZ);
            _cubeShader.SetFloat("_totalBlades", _totalBlades);
            _cubeShader.SetFloat("_genCubeWidthLength", _generatedCubeSize);
            _cubeShader.SetFloat("_gridStartX", _gridStartX);
            _cubeShader.SetFloat("_gridStartZ", _gridStartZ);

            _cubeShader.GetKernelThreadGroupSizes(_idCubeKernal, out uint threadGroupSize, out _, out _);
            _dispatchSize = Mathf.CeilToInt((float)_totalBlades / threadGroupSize);

        }


        private void OnDisable()
        {
            if (_initialized)
            {
                _drawCubesBuffer.Release();
            }

            _initialized = false;
        }

        private void LateUpdate()
        {
            // Clear the draw buffer of last frame's data
            //_drawCubesBuffer.SetCounterValue(0);

            //Graphics.RenderPrimitivesIndirect()
            /*
            Graphics.DrawProceduralIndirect(_material, bounds, MeshTopology.Triangles, _argsBuffer, 0,
            null, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
            */

            if (!_dispatched)
            {
                _cubeShader.Dispatch(_idCubeKernal, _dispatchSize, 1, 1);
                _dispatched = true;

                AsyncGPUReadback.Request(_drawCubesBuffer, (request) =>
                {
                    _cubePositions = request.GetData<Vector3>();
                    SpawnCubes();
                });
            }

            /*
            if (!_spawned && _timer > 5.0f)
            {
                _drawCubesBuffer.GetData(_cubePositions);
                SpawnCubes();
                _spawned = true;
            }
            */

            _timer += Time.deltaTime;
        }

        private void SpawnCubes()
        {
            _cubes = new Transform[_totalBlades];

            for (int x = 0, i = 0; x < _allowedBladesX; x++)
            {
                for (int z = 0; z < _allowedBladesZ; z++, i++)
                {
                    _cubes[i] = Instantiate(_cubePrefab);
                    _cubes[i].transform.position = _cubePositions[i];
                    _cubes[i].localScale = new Vector3(_generatedCubeSize, 1.5f, _generatedCubeSize);
                }
            }
        }
    }
}
