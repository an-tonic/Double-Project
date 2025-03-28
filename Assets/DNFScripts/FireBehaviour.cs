using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FireBehaviour : SpellBehaviourBase
{

    private readonly Vector3 handOffset = new Vector3(0.00f, -0.06f, 0.10f);

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

        var emission = transform.Find("Flames").GetComponent<ParticleSystem>().emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(20f * (intensity + 1));

        emission = transform.Find("Flames Secondary").GetComponent<ParticleSystem>().emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(20f * (intensity + 1));
    }


    


}
