using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static EmptySpell;


public class SpellBuilder : MonoBehaviour
{
    public Transform leftWrist;
    public Transform rightWrist;
    public Transform xrOrigin;
    

    private Spell currentSpell;

    void Start()
    {
        Spell teleportation = gameObject.AddComponent<Teleportation>();
        Spell empty = gameObject.AddComponent<EmptySpell>();
        Spell fire = gameObject.AddComponent<Fire>();

        empty.nextSpells = new List<Spell> { teleportation, fire };
        fire.nextSpells = new List<Spell> { empty };
        teleportation.nextSpells = new List<Spell> { empty };

        
        currentSpell = empty;
        

    }

    public void OnGestureRecognized(string gesture, string handedness)
    {
        gesture = gesture.ToLower();
        Log.L(gesture + " " + handedness);

        

        Spell nextSpell = currentSpell.ApplyEffect(gesture, rightWrist, leftWrist, xrOrigin);

        if(nextSpell != null)
        {
            currentSpell = nextSpell;
        }

    }

}

public abstract class Spell : MonoBehaviour
{
    public string[] letters;
    public List<Spell> nextSpells;

    public abstract Spell ApplyEffect(string letter, Transform targetRight, Transform targetLeft, Transform xrOrigin);

}

public class EmptySpell : Spell
{

    override
    public Spell ApplyEffect(string letter, Transform targetRight, Transform targetLeft, Transform xrOrigin)
    {
        foreach (Spell spell in nextSpells)
        {
            if (spell == null) continue;

            if (spell.letters[0] == letter)
            {
                spell.ApplyEffect(letter, targetRight, targetLeft, xrOrigin);
                return spell;
            }
        }
        return null;
    }
}


public class Fire : Spell
{

    
    private ParticleSystem fireEffect;
    private ParticleSystem flames;
    private ParticleSystem secondaryFlames;

    private int currentLetterIndex = 0;
    
    void Start()
    {
        letters = new string[] { "f", "i", "r", "e" };
        
    }
   

    override
    public Spell ApplyEffect(string letter, Transform targetRight, Transform targetLeft, Transform xrOrigin)
    {
        Log.L(letter);
        if(letter == "s" && fireEffect.gameObject != null )
        {
            Destroy(fireEffect.gameObject);
            fireEffect = null;
            currentLetterIndex = 0;
            return nextSpells[0];
        }

        if (Array.IndexOf(letters, letter) != currentLetterIndex) return null;

        if (letter == letters[0])
        {
            Vector3 offset = -targetRight.up * 0.05f + targetRight.forward * 0.1f;
            fireEffect = Instantiate(Resources.Load<ParticleSystem>("Effects/Fire"), targetRight.position + offset, targetRight.rotation * Quaternion.Euler(180, 0, 0));
            
            fireEffect.transform.SetParent(targetRight);

            flames = fireEffect.transform.Find("Flames").GetComponent<ParticleSystem>();
            secondaryFlames = fireEffect.transform.Find("Flames Secondary").GetComponent<ParticleSystem>();
        }

        var emission = flames.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(20f * (currentLetterIndex + 1));
        emission = secondaryFlames.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(20f * (currentLetterIndex + 1));
        currentLetterIndex++;
        return this;

    }


}


public class Teleportation : Spell
{
    private int currentLetterIndex = 0;
    private GameObject TP_Line;

    void Start()
    {
        letters = new string[] { "g", "f" };
        TP_Line = Instantiate(Resources.Load<GameObject>("Effects/TP_Line"));
    }

    override
    public Spell ApplyEffect(string letter, Transform targetRight, Transform targetLeft, Transform xrOrigin)
    {

        if (letter == "s")
        {
            TP_Line.SetActive(false);
            currentLetterIndex = 0;
            return nextSpells[0];
        }
        if (Array.IndexOf(letters, letter) != currentLetterIndex) return null;

        if (letter == letters[0])
        {
            TP_Line.GetComponent<ObjectSmoothing>().handTransform = targetLeft;
            TP_Line?.SetActive(true);
            currentLetterIndex += 1;
            return null;
        }
        else if (letter == letters[1] && CheckRaycast(out Vector3 hitPoint))
        {
            xrOrigin.position = hitPoint;

            //Uncomment for automatic TP line resetting
            //TP_Line.SetActive(false);
            //currentLetterIndex = 0;
            //return nextSpells[0];
        }
        return null;
    }


    private bool CheckRaycast(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        if (!TP_Line) return false;
        Ray ray = new Ray(TP_Line.transform.position, TP_Line.transform.up);
        LayerMask teleportLayer = 1 << LayerMask.NameToLayer("TeleportationLayer");
        if (Physics.Raycast(ray, out RaycastHit hitInfo, TP_Line.transform.localScale.y, teleportLayer) &&
            hitInfo.collider.CompareTag("TeleportSurface"))
        {
            hitPoint = hitInfo.point;
            return true;
        }
        return false;
    }

}