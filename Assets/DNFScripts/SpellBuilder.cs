using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static EmptySpell;

public static class Constants
{
    public static readonly Vector3 handOffset = new Vector3(0.00f, -0.12f, 0.10f);
}

public class SpellBuilder : MonoBehaviour
{
    public Transform leftWrist;
    public Transform rightWrist;
    public Transform xrOrigin;

    public LearningStateManager learningStateManager;

    private Spell currentSpell;
    private Spell empty;
    private Spell nextSpell;
    void Start()
    {
        empty = gameObject.AddComponent<EmptySpell>();

        Spell teleportation = gameObject.AddComponent<Teleportation>();
        teleportation.Initialize(rightWrist, leftWrist, xrOrigin);
        Spell fire = gameObject.AddComponent<Fire>();
        fire.Initialize(rightWrist, leftWrist, xrOrigin);
        Spell light = gameObject.AddComponent<Light>();
        light.Initialize(rightWrist, leftWrist, xrOrigin);
        Spell ball = gameObject.AddComponent<Ball>();
        ball.Initialize(rightWrist, leftWrist, xrOrigin);



        empty.nextSpells = new List<Spell> { teleportation, fire, light };

        fire.nextSpells = new List<Spell> { };
        teleportation.nextSpells = new List<Spell> { };
        light.nextSpells = new List<Spell> { ball };
        ball.nextSpells = new List<Spell> { };
        currentSpell = empty;


    }


    public void OnGestureRecognized(string gesture, string handedness)
    {
        gesture = gesture.ToLower();
        learningStateManager.ChangeState(gesture, handedness);
        Log.L("Current spell: " + currentSpell);

        
        if (gesture == "s" && currentSpell.activeHand != handedness)
        {
            currentSpell.StopCast();
            currentSpell = empty;
            return;
        }

        if (gesture == "a" && currentSpell.activeHand != handedness)
        {

            currentSpell = currentSpell.ActivateSpell();
            Log.L(nextSpell);

            return;
        }

        if (currentSpell == null)
        {
            currentSpell = empty;
        }
        currentSpell = currentSpell.PrepareAndCast(gesture, handedness);

    }

}

public static class LetterExtensions
{
    public static bool IsFirst(this List<(string Sign, string Hand)> letters, string letter)
    {
        return letters.Count > 0 && letters[0].Sign == letter;
    }

    public static bool IsCoorectHand(this List<(string Sign, string Hand)> letters, int index, string hand)
    {
        return letters.Count > 0 && letters[index].Hand == hand;
    }

    public static bool IsCorrectLetter(this List<(string Sign, string Hand)> letters, int index, string letter)
    {
        return letters.Count > 0 && letters[index].Sign == letter;
    }
}

public class Vessel
{
    public GameObject main;
    public Material material;
    public GameObject effect;
    public int damage;
    public VesselBehaviour behaviour;

    public Vessel()
    {
        main = new GameObject("VesselMain");
    }
    public void AddBehavior<T>(Transform target, Vector3 offset) where T : VesselBehaviour
    {
        if (behaviour != null)
        {
            UnityEngine.Object.Destroy(behaviour);
        }
        behaviour = main.AddComponent<T>();
        behaviour.Initialize(target, offset);
    }

}

public abstract class Spell : MonoBehaviour
{

    public List<Spell> nextSpells;
    public List<(string Sign, string Hand)> letters;
    public string activeHand;

    protected Vessel vessel;
    protected int currentLetterIndex = 0;

    protected Transform targetRight;
    protected Transform targetLeft;
    protected Transform xrOrigin;

    protected Spell FindNextSpell(string letter, string handedness)
    {
        foreach (Spell spell in nextSpells)
        {
            if (spell == null) continue;

            if (spell.letters.IsFirst(letter) && (spell.letters[0].Hand == handedness || spell.letters[0].Hand == "Any"))
            {
                return spell;
            }
        }
        return null;
    }

    public void RecieveVessel(Vessel vessel)
    {
        this.vessel = vessel;
    }

    public void Initialize(Transform targetR, Transform targetL, Transform targetXRorigin)
    {
        targetRight = targetR;
        targetLeft = targetL;
        xrOrigin = targetXRorigin;
    }

    public Spell PrepareAndCast(string letter, string handedness)
    {
        if (currentLetterIndex > 0 && !letters.IsCorrectLetter(currentLetterIndex, letter))
        {
            Spell nextSpell = FindNextSpell(letter, handedness);
            if (nextSpell)
            {
                nextSpell.RecieveVessel(vessel);
                nextSpell.Cast(letter, handedness);
                currentLetterIndex = 0;
                return nextSpell;
            }
        }

        if (letters.Count > 0)
        {

            if (!letters.IsCorrectLetter(currentLetterIndex, letter))
            {
                return this;
            }
            if (!letters.IsCoorectHand(currentLetterIndex, "Any") && !letters.IsCoorectHand(currentLetterIndex, handedness))
            {
                return this;
            }

        }

        return Cast(letter, handedness);
    }

    public abstract Spell Cast(string letter, string handedness);

    public void StopCast()
    {
        if (vessel != null)
        {
            Destroy(vessel.main.gameObject);
            currentLetterIndex = 0;
        }
    }

    public virtual Spell ActivateSpell()
    {
        return this;
    }
}


public class EmptySpell : Spell
{

    void Start()
    {

        letters = new List<(string Sign, string Hand)>();
    }

