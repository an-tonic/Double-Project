using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool updateX;
    public bool updateY;
    public bool updateZ;
    
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

    public float smoothing = 0.1f;

    void LateUpdate()
    {

        float yAngle = target.eulerAngles.y;
        Quaternion yRotation = Quaternion.Euler(0, yAngle, 0);
        Vector3 targetPos = target.position + yRotation * offset;

        // Update position on selected axes
        Vector3 newPos = transform.position;
        if (updateX) newPos.x = targetPos.x;
        if (updateY) newPos.y = targetPos.y;
        if (updateZ) newPos.z = targetPos.z;
        transform.position = Vector3.Lerp(transform.position, newPos, smoothing);


        Vector3 currentEuler = transform.localEulerAngles;
        Vector3 targetEuler = target.localEulerAngles;
        Vector3 newEuler = new Vector3(
            rotateX ? targetEuler.x : currentEuler.x,
            rotateY ? targetEuler.y : currentEuler.y,
            rotateZ ? targetEuler.z : currentEuler.z
        );

        //transform.localRotation = Quaternion.Euler(newEuler);
        transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newEuler), smoothing);

    }
}
