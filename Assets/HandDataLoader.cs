using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



public class HandDataLoader : MonoBehaviour
{
    public GameObject wrist;
    public char signName;
    private Dictionary<string, HandJointData> jointData;


    void Start()
    {
        jointData = new Dictionary<string, HandJointData>();
    }

    public void LoadHandData(char letter)
    {

        if (signName == null)
        {
            Debug.Log("Letter/sign not provided");
            return;
        }
        if (letter == null)
        {
            letter = signName;
        }

        string filePath = Path.Combine(Application.dataPath, "Shape Data", letter.ToString().ToUpper() + ".txt");

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found!");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);


        foreach (var line in lines)
        {
            if (line.Trim().Length == 0) { continue; }

            // Parse the data line, e.g., "R_IndexProximal: Pos(-0.003732, 0.002189, 0.059548) Rot(0.151882, -0.07698268, 0.0411778, 0.9845354)"
            string jointName = line.Split(':')[0].Trim().ToLower();
            string posString = line.Split("Pos(")[1].Split(')')[0];
            string rotString = line.Split("Rot(")[1].Split(')')[0];

            Vector3 position = ParseVector3(posString);
            Quaternion rotation = ParseQuaternion(rotString);
            
            jointData[jointName] = new HandJointData { position = position, rotation = rotation };

        }
        
        ApplyHandData(wrist.transform);
    }

    private void ApplyHandData(Transform joint)
    {
        
        if (jointData.ContainsKey(joint.name.ToLower()))
        {
            
            joint.localPosition = jointData[joint.name.ToLower()].position;
            joint.localRotation = jointData[joint.name.ToLower()].rotation;

        }

        foreach (Transform child in joint)
        {
            //Recursive update for each joint
            ApplyHandData(child);
        }
    }



    private Vector3 ParseVector3(string vectorString)
    {
        string[] values = vectorString.Split(',');
        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);
        return new Vector3(x, y, z);
    }

    private Quaternion ParseQuaternion(string quatString)
    {
        string[] values = quatString.Split(',');
        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);
        float w = float.Parse(values[3]);
        return new Quaternion(x, y, z, w);
    }
}

public class HandJointData
{
    public Vector3 position;
    public Quaternion rotation;
}

[CustomEditor(typeof(HandDataLoader))]
public class HandDataLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HandDataLoader recorder = (HandDataLoader)target;
        if (GUILayout.Button("Load Hand Data"))
        {
            recorder.LoadHandData('L');
        }
    }
}