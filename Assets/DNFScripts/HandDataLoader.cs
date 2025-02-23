using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class HandDataLoader : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, Quaternion>> handData = new Dictionary<string, Dictionary<string, Quaternion>>();
    public float rotationAngle = -60.0f;

    void Start()
    {
        
        //StartCoroutine(LoadAllHandData());
    }

    public IEnumerator LoadAllHandData()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Shape Data");
        string allFilesPath = Path.Combine(folderPath, "allFiles.txt");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(allFilesPath))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load allFiles.txt");
                yield break;
            }

            foreach (string fileName in webRequest.downloadHandler.text.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(fileName)) continue;

                string filePath = Path.Combine(folderPath, fileName.Trim());
                using (UnityWebRequest fileRequest = UnityWebRequest.Get(filePath))
                {
                    yield return fileRequest.SendWebRequest();

                    if (fileRequest.result != UnityWebRequest.Result.Success) continue;

                    Dictionary<string, Quaternion> jointRotation = new Dictionary<string, Quaternion>();
                    foreach (var line in fileRequest.downloadHandler.text.Split('\n'))
                    {
                        if (line.Trim().Length == 0) continue;
                        string jointName = line.Split(':')[0].Split('_')[1].Trim().ToLower();
                        string rotString = line.Split("Rot(")[1].Split(')')[0];
                        jointRotation[jointName] = ParseQuaternion(rotString);
                    }

                    string letter = fileName.Split('.')[0];
                    handData[letter] = jointRotation;
                }
            }
        }

        Debug.Log("All hand data loaded successfully.");
        
    }


    public void LoadHandData(Transform wrist, string letter)
    {
        string upperLetter = letter.ToUpper();
        if (!handData.ContainsKey(upperLetter))
        {
            Debug.LogError("No hand data found for letter: " + upperLetter);
            return;
        }

        Dictionary<string, Quaternion> jointRotation = handData[upperLetter];
        ApplyHandData(wrist, wrist, jointRotation);
    }

    private void ApplyHandData(Transform joint, Transform wrist, Dictionary<string, Quaternion> jointRotation)
    {
        string jointName = joint.name.Split('_')[1].ToLower();

        if (jointRotation.ContainsKey(jointName))
        {
            joint.localRotation = jointRotation[jointName];

            //if (jointName.Contains("metacarpal") || jointName.Contains("palm"))
            //{
            //    joint.RotateAround(wrist.position, wrist.right, rotationAngle);
            //}
        }

        foreach (Transform child in joint)
        {
            ApplyHandData(child, wrist, jointRotation);
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
