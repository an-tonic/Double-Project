using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightBehaviour : SpellBehaviourBase
{


    private readonly Vector3 handOffset = new Vector3(0.00f, -0.12f, 0.10f);

    override
    public void Initialize(Transform target)
    {
        GetComponent<FollowTransform>().Initialize(target, handOffset);
    }


    override
    public void StopCast()
    {
        Destroy(this.gameObject);
    }

    override
    public void AdvanceSpell(int intensity)
    {
        transform.Find("Light").GetComponent<UnityEngine.Light>().intensity = 1.0f + intensity * 0.3f;
        transform.Find("Light").GetComponent<UnityEngine.Light>().range = 4.0f + intensity * 0.25f;
        transform.Find("Halo Large").GetComponent<UnityEngine.Light>().range = 0.2f + intensity * 0.02f;

    }
 
}
