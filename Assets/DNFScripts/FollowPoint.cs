using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;

    public bool updateX;
    public bool updateY;
    public bool updateZ;
    
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;


    [Range(0.005f, 1f)]
    public float smoothing;
    [Range(0f, 15f)]
    public float rotationDeadZone;

    private float offsetY;

    void Start()
    {
        offsetY = Mathf.Abs(Mathf.Atan2(targetOffset.x, targetOffset.z) * Mathf.Rad2Deg);
    }

    void Update()
    {
 
        float headY = target.eulerAngles.y;

        if (rotationDeadZone > 0)
        {
            Vector3 diffVec = transform.position - target.position;
            float bookY = Mathf.Atan2(diffVec.x, diffVec.z) * Mathf.Rad2Deg;
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(headY, bookY));
            if (Mathf.Abs(angleDiff - offsetY) < rotationDeadZone)
            {
                return;
            }
        }
        


        Quaternion yRotation = Quaternion.Euler(0, headY, 0);
        Vector3 targetPos = target.position + yRotation * targetOffset;

        Vector3 newPos = transform.position;
        
        if (updateX) newPos.x = targetPos.x;
        if (updateY) newPos.y = targetPos.y;
        if (updateZ) newPos.z = targetPos.z;


        if (targetOffset.y > 0) newPos.y = targetOffset.y;

        //If smoothing is not enabled (i.e. instant lerp) then use dynamic
        float dynamicSmoothing = smoothing;
        if (smoothing != 1)
        {
            float distance = Vector3.Distance(transform.position, newPos);
            dynamicSmoothing = Mathf.Clamp(distance * 0.3f, smoothing, 1f);
        }
        
        
        transform.position = Vector3.Lerp(transform.position, newPos, dynamicSmoothing);


        Vector3 currentEuler = transform.localEulerAngles;
        Vector3 targetEuler = target.localEulerAngles;
        Vector3 newEuler = new Vector3(
            rotateX ? targetEuler.x : currentEuler.x,
            rotateY ? targetEuler.y : currentEuler.y,
            rotateZ ? targetEuler.z : currentEuler.z
        );

        //transform.localRotation = Quaternion.Euler(newEuler);
        transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newEuler), dynamicSmoothing);
        
    }
}
