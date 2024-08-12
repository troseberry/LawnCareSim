using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

namespace LawnCareSim.Grass
{
    public class LawnGenerator : MonoBehaviour
    {
        private struct CubeVertex
        {
            public Vector3 OffsetPosition;
            public Vector2 UV;
        }

        private struct DrawCube
        {
            public Vector3 Origin;
            public CubeVertex[] Vertices;
        }

        [SerializeField] private Mesh _cubeMesh;
        [SerializeField] private Transform _cubePrefab;
        [SerializeField] private ComputeShader _grassComputeShader;
        [SerializeField] private ComputeShader _triToVertComputeShader;
        [SerializeField] private float _generatedCubeSize;
        [SerializeField] private float _generatedCubeHeight;
        [SerializeField] private Vector2 _gridSize;
        [SerializeField] private Vector2 _gridStart;
        [SerializeField] private float _spacing;
        [SerializeField] private Material _material = default;

        private ComputeBuffer _drawBuffer;
        private ComputeBuffer _argsBuffer;
        private ComputeBuffer _sourceMeshVerticesBuffer;
        private ComputeBuffer _sourceMeshUVsBuffer;
        private ComputeBuffer _sourceMeshNormalsBuffer;
        private ComputeBuffer _sourceCubeTrianglesBuffer;
        private NativeArray<Vector3> _cubePositions;

        private int _allowedBladesX;
        private int _allowedBladesZ;
        private int _totalBlades;

        private Transform[] _cubes;

        private bool _initialized;
        private int _idMeshGeneratorKernal;
        private int _idTriToVertKernal;
        private int _meshDispatchSize;
        private Bounds _localBounds;

        private const int DRAW_STRIDE = sizeof(float) * (3 + (3 + 2) * 3);
        private const int ARGS_STRIDE = sizeof(int) * 4;


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

            CalculateBounds();

            //var sourceMesh = GenerateSourceMesh();
            var sourceMesh = _cubeMesh;

            _drawBuffer = new ComputeBuffer(_totalBlades * sourceMesh.triangles.Length, DRAW_STRIDE, ComputeBufferType.Append);
            _drawBuffer.SetCounterValue(0);

            _argsBuffer = new ComputeBuffer(1, ARGS_STRIDE, ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(new int[] { 0, 1, 0, 0 });

            _sourceMeshVerticesBuffer = new ComputeBuffer(_totalBlades * sourceMesh.vertices.Length, sizeof(float) * 3);
            _sourceMeshVerticesBuffer.SetData(sourceMesh.vertices);

            _sourceMeshUVsBuffer = new ComputeBuffer(_totalBlades * sourceMesh.uv.Length, sizeof(float) * 2);
            _sourceMeshUVsBuffer.SetData(sourceMesh.uv);

            _sourceMeshNormalsBuffer = new ComputeBuffer(_totalBlades * sourceMesh.normals.Length, sizeof(float) * 3);
            _sourceMeshNormalsBuffer.SetData(sourceMesh.normals);

            _sourceCubeTrianglesBuffer = new ComputeBuffer(_totalBlades * sourceMesh.triangles.Length, sizeof(int));
            _sourceCubeTrianglesBuffer.SetData(sourceMesh.triangles);

            _idMeshGeneratorKernal = _grassComputeShader.FindKernel("MeshGenerator");
            _idTriToVertKernal = _triToVertComputeShader.FindKernel("CSMain");

            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_drawTriangles", _drawBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeVertices", _sourceMeshVerticesBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeUVs", _sourceMeshUVsBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeNormals", _sourceMeshNormalsBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeTriangles", _sourceCubeTrianglesBuffer);

            _grassComputeShader.SetFloat("_totalBlades", _totalBlades);
            _grassComputeShader.SetFloat("_genCubeWidthLength", _generatedCubeSize);
            _grassComputeShader.SetFloat("_genCubeHeight", _generatedCubeHeight);
            _grassComputeShader.SetFloats("_gridSize", new float[] { _allowedBladesX, _allowedBladesZ });
            _grassComputeShader.SetFloats("_gridStart", new float[] { _gridStart.x, _gridStart.y });
            _grassComputeShader.SetFloat("_gridSpacing", _spacing);

            _grassComputeShader.GetKernelThreadGroupSizes(_idMeshGeneratorKernal, out uint mesThreadGroupSize, out _, out _);
            _meshDispatchSize = Mathf.CeilToInt((float)_totalBlades / mesThreadGroupSize);

            _triToVertComputeShader.SetBuffer(_idTriToVertKernal, "_indirectArgsBuffer", _argsBuffer);

            _material.SetBuffer("_drawTriangles", _drawBuffer);

            //CalculateFunc();
        }


        private void OnDisable()
        {
            if (_initialized)
            {
                _drawBuffer.Release();
                _argsBuffer.Release();
                _sourceMeshVerticesBuffer.Release();
                _sourceMeshUVsBuffer.Release();
                _sourceMeshNormalsBuffer.Release();
                _sourceCubeTrianglesBuffer.Release();
            }

            _initialized = false;
        }

        private void LateUpdate()
        {
            _drawBuffer.SetCounterValue(0);

            _grassComputeShader.SetMatrix("_localToWorld", transform.localToWorldMatrix);

            _grassComputeShader.Dispatch(_idMeshGeneratorKernal, _meshDispatchSize, 1, 1);

            ComputeBuffer.CopyCount(_drawBuffer, _argsBuffer, 0);

            _triToVertComputeShader.Dispatch(_idTriToVertKernal, 1, 1, 1);

            /*
           RenderParams rendParams = new RenderParams(_material);
           rendParams.worldBounds = _localBounds;

           Graphics.RenderPrimitivesIndirect(rendParams, MeshTopology.Triangles, _drawBuffer);
           */

            Graphics.DrawProceduralIndirect(_material, _localBounds, MeshTopology.Triangles, _argsBuffer, 0,
                null, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
        }

        private void CalculateBounds()
        {
            var gridCenter = new Vector3(_gridStart.x, 1, _gridStart.y);
            gridCenter.x += _gridSize.x / 2;
            gridCenter.z += _gridSize.y / 2;
            var extents = new Vector3(_gridSize.x, 2.0f, _gridSize.y);
            _localBounds = new Bounds(gridCenter, extents);
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

        private Mesh GenerateSourceMesh()
        {
            Mesh mesh = new Mesh();
            Vector3 pos = Vector3.zero;

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
            //mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            //mesh.Optimize();

            Debug.Log($"V: {mesh.vertices.Length} | UVs: {mesh.uv.Length} | N: {mesh.normals.Length} | T:{mesh.triangles.Length}");

            return mesh;
        }

        int Mod(int input, int div)
        {
            if (input < div)
            {
                return input;
            }

            if (input == div)
            {
                return 0;
            }


            for (int i = 0; i < div; i++)
            {
                int cur = div * i;
                int next = div * (i + 1);

                if (next < input)
                {
                    continue;
                }

                return input - cur;
            }

            return -1;
        }

        private void CalculateFunc()
        {
            
            for (int i = 1; i < _totalBlades + 1; i++)
            {
                //var x = i % _gridSize.y;
                //var y = (i - x) % _gridSize.x;

                //var x = i % _gridSize.x;
                //var y = Mathf.Floor(i / _gridSize.y);

                //var x = (i - 1) % _gridSize.y;
                var x = Mod(i, (int)_gridSize.y);
                var y = Mathf.Floor((i - 1) / _gridSize.y);

                Debug.Log($"({x}, {y})");
            }
        }
    }
}
