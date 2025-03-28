using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPLineBehaviour : SpellBehaviourBase
{
    private readonly Vector3 handOffset = new Vector3(0.00f, -0.12f, 0.10f);
    public GameObject indicatorInstance;
    public LayerMask teleportationLayer;

    override
    public void Initialize(Transform target)
    {
        GetComponent<TPLineSmoothing>().Initialize(target, handOffset);
    }


    override
    public void StopCast()
    {
        Destroy(this.gameObject);
    }


    void Update()
    {

        Vector3 start = transform.position;
        Vector3 direction = transform.rotation * Vector3.forward;

        if (Physics.Raycast(start, direction, out RaycastHit hit, transform.localScale.z, teleportationLayer))
        {
            indicatorInstance.transform.position = hit.point + Vector3.up * 0.01f;

            indicatorInstance.SetActive(true);
        }
        else
        {
            indicatorInstance.SetActive(false);
        }
    }
}
