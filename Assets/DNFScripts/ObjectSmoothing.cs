using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjectSmoothing : MonoBehaviour
{
    public Transform handTransform; 
    public Transform teleportationLine;
    public float smoothingFactor = 0.1f; 
    public float offsetAmount = 0.5f;

    private Vector3 smoothedEndPoint;


    void OnEnable()
    {
        teleportationLine.position = handTransform.position + handTransform.up * -offsetAmount + handTransform.forward * offsetAmount;
        teleportationLine.rotation = handTransform.rotation * Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {
        teleportationLine.position = handTransform.position + handTransform.up * -offsetAmount + handTransform.forward * offsetAmount;

        Quaternion targetRotation = handTransform.rotation * Quaternion.Euler(90, 0, 0);
        teleportationLine.rotation = Quaternion.Slerp(teleportationLine.rotation, targetRotation, smoothingFactor);

    }

}
