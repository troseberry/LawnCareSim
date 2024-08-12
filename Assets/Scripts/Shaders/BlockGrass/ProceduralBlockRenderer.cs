using System;
using UnityEngine;

public class ProceduralBlockRenderer : MonoBehaviour
{
    [Tooltip("A mesh to extrude pyramids from")]
    [SerializeField] private Mesh _sourceMesh = default;

    [Tooltip("The geometry creating compute shader")]
    [SerializeField] private ComputeShader _blockComputeShader = default;

    [Tooltip("The traignle count adjustment compute shader")]
    [SerializeField] private ComputeShader _triToVertComputeShader = default;

    [Tooltip("The material to render the pyramid mesh")]
    [SerializeField] private Material _material = default;

    [Tooltip("The  height of the blocks")]
    [SerializeField] private float _blockHeight = 1;

    //[ToolTip("Whether the block should cast shadows")]
    //[SerializeField] private float _animationFrequency = 1;

    // The structure to send to the compute shader
    // The layout kind assures that the data is laid out sequentially
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct SourceVertex
    {
        public Vector3 Position;
        public Vector2 UV;
    }

    // A state variable to help keep track of whether compute buffers have been set up
    private bool _initialized;

    // A compute buffer to hold vertex data of the source mesh
    private ComputeBuffer _sourceVertexBuffer;

    // A compute buffer to hold index data of the source mesh
    private ComputeBuffer _sourceTriBuffer;

    // A compute buffer to hold vertex data of the generated mesh
    private ComputeBuffer _drawBuffer;

    // A compute buffer to hold indirect draw arguments
    private ComputeBuffer _argsBuffer;

    // The id of the kernel in the block compute shader
    private int _idBlockKernal;

    // The id of the kernel in the tri to vert count compute shade
    private int _idTriToVertKernal;

    // The number of thread groups to run when dispatching this shader
    private int _dispatchSize;

    // The local bounds of the generated mesh
    private Bounds _localBounds;

    // The size of one entry into the various compute buffers
    private const int SOURCE_VERTEX_STRIDE = sizeof(float) * (3 + 2);
    private const int SOURCE_TRI_STRIDE = sizeof(int);
    private const int DRAW_STRIDE = sizeof(float) * (3 + (3 + 2) * 3);
    private const int ARGS_STRIDE = sizeof(int) * 4;

    private void OnEnable()
    {
        if (_initialized)
        {
            OnDisable();
        }

        _initialized = true;

        // Grab data from the source mesh
        Vector3[] positions = _sourceMesh.vertices;
        Vector2[] uvs = _sourceMesh.uv;
        int[] tris = _sourceMesh.triangles;

        // Create the data to upload to the source vert buffer
        SourceVertex[] vertices = new SourceVertex[positions.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new SourceVertex()
            {
                Position = positions[i],
                UV = uvs[i]
            };
        }
        int numTriangles = tris.Length / 3;     // The number of triangles in the source mesh is the index array / 3

        // Create compute buffers
        // The stride is the size, in bytes, each object in the buffer takes up
        _sourceVertexBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERTEX_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        _sourceVertexBuffer.SetData(vertices);

        _sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        _sourceTriBuffer.SetData(tris);

        // Split each triagnle into three new ones
        _drawBuffer = new ComputeBuffer(numTriangles * 3, DRAW_STRIDE, ComputeBufferType.Append);
        _drawBuffer.SetCounterValue(0);     //Set the count to zero

        //The data in the args buffer cooresponds to:
        // 0: vertext count per draw instance. We will only use one instance
        // 1: instance count. One
        // 2: start vertex location if using a Graphics buffer
        // 3: start instance location if using a Graphics Buffer
        _argsBuffer = new ComputeBuffer(1, ARGS_STRIDE, ComputeBufferType.IndirectArguments);
        _argsBuffer.SetData(new int[] { 0, 1, 0, 0 });

        // Cache the kernel IDs we will be dispatchign
        _idBlockKernal = _blockComputeShader.FindKernel("CSMain");
        _idTriToVertKernal = _triToVertComputeShader.FindKernel("CSMain");

        // Set data on the shaders
        _blockComputeShader.SetBuffer(_idBlockKernal, "_sourceVertices", _sourceVertexBuffer);
        _blockComputeShader.SetBuffer(_idBlockKernal, "_sourceTriangles", _sourceTriBuffer);
        _blockComputeShader.SetBuffer(_idBlockKernal, "_drawTriangles", _drawBuffer);
        _blockComputeShader.SetInt("_numSourceTriangles", numTriangles);

        _triToVertComputeShader.SetBuffer(_idTriToVertKernal, "_indirectArgsBuffer", _argsBuffer);

        _material.SetBuffer("_drawTriangles", _drawBuffer);

        // Calculate the number of threads to use. Get the thread size from the kernel
        // Then, divide the number of triangles by that size
        _blockComputeShader.GetKernelThreadGroupSizes(_idBlockKernal, out uint threadGroupSize, out _, out _);
        _dispatchSize = Mathf.CeilToInt((float)numTriangles / threadGroupSize);

        _localBounds = _sourceMesh.bounds;
        _localBounds.Expand(_blockHeight);
    }

    private void OnDisable()
    {
        // Dispose of buffers
        if (_initialized)
        {
            _sourceVertexBuffer.Release();
            _sourceTriBuffer.Release();
            _drawBuffer.Release();
            _argsBuffer.Release();
        }

        _initialized = false;
    }

    private void LateUpdate()
    {
        // Clear the draw buffer of last frame's data
        _drawBuffer.SetCounterValue(0);

        // Transform the bounds to world space
        Bounds bounds = TransformBounds(_localBounds);

        // Update the shader with frame specific data
        _blockComputeShader.SetMatrix("_localToWorld", transform.localToWorldMatrix);
        _blockComputeShader.SetFloat("_blockHeight", _blockHeight /** Mathf.Sin(_animationFrequency * Time.timeSinceLevelLoad) */);

        // Dispatch the block shader. It will run on the GPU
        _blockComputeShader.Dispatch(_idBlockKernal, _dispatchSize, 1, 1);

        // Copy the count (stack size) of the draw buffer to the args buffer, at byte position zero
        // This sets the vertex count for our draw procedural indirect call
        ComputeBuffer.CopyCount(_drawBuffer, _argsBuffer, 0);

        // This is the compute shader outputs triangles, but the grpahics shader needs the number of vertices,
        // we need to multiply the vertex cound by three. We'll do this on the GPU with a compute shader
        // so we don't have to transfer data back to the CPU
        _triToVertComputeShader.Dispatch(_idTriToVertKernal, 1, 1, 1);

        Graphics.DrawProceduralIndirect(_material, bounds, MeshTopology.Triangles, _argsBuffer, 0,
            null, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
    }

    public Bounds TransformBounds(Bounds boundsOS)
    {
        var center = transform.TransformPoint(boundsOS.center);

        // Tranform the local extents' axes
        var extents = boundsOS.extents;
        var axisX = transform.TransformVector(extents.x, 0, 0);
        var axisY = transform.TransformVector(0, extents.y, 0);
        var axisZ = transform.TransformVector(0, 0, extents.z);

        // Sum their absolute values to get the world extents
        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds { center = center, extents = extents };
    }
}