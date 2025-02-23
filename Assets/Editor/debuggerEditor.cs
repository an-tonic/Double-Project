using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(debugger))]
public class debuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        debugger deb = (debugger)target;
        deb.inputText = EditorGUILayout.TextField("Input Text", deb.inputText);
        deb.isRight = EditorGUILayout.Toggle("Is Right", deb.isRight);

        if (GUILayout.Button("Debug"))
        {
            deb.OnButtonPress();
        }
    }
}