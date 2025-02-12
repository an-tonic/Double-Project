using UnityEngine;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;

public class HandPositionController : MonoBehaviour
{
    public Transform wrist;
    public Transform thumbRoot, indexRoot, middleRoot, ringRoot, pinkyRoot;
    public float scale = 1f;


    private Transform[][] fingers;
    private List<Vector3> landmarks = new List<Vector3>
        {
            new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-0.0736318826675415f, -0.03350412845611572f, -0.005335051396286872f), new Vector3(-0.1276523470878601f, -0.09729743003845215f, -0.008053318892962125f), new Vector3(-0.13047075271606445f, -0.1648356318473816f, -0.016170078252798703f), new Vector3(-0.09341615438461304f, -0.19653910398483276f, -0.01994583692157903f), new Vector3(-0.09578585624694824f, -0.16711515188217163f, 0.01804302696382365f), new Vector3(-0.12563258409500122f, -0.2215903401374817f, -0.010026107837802556f), new Vector3(-0.11248642206192017f, -0.15640825033187866f, -0.022042381872779515f), new Vector3(-0.09796428680419922f, -0.13650959730148315f, -0.02595547336841264f), new Vector3(-0.061030447483062744f, -0.17671674489974976f, 0.008352298925274226f), new Vector3(-0.09116053581237793f, -0.22428035736083984f, -0.021960416172987607f), new Vector3(-0.07808542251586914f, -0.14915388822555542f, -0.02281194861734548f), new Vector3(-0.06164395809173584f, -0.13695740699768066f, -0.013675878120011475f), new Vector3(-0.024318218231201172f, -0.18164193630218506f, -0.007501244840568688f), new Vector3(-0.056481778621673584f, -0.221918523311615f, -0.036488314454800275f), new Vector3(-0.04515355825424194f, -0.14814847707748413f, -0.022206432481652882f), new Vector3(-0.0279768705368042f, -0.13389581441879272f, -0.0027198792131457594f), new Vector3(0.01783043146133423f, -0.18087095022201538f, -0.02483848232532182f), new Vector3(-0.01980280876159668f, -0.2086755633354187f, -0.03709882993007341f), new Vector3(-0.01647394895553589f, -0.1564929485321045f, -0.023474552964216855f), new Vector3(-0.00029021501541137695f, -0.1408732533454895f, -0.008268544745988038f),

        };

    void Start()
    {
        // Get child bones automatically
        fingers = new Transform[][] {
            GetFingerBones(thumbRoot),
            GetFingerBones(indexRoot),
            GetFingerBones(middleRoot),
            GetFingerBones(ringRoot),
            GetFingerBones(pinkyRoot)
        };
        
    }

    void Update()
    {
        ApplyHandTracking(landmarks);

    }

    public void ApplyHandTracking(List<Vector3> landmarks)
    {
        if (landmarks.Count != 21) return;

        wrist.position += landmarks[0];

        // Apply positions for each finger
        ApplyFingerPositions(fingers[0], landmarks, 1);
        ApplyFingerPositions(fingers[1], landmarks, 5);
        ApplyFingerPositions(fingers[2], landmarks, 9);
        ApplyFingerPositions(fingers[3], landmarks, 13);
        ApplyFingerPositions(fingers[4], landmarks, 17);
    }

    private Transform[] GetFingerBones(Transform root)
    {
        List<Transform> bones = new List<Transform>();
        Transform current = root;
        while (current != null)
        {
            bones.Add(current);
            if (current.childCount > 0)
            {
                current = current.GetChild(0);
            }
            else
            {
                break;
            }
        }
        return bones.ToArray();
    }

    private void ApplyFingerPositions(Transform[] fingerBones, List<Vector3> landmarks, int startIndex)
    {
        for (int i = 0; i < fingerBones.Length-1; i++)
        {

            
            fingerBones[i].localPosition = landmarks[startIndex + i];
            Vector3 currentPos = landmarks[i];
            Vector3 nextPos = landmarks[i + 1];

            // Calculate direction from current joint to the next
            Vector3 direction = nextPos - currentPos;

            // Calculate rotation needed to look at the next joint
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Apply rotation to the bone
            fingerBones[i].rotation = rotation;
            

        }
    }
}
