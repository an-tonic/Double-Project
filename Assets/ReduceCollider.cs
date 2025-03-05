using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceCollider : MonoBehaviour
{
    private void OnDestroy()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        Log.L("reducing");
        if (sphereCollider != null)
        {
            sphereCollider.radius *= 0.1f;
            Log.L("reduced");
        }
    }
}
