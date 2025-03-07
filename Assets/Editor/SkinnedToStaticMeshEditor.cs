#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkinnedToStaticMesh))]
public class SkinnedToStaticMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkinnedToStaticMesh script = (SkinnedToStaticMesh)target;

        if (GUILayout.Button("Convert & Save"))
        {
            script.ConvertAndSave();
        }
    }
}
#endif
