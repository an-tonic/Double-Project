using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Constants
{
    public static readonly Vector3 handOffset = new Vector3(0.00f, -0.12f, 0.10f);
}

public class SpellBuilder : MonoBehaviour
{
    public Transform leftWrist;
    public Transform rightWrist;
    public Transform xrOrigin;

    [SerializeField]
    [Tooltip("The event fired when the gesture is performed.")]
    UnityEvent<string, string> m_GesturePerformed;

    private Spell currentSpell;
    private Spell empty;
    void Start()
    {
        empty = AddSpell<EmptySpell>();
        Spell teleportation = AddSpell<Teleportation>();
        Spell fire = AddSpell<Fire>();
        Spell light = AddSpell<Light>();
        Spell ball = AddSpell<Ball>();



        empty.nextSpells = new List<Spell> { teleportation, fire, light };

        fire.nextSpells = new List<Spell> { ball };
        teleportation.nextSpells = new List<Spell> { };
        light.nextSpells = new List<Spell> { ball };
        ball.nextSpells = new List<Spell> { };
        currentSpell = empty;


    }


    public void OnGestureRecognized(string gesture, string handedness)
    {
        gesture = gesture.ToLower();

        m_GesturePerformed?.Invoke(gesture, handedness);
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
            return;
        }

        if (currentSpell == null)
        {
            currentSpell = empty;
        }
        currentSpell = currentSpell.PrepareAndCast(gesture, handedness);

    }

    private Spell AddSpell<T>() where T : Spell
    {
        Spell s = gameObject.AddComponent<T>();
        s.Initialize(rightWrist, leftWrist, xrOrigin);
        return s;
    }
}

public static class LetterExtensions
{
    public static bool IsFirst(this List<(string Sign, string Hand)> letters, string letter) =>
        letters.Count > 0 && letters[0].Sign == letter;

    public static bool IsCoorectHand(this List<(string Sign, string Hand)> letters, int index, string hand) =>
        letters.Count > 0 && letters[index].Hand == hand;

    public static bool IsCorrectLetter(this List<(string Sign, string Hand)> letters, int index, string letter) =>
        letters.Count > 0 && index < letters.Count && letters[index].Sign == letter;
}

public class Vessel
{
    public GameObject main;
    public GameObject effect;
    public GameObject shape;
    public Material material;
    public int damage;
    public VesselBehaviour mainBehaviour;
    public VesselBehaviour secondaryBehaviour;

    public Vessel()
    {
        main = new GameObject("VesselMain");
    }
    public void AddBehavior<T>(Transform target = null, Vector3? offset = null) where T : VesselBehaviour
    {
        if (mainBehaviour != null)
        {
            UnityEngine.Object.Destroy(mainBehaviour);
        }
        mainBehaviour = main.AddComponent<T>();
        Vector3 finalOffset = offset ?? Vector3.zero;
        mainBehaviour.Initialize(target, finalOffset);
    }
    public void AddSecondaryBehavior<T>(bool enable = false) where T : VesselBehaviour
    {
        if (secondaryBehaviour != null)
        {
            UnityEngine.Object.Destroy(secondaryBehaviour);
        }
        secondaryBehaviour = main.AddComponent<T>();
        secondaryBehaviour.enabled = enable;

    }

    public void DestroyAfter(MonoBehaviour caller, float seconds)
    {
        caller.StartCoroutine(DestroySequence(seconds));
    }


    private IEnumerator DestroySequence(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (mainBehaviour != null)
        {
            mainBehaviour.enabled = false;
        }
        if (secondaryBehaviour != null)
        {
            secondaryBehaviour.enabled = false;
        }
        //Have to move the spell somewhare so that the collider is away and ontriggerext works. Othervise, if deleted, the event doesn't happen.
        main.transform.position = new Vector3(0, -100, 0);
        yield return new WaitForSeconds(0.1f);
        UnityEngine.Object.Destroy(main);
    }

}

public abstract class Spell : MonoBehaviour
{

    public List<Spell> nextSpells;
    public List<(string Sign, string Hand)> letters;
    public int manaCost = 10;
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
                nextSpell.vessel = this.vessel;
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

