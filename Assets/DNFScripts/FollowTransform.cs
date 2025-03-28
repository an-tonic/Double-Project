using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;
    public float smoothingFactor = 0.1f;
    private Vector3 trackingPoint;

    public void Initialize(Transform target, Vector3 offset)
    {
        targetTransform = target;
        trackingPoint = offset;
    }

    void Start()
    {
        if (targetTransform == null) return;
        transform.position = targetTransform.TransformPoint(trackingPoint);
        transform.rotation = targetTransform.rotation;
    }

    void Update()
    {
        if (targetTransform == null) return;

        Vector3 targetPosition = targetTransform.TransformPoint(trackingPoint);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingFactor);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, smoothingFactor);

    }
}