    override
    public Spell Cast(string letter, string handedness)
    {
        Spell nextSpell = FindNextSpell(letter, handedness);

        if (nextSpell == null)
        {
            Log.L("Empty did not find spell for " + letter + " and hand " + handedness);
            return null;
        }
        nextSpell.PrepareAndCast(letter, handedness);
        return nextSpell;
    }
}


public class Fire : Spell
{
    private ParticleSystem flames;
    private ParticleSystem secondaryFlames;

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
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            vessel = new Vessel();
            activeHand = handedness;
            Vector3 offset = -targetRight.up * 0.05f + targetRight.forward * 0.1f;
            vessel.effect = Instantiate(Resources.Load<GameObject>("Effects/Fire"), vessel.main.transform.position, vessel.main.transform.rotation, vessel.main.transform);

            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;
            ObjectSmoothing smoothing = vessel.main.AddComponent<ObjectSmoothing>();
            smoothing.handTransform = targetHand;

            flames = vessel.effect.transform.Find("Flames").GetComponent<ParticleSystem>();
            secondaryFlames = vessel.effect.transform.Find("Flames Secondary").GetComponent<ParticleSystem>();
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
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            vessel = new Vessel();
            activeHand = handedness;
            vessel.effect = Instantiate(Resources.Load<GameObject>("Effects/Light"), vessel.main.transform.position, vessel.main.transform.rotation, vessel.main.transform);
            vessel.material = Resources.Load<Material>("Materials/Light Glow");
            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;

            vessel.AddBehavior<FollowTransform>(targetHand, Constants.handOffset);

        }


        vessel.effect.GetComponent<UnityEngine.Light>().intensity = 1.0f + currentLetterIndex * 0.2f;
        vessel.effect.GetComponent<UnityEngine.Light>().range = 2.0f + currentLetterIndex * 0.25f;
        vessel.effect.transform.Find("Halo Center").GetComponent<UnityEngine.Light>().range = 0.01f + currentLetterIndex * 0.01f;
        vessel.effect.transform.Find("Halo Large").GetComponent<UnityEngine.Light>().range = 0.1f + currentLetterIndex * 0.05f;
        currentLetterIndex++;
        return this;
    }

}


public class Ball : Spell
{

    void Start()
    {
        letters = new List<(string Sign, string Hand)>
        {
            ("b", "Any"),
            ("a", "Any"),
            ("l", "Any"),
            ("l", "Any"),
        };
    }

    override
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            activeHand = handedness;
            vessel.effect = Instantiate(Resources.Load<GameObject>("Effects/Ball"), vessel.main.transform.position, vessel.main.transform.rotation, vessel.main.transform);
            vessel.effect.GetComponent<MeshRenderer>().material = vessel.material;
        }

        vessel.effect.transform.localScale *= (1 + currentLetterIndex * 0.3f);

        currentLetterIndex++;
        return this;
    }

    public override Spell ActivateSpell()
    {
        Log.L("Ball activated");
        //Vector3 offset = Camera.main.transform.position - vessel.main.transform.position;
        Vector3 offset = Camera.main.transform.InverseTransformPoint(vessel.main.transform.position);
        vessel.AddBehavior<FollowTransform>(Camera.main.transform, offset);
        currentLetterIndex = 0;
        Destroy(vessel.main.gameObject, (currentLetterIndex + 1) * 20);
        return null;
    }

}


public class Teleportation : Spell
{

    void Start()
    {
        letters = new List<(string Sign, string Hand)>
        {
            ( "d", "Left" ),
            ( "f", "Right" )
        };

    }

    override
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            vessel = new Vessel();
            activeHand = handedness;
            vessel.effect = Instantiate(Resources.Load<GameObject>("Effects/TP_Line"), vessel.main.transform);
            vessel.effect.GetComponent<TPLineSmoothing>().handTransform = targetLeft;

            currentLetterIndex += 1;
        }
        else if (letter == letters[1].Sign && CheckRaycast(out Vector3 hitPoint))
        {
            xrOrigin.position = hitPoint;
            //StopCast();
        }
        return this;
    }


    private bool CheckRaycast(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        if (!vessel.effect) return false;
        Ray ray = new Ray(vessel.effect.transform.position, vessel.effect.transform.up);
        LayerMask teleportLayer = 1 << LayerMask.NameToLayer("TeleportationLayer");
        if (Physics.Raycast(ray, out RaycastHit hitInfo, vessel.effect.transform.localScale.y, teleportLayer) &&
            hitInfo.collider.CompareTag("TeleportSurface"))
        {
            hitPoint = hitInfo.point;
            return true;
        }
        return false;
    }

}


public abstract class VesselBehaviour : MonoBehaviour
{
    public virtual void Initialize(Transform target, Vector3 offset) { }
}

public class FollowTransform : VesselBehaviour
{
    private Transform targetTransform;
    public float smoothingFactor = 0.1f;
    private Vector3 trackingPoint;

    public override void Initialize(Transform target, Vector3 offset)
    {
        targetTransform = target;
        trackingPoint = offset;
    }

    void Start()
    {
        if (targetTransform == null) return;
        transform.position = targetTransform.TransformPoint(trackingPoint);
        transform.rotation = targetTransform.rotation;
    }

    void Update()
    {
        if (targetTransform == null) return;

        //Vector3 targetPosition = targetTransform.position + targetTransform.right * trackingPoint.x + targetTransform.up * trackingPoint.y + targetTransform.forward * -trackingPoint.z;
        Vector3 targetPosition = targetTransform.TransformPoint(trackingPoint);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingFactor);
        transform.rotation = targetTransform.rotation;

    }
}
