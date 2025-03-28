using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBallBehaviour : SpellBehaviourBase
{
    private readonly Vector3 handOffset = new Vector3(0.00f, -0.12f, 0.10f);

    private int spellLevel = 1;


    override
    public void Initialize(Transform target, int lightIntensity)
    {
        GetComponent<FollowTransform>().Initialize(target, handOffset);
        transform.Find("Light").GetComponent<UnityEngine.Light>().intensity = 1.0f + lightIntensity * 0.3f;
        transform.Find("Light").GetComponent<UnityEngine.Light>().range = 4.0f + lightIntensity * 0.25f;
    }


    override
    public void AdvanceSpell(int intensity)
    {
        spellLevel++;
        transform.Find("Sphere").transform.localScale *= (1 + intensity * 0.2f);
    }

    override
    public void StopCast()
    {
        Destroy(this.gameObject);
    }


    override
    public void ActivateSpell()
    {
        StartCoroutine(DepleteBall());
        GetComponent<FollowTransform>().enabled = false;
        GetComponent<FollowCamera>().enabled = true;
    }

    private IEnumerator DepleteBall()
    {
        Transform sphere = transform.Find("Sphere");
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        Vector3 initialScale = sphere.localScale;
        float duration = spellLevel * 20f;
        float timeElapsed = 0f;
        float blinkStart = duration * 0.7f;
        bool isVisible = true;

        while (timeElapsed < duration)
        {
            if (timeElapsed > blinkStart)
            {
                if (UnityEngine.Random.value > 0.6f)
                    isVisible = !isVisible;
                sphereRenderer.enabled = isVisible;
            }

            sphere.localScale = Vector3.Lerp(initialScale, Vector3.zero, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        sphere.localScale = Vector3.zero;
        StopCast();
    }
}
