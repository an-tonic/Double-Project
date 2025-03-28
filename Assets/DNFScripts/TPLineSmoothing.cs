using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TPLineSmoothing : MonoBehaviour
{
    private Transform targetTransform; 
    public float smoothingFactor = 0.1f;
    private Vector3 trackingPoint;


    public void Initialize(Transform target, Vector3 offset)
    {
        targetTransform = target;
        trackingPoint = offset;
        transform.rotation = targetTransform.rotation;
    }

    void Update()
    {
   
        transform.position = targetTransform.TransformPoint(trackingPoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, smoothingFactor);

    }

}
