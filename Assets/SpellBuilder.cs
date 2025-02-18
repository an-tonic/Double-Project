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
    public LearningStateManager learningStateManager;

    private Spell currentSpell;

    void Start()
    {
        Spell teleportation = gameObject.AddComponent<Teleportation>();
        teleportation.Initialize(rightWrist, leftWrist, xrOrigin);
        Spell fire = gameObject.AddComponent<Fire>();
        fire.Initialize(rightWrist, leftWrist, xrOrigin);
        Spell light = gameObject.AddComponent<Light>();
        light.Initialize(rightWrist, leftWrist, xrOrigin);

        Spell empty = gameObject.AddComponent<EmptySpell>();


        empty.nextSpells = new List<Spell> { teleportation, fire, light };

        fire.nextSpells = new List<Spell> { empty };
        teleportation.nextSpells = new List<Spell> { empty };
        light.nextSpells = new List<Spell> { empty };

        currentSpell = empty;


    }

    public void OnGestureRecognized(string gesture, string handedness)
    {
        learningStateManager.ChangeState(gesture, handedness);

        gesture = gesture.ToLower();
        Log.L(gesture + " " + handedness);

        Spell nextSpell = currentSpell.ApplyEffect(gesture, handedness);

        if (nextSpell != null)
        {
            currentSpell = nextSpell;
        }

    }

}
public static class LetterExtensions
{
    public static bool IsFirst(this List<(string Sign, string Hand)> letters, string letter)
    {
        return letters.Count > 0 && letters[0].Sign == letter;
    }
}


public abstract class Spell : MonoBehaviour
{
    //public string[] letters;
    public List<Spell> nextSpells;
    public List<(string Sign, string Hand)> letters;

    
    protected Transform targetRight;
    protected Transform targetLeft;
    protected Transform xrOrigin;

    public void Initialize(Transform targetR, Transform targetL, Transform targetXRorigin)
    {
        targetRight = targetR;
        targetLeft = targetL;
        xrOrigin = targetXRorigin;
    }

    public abstract Spell ApplyEffect(string letter, string handedness);

}

public class EmptySpell : Spell
{

    override
    public Spell ApplyEffect(string letter, string handedness)
    {
        foreach (Spell spell in nextSpells)
        {
            if (spell == null) continue;

            if (spell.letters.IsFirst(letter))
            {
                spell.ApplyEffect(letter, handedness);
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

        letters = new List<(string Sign, string Hand)>
        {
            ("f", "Right"),
            ("i", "Right"),
            ("r", "Right"),
            ("e", "Right")
        };
    }


    override
    public Spell ApplyEffect(string letter, string handedness)
    {
        if (letter == "s" && fireEffect.gameObject != null)
        {
            Destroy(fireEffect.gameObject);
            fireEffect = null;
            currentLetterIndex = 0;
            return nextSpells[0];
        }

        if (letters[currentLetterIndex].Sign != letter || letters[currentLetterIndex].Hand != handedness) return null;


        if (letters.IsFirst(letter))
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


public class Light : Spell
{

    private int currentLetterIndex = 0;
    private GameObject lightObject;
    void Start()
    {

        letters = new List<(string Sign, string Hand)>
        {
            ("l", "Any"),
            ("i", "Any"),
            ("g", "Any"),
            ("h", "Any"),
            ("t", "Any"),
        };
    }


    override
    public Spell ApplyEffect(string letter, string handedness)
    {
        Log.L("L enter");

        if (letter == "s" && lightObject != null)
        {
            Destroy(lightObject);
            lightObject = null;
            currentLetterIndex = 0;
            return nextSpells[0];
        }

        if (letters[currentLetterIndex].Sign != letter) return null;

        Log.L(currentLetterIndex);
        if (letters.IsFirst(letter))
        {
            Log.L("iS first");
            //Vector3 offset = -targetRight.up * 0.15f + targetRight.forward * 0.1f;
            Transform tragetHand = handedness == "Right" ? targetRight : targetLeft;
            Log.L(tragetHand);
            lightObject = Instantiate(Resources.Load<GameObject>("Effects/Light"));
            lightObject.GetComponent<ObjectSmoothing>().handTransform = tragetHand;
            //gameObject.transform.SetParent(targetRight);

        }
        Log.L("after");
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
        letters = new List<(string Sign, string Hand)>
        {
            ( "g", "Left" ),
            ( "f", "Right" )
        };

        TP_Line = Instantiate(Resources.Load<GameObject>("Effects/TP_Line"));
        TP_Line.SetActive(false);
    }

    override
    public Spell ApplyEffect(string letter, string handedness)
    {

        if (letter == "s")
        {
            TP_Line.SetActive(false);
            currentLetterIndex = 0;
            return nextSpells[0];
        }

        if (letters[currentLetterIndex].Sign != letter || letters[currentLetterIndex].Hand != handedness) return null;

        if (letters.IsFirst(letter))
        {
            TP_Line.GetComponent<TPLineSmoothing>().handTransform = targetLeft;
            TP_Line?.SetActive(true);
            currentLetterIndex += 1;
            return null;
        }
        else if (letter == letters[1].Sign && CheckRaycast(out Vector3 hitPoint))
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