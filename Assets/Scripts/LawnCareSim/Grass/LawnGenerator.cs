using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

namespace LawnCareSim.Grass
{
    public class LawnGenerator : MonoBehaviour
    {
        [SerializeField] private Mesh _cubeMesh;
        [SerializeField] private ComputeShader _grassComputeShader;
        [SerializeField] private ComputeShader _triToVertComputeShader;
        [SerializeField] private Material _material = default;
        [SerializeField] private float _generatedCubeSize;
        [SerializeField] private float _generatedCubeHeight;
        [SerializeField] private Vector2 _gridSize;
        [SerializeField] private Vector2 _gridStart;
        [SerializeField] private float _spacing;

        private ComputeBuffer _drawBuffer;
        private ComputeBuffer _argsBuffer;
        private ComputeBuffer _sourceMeshVerticesBuffer;
        private ComputeBuffer _sourceMeshUVsBuffer;
        private ComputeBuffer _sourceMeshNormalsBuffer;
        private ComputeBuffer _sourceCubeTrianglesBuffer;
        private ComputeBuffer _bladeRandomPositionJittersBuffer;
        private ComputeBuffer _bladeRandomColorsBuffer;

        private int _allowedBladesX;
        private int _allowedBladesZ;
        private int _totalBlades;
        private Vector2[] _randomPositionJitters;
        private float[] _randomBladeColors;

        private bool _dispatched;
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

            _randomPositionJitters = new Vector2[_totalBlades];
            _randomBladeColors = new float[_totalBlades];
            for (int i = 0; i < _totalBlades; i++)
            {
                _randomPositionJitters[i] = new Vector2(Random.Range(0f, _spacing / 4.0f), Random.Range(0f, _spacing / 4.0f));
                _randomBladeColors[i] = Random.Range(0f, 1.0f);
            }

            CalculateBounds();

            var sourceMesh = _cubeMesh;

            #region Define Buffers
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

            _bladeRandomPositionJittersBuffer = new ComputeBuffer(_totalBlades, sizeof(float) * 2);
            _bladeRandomPositionJittersBuffer.SetData(_randomPositionJitters);

            _bladeRandomColorsBuffer = new ComputeBuffer(_totalBlades, sizeof(float));
            _bladeRandomColorsBuffer.SetData(_randomBladeColors);
            #endregion

            #region Set Shader Values
            _idMeshGeneratorKernal = _grassComputeShader.FindKernel("MeshGenerator");
            _idTriToVertKernal = _triToVertComputeShader.FindKernel("CSMain");

            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_drawTriangles", _drawBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeVertices", _sourceMeshVerticesBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeUVs", _sourceMeshUVsBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeNormals", _sourceMeshNormalsBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_sourceCubeTriangles", _sourceCubeTrianglesBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_bladeRandomPositionJitters", _bladeRandomPositionJittersBuffer);
            _grassComputeShader.SetBuffer(_idMeshGeneratorKernal, "_bladeRandomColors", _bladeRandomColorsBuffer);

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
            #endregion
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
                _bladeRandomPositionJittersBuffer.Release();
                _bladeRandomColorsBuffer.Release();
            }

            _initialized = false;
        }

        private void LateUpdate()
        {
            /*
            if (_dispatched)
            {
                return;
            }
            */

            _drawBuffer.SetCounterValue(0);

            _grassComputeShader.SetMatrix("_localToWorld", transform.localToWorldMatrix);

            _grassComputeShader.Dispatch(_idMeshGeneratorKernal, _meshDispatchSize, 1, 1);

            ComputeBuffer.CopyCount(_drawBuffer, _argsBuffer, 0);

            _triToVertComputeShader.Dispatch(_idTriToVertKernal, 1, 1, 1);

            Graphics.DrawProceduralIndirect(_material, _localBounds, MeshTopology.Triangles, _argsBuffer, 0,
                null, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
            //_dispatched = true;
        }

        private void CalculateBounds()
        {
            var gridCenter = new Vector3(_gridStart.x, 1, _gridStart.y);
            gridCenter.x += _gridSize.x / 2;
            gridCenter.z += _gridSize.y / 2;
            var extents = new Vector3(_gridSize.x, 2.0f, _gridSize.y);
            _localBounds = new Bounds(gridCenter, extents);
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
    }
}