        GameManager.Instance.UseMana(manaCost);
        return Cast(letter, handedness);
    }

    public abstract Spell Cast(string letter, string handedness);

    public void StopCast()
    {
        if (vessel != null)
        {
            vessel.DestroyAfter(this, 0);
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
        manaCost = 0;

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
            vessel.shape = Instantiate(Resources.Load<GameObject>("Effects/Fire"), vessel.main.transform.position, vessel.main.transform.rotation * Quaternion.Euler(180, 0, 0), vessel.main.transform);

            vessel.material = Resources.Load<Material>("Materials/Fire Material");
            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;

            vessel.AddBehavior<FollowTransform>(targetHand, Constants.handOffset);


            flames = vessel.shape.transform.Find("Flames").GetComponent<ParticleSystem>();
            secondaryFlames = vessel.shape.transform.Find("Flames Secondary").GetComponent<ParticleSystem>();
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
            Transform spellTF = vessel.main.transform;
            vessel.shape = Instantiate(Resources.Load<GameObject>("Effects/Disk"), spellTF.position, spellTF.rotation * Quaternion.Euler(90, 0, 0), spellTF);
            vessel.effect = Instantiate(Resources.Load<GameObject>("Effects/Light"), spellTF.position, spellTF.rotation, spellTF);
            vessel.effect.SetActive(false);

            vessel.material = Resources.Load<Material>("Materials/Light Glow");
            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;

            vessel.AddBehavior<FollowTransform>(targetHand, Constants.handOffset);

            vessel.AddSecondaryBehavior<FollowCamera>();

        }


        vessel.shape.GetComponent<UnityEngine.Light>().intensity = 1.0f + currentLetterIndex * 0.3f;
        vessel.shape.GetComponent<UnityEngine.Light>().range = 4.0f + currentLetterIndex * 0.25f;
        vessel.shape.transform.Find("Halo Large").GetComponent<UnityEngine.Light>().range = 0.2f + currentLetterIndex * 0.02f;

        vessel.effect.GetComponent<UnityEngine.Light>().intensity = 1.0f + currentLetterIndex * 0.2f;
        vessel.effect.GetComponent<UnityEngine.Light>().range = 3.0f + currentLetterIndex * 0.25f;
        vessel.effect.transform.Find("Halo Large").GetComponent<UnityEngine.Light>().range = 0.2f + currentLetterIndex * 0.02f;

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
            Destroy(vessel.shape.gameObject);
            if (vessel.effect != null)
            {
                vessel.effect.SetActive(true);
            }
            vessel.shape = Instantiate(Resources.Load<GameObject>("Effects/Ball"), vessel.main.transform.position, vessel.main.transform.rotation, vessel.main.transform);
            vessel.shape.GetComponent<MeshRenderer>().material = vessel.material;
            if (vessel.secondaryBehaviour == null)
            {
                vessel.AddSecondaryBehavior<FlyForward>();
            }
        }

        vessel.shape.transform.localScale *= (1 + currentLetterIndex * 0.3f);

        currentLetterIndex++;
        return this;
    }

    public override Spell ActivateSpell()
    {

        if (vessel.secondaryBehaviour != null)
        {
            vessel.secondaryBehaviour.enabled = true;
        }
        if (vessel.mainBehaviour != null)
        {
            vessel.mainBehaviour.enabled = false;
        }
        currentLetterIndex = 0;
        vessel.DestroyAfter(this, (currentLetterIndex + 3) * 20);
        //Destroy(vessel.main.gameObject, (currentLetterIndex + 1) * 20);
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
            Vector3 difference = Camera.main.transform.position - xrOrigin.position;

            xrOrigin.position = hitPoint;
            
            xrOrigin.transform.Find("Camera Offset").localPosition += new Vector3(difference.x, 0f, difference.z);
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

public class FollowCamera : VesselBehaviour
{
    private Transform target;
    public float smoothing = 0.1f;
    private Vector3 offset;

    void Start()
    {
        target = Camera.main.transform;
        offset = target.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        if (!target) return;
        transform.position = Vector3.Lerp(transform.position, target.TransformPoint(offset), smoothing);
        transform.rotation = target.rotation;
    }
}


public class FlyForward : VesselBehaviour
{
    public float speed = 3f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
