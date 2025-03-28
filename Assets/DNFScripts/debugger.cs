using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class debugger : MonoBehaviour
{

    public SpellBuilder spellBuilder;
    public string inputText;
    public bool isRight;
    public void OnButtonPress()
    {
        if (spellBuilder != null)
        {
            
            spellBuilder.OnGestureRecognized(inputText, isRight ? "Right" : "Left");

    
        }
        else
        {
            Debug.LogWarning("SpellBuilder is not assigned.");
        }
    }

}
