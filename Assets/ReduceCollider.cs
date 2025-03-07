using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceCollider : MonoBehaviour
{
    private void OnDestroy()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.radius *= 0.1f;
        }
    }
}
