using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraXZ : MonoBehaviour
{
    public Transform cameraTransform;

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = cameraTransform.position.x;
        newPosition.z = cameraTransform.position.z;
        transform.position = newPosition;
    }
}
