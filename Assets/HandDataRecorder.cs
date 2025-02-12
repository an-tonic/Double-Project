using UnityEngine;
using UnityEditor;
using System.IO;


public class HandDataRecorder : MonoBehaviour
{
    public Transform handTransform;
    
    public string signName;
    [ContextMenu("Record Hand Data")]
    public void RecordHandData()
    {
        if (handTransform == null || signName == null || signName.Trim() == "")
        {
            Debug.LogError("All propertiesmust be assigned.");
            return;
        }

        string filePath = Path.Combine(Application.dataPath, "Shape Data", signName.Trim() + ".txt");
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {

            RecordTransformData(handTransform, writer);

            writer.WriteLine();
        }

        Debug.Log($"Hand data saved to {filePath}");
    }

    private void RecordTransformData(Transform parent, StreamWriter writer)
    {
        foreach (Transform joint in parent)
        {
            Vector3 position = joint.localPosition;
            Quaternion rotation = joint.localRotation;
            if ((!joint.name.ToLower().Contains("palm") && !joint.name.ToLower().Contains("metacarpal")) || joint.name.ToLower().Contains("thumbmetacarpal"))
            {
                writer.WriteLine($"{joint.name}: Pos({position.x}, {position.y}, {position.z}) Rot({rotation.x}, {rotation.y}, {rotation.z}, {rotation.w})");
            }
            
            // Recursively get child joints
            RecordTransformData(joint, writer);
        }
    }
}

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
