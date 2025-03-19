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
            Debug.LogError("All properties must be assigned.");
            return;
        }

        string filePath = Path.Combine(Application.streamingAssetsPath, "Shape Data", signName.Trim() + ".txt");
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {

            RecordTransformData(handTransform, writer);

            writer.WriteLine();
        }

        filePath = Path.Combine(Application.streamingAssetsPath, "Shape Data", "allFiles.txt");
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(signName + ".txt");
        }

        Debug.Log($"Hand data saved to {filePath}");
    }

    private void RecordTransformData(Transform parent, StreamWriter writer)
    {
        foreach (Transform joint in parent)
        {
            if (!(joint.name.StartsWith("R_") || joint.name.StartsWith("L_"))) continue;

            Vector3 position = joint.localPosition;
            Quaternion rotation = joint.localRotation;

            writer.WriteLine($"{joint.name}: Rot({rotation.x}, {rotation.y}, {rotation.z}, {rotation.w})");


            // Recursively get child joints
            RecordTransformData(joint, writer);
        }
    }
}