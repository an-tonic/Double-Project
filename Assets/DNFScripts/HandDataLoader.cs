using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



public class HandDataLoader : MonoBehaviour
{
    
    
    private Dictionary<string, Quaternion> jointRotation;
    

    public string signName;
    public float rotationAngle = -60.0f;

    void Start()
    {
        jointRotation = new Dictionary<string, Quaternion>();
    }

    public void LoadHandData(Transform wrist, string letter)
    {   
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Shape Data", letter.ToString().ToUpper() + ".txt");
        
        StartCoroutine(DownloadHandData(filePath, wrist));
    }


    private IEnumerator DownloadHandData(string filePath, Transform wrist)
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
                string jointName = line.Split(':')[0].Split('_')[1].Trim().ToLower();
                string rotString = line.Split("Rot(")[1].Split(')')[0];

                
                Quaternion rotation = ParseQuaternion(rotString);

                jointRotation[jointName] = rotation;
            }

            ApplyHandData(wrist, wrist);
        }
    }

    private void ApplyHandData(Transform joint, Transform wrist)
    {
        string jointName = joint.name.Split('_')[1].ToLower();
        
        if (jointRotation.ContainsKey(jointName))
        {
            joint.localRotation = jointRotation[jointName];

            if (jointName.Contains("metacarpal") || jointName.Contains("palm"))
            {
                joint.RotateAround(wrist.position, wrist.right, rotationAngle);
            }
        }

        foreach (Transform child in joint)
        {
            ApplyHandData(child, wrist);
        }
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
