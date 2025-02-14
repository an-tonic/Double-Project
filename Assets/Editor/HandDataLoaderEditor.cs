using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandDataLoader))]
public class HandDataLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HandDataLoader recorder = (HandDataLoader)target;
        if (GUILayout.Button("Load Hand Data"))
        {
            recorder.LoadHandData();
        }
    }
}
