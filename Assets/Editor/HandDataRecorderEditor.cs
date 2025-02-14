using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandDataRecorder))]
public class HandDataRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HandDataRecorder recorder = (HandDataRecorder)target;
        if (GUILayout.Button("Record Hand Data"))
        {
            recorder.RecordHandData();
        }
    }
}