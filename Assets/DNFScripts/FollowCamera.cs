using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform target;
    public float smoothing = 0.1f;
    private Vector3 offset;

    void Start()
    {
        target = Camera.main.transform;
        offset = target.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        if (!target) return;
        transform.position = Vector3.Lerp(transform.position, target.TransformPoint(offset), smoothing);
        transform.rotation = target.rotation;
    }
}
