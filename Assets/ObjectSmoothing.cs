using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjectSmoothing : MonoBehaviour
{
    public Transform handTransform = null;
    public Transform objectToSmooth;
    public float smoothingFactor = 0.1f;
    public float offsetAmount = 0.15f;

    private Vector3 smoothedEndPoint;

    void Start()
    {
        if (handTransform == null) return;
        objectToSmooth.position = handTransform.position + handTransform.up * -offsetAmount + handTransform.forward * offsetAmount;
        
    }

    void Update()
    {
        if (handTransform == null) return;

        Vector3 targetPosition = handTransform.position + handTransform.up * -offsetAmount + handTransform.forward * offsetAmount;
        objectToSmooth.position = Vector3.Lerp(objectToSmooth.position, targetPosition, smoothingFactor);


    }

}
