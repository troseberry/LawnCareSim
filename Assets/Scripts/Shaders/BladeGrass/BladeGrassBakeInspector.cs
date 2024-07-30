using System.Linq;
using UnityEditor;
using UnityEngine;

// This is a custom inspector for BladeGrassBakeSettings
[CustomEditor(typeof(BladeGrassBakeSettings))]
public class BladeGrassBakeInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        // After drawing the default GUI, add a button to trigger mesh creation
        if(GUILayout.Button("Create")) {
            // Find the unique ID for our compute shader
            var shaderGUID = AssetDatabase.FindAssets("BladeGrassMeshBuilder").FirstOrDefault();
            if(string.IsNullOrEmpty(shaderGUID)) {
                Debug.LogError("Cannot find compute shader: BladeGrassMeshBuilder.compute");
            } else {
                // Turn the GUID into the actual compute shader object
                var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(shaderGUID));

                // Opens a progress bar window
                EditorUtility.DisplayProgressBar("Building mesh", "", 0);
                // Run the baker
                var settings = serializedObject.targetObject as BladeGrassBakeSettings;
                bool success = BladeGrassBaker.Run(shader, settings, out var generatedMesh);

                EditorUtility.ClearProgressBar();

                if(success) {
                    SaveMesh(generatedMesh);
                    Debug.Log("Mesh saved successfully");
                } else {
                    Debug.LogError("Failed to create mesh");
                }
            }
        }
    }

    private void SaveMesh(Mesh mesh) {
        // Opens a file save dialog window
        string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", name, "asset");
        // Path is empty if the user exits out of the window
        if(string.IsNullOrEmpty(path)) {
            return;
        }

        // Transforms the path to a full system path, to help minimize bugs
        path = FileUtil.GetProjectRelativePath(path);

        // Check if this path already contains a mesh
        // If yes, we want to replace that mesh with the baked mesh while keeping the same GUID,
        // so any other object using it will automatically update
        var oldMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if(oldMesh != null) {
            // Clear all mesh data on the old mesh, readying it to receive new data
            oldMesh.Clear();
            // Copy mesh data from the new mesh to the old mesh
            EditorUtility.CopySerialized(mesh, oldMesh);
        } else {
            // Nothing is at this path (or it wasn't a mesh), so create a new asset
            AssetDatabase.CreateAsset(mesh, path);
        }

        // Tell Unity to save all assets
        AssetDatabase.SaveAssets();
    }
}