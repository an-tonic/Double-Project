using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FireBallBehaviour : SpellBehaviourBase
{
    private readonly Vector3 handOffset = new Vector3(0.00f, -0.12f, 0.10f);
    private Rigidbody rb;
    
    public bool isActive = false;
    public float speed = 2f;
    public float acceleration = 10f;

    override
    public void Initialize(Transform target, int previousSpellIndex)
    {
        rb = GetComponent<Rigidbody>();

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
        transform.localScale *= (1 + intensity * 0.3f);
    }


    override
    public void ActivateSpell()
    {
        isActive = true;
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        GetComponent<FollowTransform>().enabled = false;
        Destroy(this.gameObject, 10f);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            transform.Find("Explosion").gameObject.SetActive(true);
            rb.velocity = Vector3.zero;
            GetComponent<Renderer>().enabled = false;
            Destroy(this.gameObject, 3f);
            isActive = false;
        }
    }

    void Update()
    {
        if (isActive)
        {
            rb.velocity += transform.forward * acceleration * Time.deltaTime;
        }
    }
}
