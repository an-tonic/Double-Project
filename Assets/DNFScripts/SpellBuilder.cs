using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        Spell teleport = AddSpell<Teleportation>();
        Spell fire = AddSpell<Fire>();
        Spell light = AddSpell<Light>();
        Spell ball = AddSpell<Ball>();



        empty.nextSpells = new List<Spell> { teleport, fire, light };

        fire.nextSpells = new List<Spell> { ball };
        teleport.nextSpells = new List<Spell> { };
        light.nextSpells = new List<Spell> { ball };
        ball.nextSpells = new List<Spell> { };
        currentSpell = empty;


    }


    public void OnGestureRecognized(string gesture, string handedness)
    {
        gesture = gesture.ToLower();

        m_GesturePerformed?.Invoke(gesture, handedness);
        Log.L("Current spell: " + currentSpell);

        if (currentSpell == null)
        {
            currentSpell = empty;
        }
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

        if (GameManager.Instance.IsSignLearned(gesture[0]))
        {
            currentSpell = currentSpell.PrepareAndCast(gesture, handedness);
        }


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

    public static string CollectLetters(this List<(string Sign, string Hand)> letters) =>
        string.Concat(letters.ConvertAll(letter => letter.Sign.Substring(0, 1).ToUpper() + letter.Sign.Substring(1).ToLower()));

}

public abstract class Spell : MonoBehaviour
{

    public List<Spell> nextSpells;
    public List<(string Sign, string Hand)> letters = new List<(string Sign, string Hand)> { };
    public int manaCost = 10;
    public string activeHand;
    public string modifierName;

    protected GameObject refToSpell;

    protected int currentLetterIndex = 0;

    protected Transform targetRight;
    protected Transform targetLeft;
    protected Transform xrOrigin;

    public void Initialize(Transform targetR, Transform targetL, Transform targetXRorigin)
    {
        targetRight = targetR;
        targetLeft = targetL;
        xrOrigin = targetXRorigin;
    }

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

    public Spell PrepareAndCast(string letter, string handedness)
    {

        if (currentLetterIndex > 0 && !letters.IsCorrectLetter(currentLetterIndex, letter))
        {
            Spell nextSpell = FindNextSpell(letter, handedness);
            if (nextSpell && HasEnoughMana(nextSpell.manaCost))
            {
                this.StopCast();
                nextSpell.modifierName = this.letters.CollectLetters();

                currentLetterIndex = 0;
                return nextSpell.Cast(letter, handedness);
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

        if (!HasEnoughMana(this.manaCost))
        {
            Log.L("Not enough mana!");
            // Add some visual feedback maybe
            return this;
        }

        return Cast(letter, handedness);
    }

    protected bool HasEnoughMana(int manaValue)
    {
        if (manaValue == 0) return true;
        return GameManager.Instance.UseMana(manaValue);
    }

    abstract
    public Spell Cast(string letter, string handedness);

    virtual
    public void StopCast()
    {
        if(!refToSpell) return;
        refToSpell.GetComponent<SpellBehaviourBase>().StopCast();
        currentLetterIndex = 0;
    }

    virtual
    public Spell ActivateSpell()
    {
        if (!refToSpell) return this;
        refToSpell.GetComponent<SpellBehaviourBase>().ActivateSpell();
        currentLetterIndex = 0;
        return null;
    }
}


public class EmptySpell : Spell
{
    void Start()
    {
        manaCost = 0;
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
    public Spell ActivateSpell()
    {
        return this;
    }

    override
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            activeHand = handedness;
            refToSpell = Instantiate(Resources.Load<GameObject>("Effects/Fire"));

            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;
            refToSpell.GetComponent<FireBehaviour>().Initialize(targetHand);
          
        }
        refToSpell.GetComponent<FireBehaviour>().AdvanceSpell(currentLetterIndex);

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
    public Spell ActivateSpell()
    {
        return this;
    }

    override
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            activeHand = handedness;
            refToSpell = Instantiate(Resources.Load<GameObject>("Effects/Light"));

            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;
            refToSpell.GetComponent<LightBehaviour>().Initialize(targetHand);
        }

        refToSpell.GetComponent<LightBehaviour>().AdvanceSpell(currentLetterIndex);

        //vessel.effect.GetComponent<UnityEngine.Light>().intensity = 1.0f + currentLetterIndex * 0.2f;
        //vessel.effect.GetComponent<UnityEngine.Light>().range = 3.0f + currentLetterIndex * 0.25f;
        //vessel.effect.transform.Find("Halo Large").GetComponent<UnityEngine.Light>().range = 0.2f + currentLetterIndex * 0.02f;

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
            refToSpell = Instantiate(Resources.Load<GameObject>($"Effects/{modifierName}Ball"));

            Transform targetHand = handedness == "Right" ? targetRight : targetLeft;
            refToSpell.GetComponent<SpellBehaviourBase>()?.Initialize(targetHand);
        }

        refToSpell.GetComponent<SpellBehaviourBase>().AdvanceSpell(currentLetterIndex);

        currentLetterIndex++;
        return this;
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
    public Spell ActivateSpell()
    {
        return this;
    }


    override
    public Spell Cast(string letter, string handedness)
    {

        if (letters.IsFirst(letter))
        {
            activeHand = handedness;
            refToSpell = Instantiate(Resources.Load<GameObject>("Effects/TP_Line"));
            refToSpell.GetComponent<SpellBehaviourBase>()?.Initialize(targetLeft);
            
            currentLetterIndex++;
        }
        else if (letter == letters[1].Sign && CheckRaycast(out Vector3 hitPoint))
        {
            float distanceTeleported = Vector3.Distance(xrOrigin.position, hitPoint);
            GameManager.Instance.TravelDistance(distanceTeleported);

            xrOrigin.position = hitPoint;

            //Setting the xrOrigin and camera in the same spot relatively by changing offset (child of XRorigin, parent of camera)
            Vector3 difference = Camera.main.transform.position - xrOrigin.position;
            xrOrigin.transform.Find("Camera Offset").localPosition += new Vector3(difference.x, 0f, difference.z);
        }
        return this;
    }


    private bool CheckRaycast(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        Ray ray = new Ray(refToSpell.transform.position, refToSpell.transform.forward);
        LayerMask teleportLayer = 1 << LayerMask.NameToLayer("TeleportationLayer");
        if (Physics.Raycast(ray, out RaycastHit hitInfo, refToSpell.transform.localScale.z, teleportLayer) &&
            hitInfo.collider.CompareTag("TeleportSurface"))
        {
            hitPoint = hitInfo.point;
            return true;
        }
        return false;
    }

}
