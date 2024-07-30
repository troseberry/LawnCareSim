using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BladeGrassMeshSettings", menuName = "NedMakesGames/BladeGrassMeshSettings")]
public class BladeGrassBakeSettings : ScriptableObject {
    [Tooltip("The source mesh to build off of")]
    public Mesh sourceMesh;
    [Tooltip("The submesh index of the source mesh to use")]
    public int sourceSubMeshIndex;
    [Tooltip("A scale to apply to the source mesh before generating pyramids")]
    public Vector3 scale;
    [Tooltip("A rotation to apply to the source mesh before generating pyramids. Euler angles, in degrees")]
    public Vector3 rotation;
    [Tooltip("Grass blade height")]
    public float height;
    [Tooltip("Grass blade width")]
    public float width;
}