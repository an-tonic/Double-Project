using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class HandDataLoader : MonoBehaviour
{
    public Transform wrist;
    public string signName;
    private Dictionary<string, HandJointData> jointData;
    public float rotationAngle = -60.0f;

    void Start()
    {
        jointData = new Dictionary<string, HandJointData>();
    }

    public void LoadHandData(string letter = " ")
    {
        if (signName == null)
        {
            Debug.Log("Letter/sign not provided");
            return;
        }

        if (letter == " ")
        {
            letter = signName;
        }

        // Build the local file path using StreamingAssets
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Shape Data", letter.ToString().ToUpper() + ".txt");

        // Start the web request to fetch the hand data
        StartCoroutine(DownloadHandData(filePath));
    }

    // Coroutine to download the data asynchronously from the local file system
    private IEnumerator DownloadHandData(string filePath)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(filePath))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load hand data from: " + filePath);
                yield break;
            }

            // Parse the data from the response
            string responseText = webRequest.downloadHandler.text;
            string[] lines = responseText.Split('\n');

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

            ApplyHandData(wrist);
        }
    }

    private void ApplyHandData(Transform joint)
    {
        if (jointData.ContainsKey(joint.name.ToLower()))
        {
            joint.localPosition = jointData[joint.name.ToLower()].position;
            joint.localRotation = jointData[joint.name.ToLower()].rotation;

            if (joint.name.Contains("Metacarpal") || joint.name.Contains("Palm"))
            {
                joint.RotateAround(wrist.position, wrist.right, rotationAngle);
            }
        }

        foreach (Transform child in joint)
        {
            // Recursive update for each joint
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
