using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;

public class HighlightLetter : MonoBehaviour
{
    public TextMeshPro textMesh;
    public Color32 highlightColor;
    public float transitionDuration = 1.0f;
    public float transitionStep = 0.01f;

    public bool rememberColour = true;

    private Color32[] colors;
    private Color32[] originalColors;

    void Start()
    {
        if (textMesh == null) return;

        textMesh.ForceMeshUpdate();
        colors = textMesh.mesh.colors32;
        originalColors = textMesh.mesh.colors32;
    }

    public void GlowLetter(string letter, string handedness)
    {
        string currentText = textMesh.text.ToLower();     
        GlowLetterAtIndex(currentText.IndexOf(letter.ToLower()));
    }

    public void GlowLetterAtIndex(int letterIndex)
    {
        if (letterIndex < 0 || letterIndex >= textMesh.text.Length || !gameObject.activeInHierarchy) return;

        textMesh.ForceMeshUpdate();

        // 4 is number of vertices per letter in text
        int numOfSpacesBeforeLetter = textMesh.text.Substring(0, letterIndex).Count(Char.IsWhiteSpace);
        int vertexIndex = (letterIndex - numOfSpacesBeforeLetter) * 4;

        if (!rememberColour)
        {
            //Reset colors
            colors = originalColors.Clone() as Color32[];
        }
        StopAllCoroutines();
        
        StartCoroutine(ChangeLetterColor(vertexIndex));
        
    }

    private IEnumerator ChangeLetterColor(int index)
    {
        for (float elapsed = 0f; elapsed < transitionDuration; elapsed += transitionStep)
        {
            float t = elapsed / transitionDuration;

            for (int i = index; i < index + 4; i++)
            {
                colors[i] = Color32.Lerp(textMesh.mesh.colors32[i], highlightColor, t);
            }
            textMesh.mesh.colors32 = colors;
            yield return null;
        }
    }

    public void SetText(string text)
    {
        
        textMesh.text = text;
        textMesh.ForceMeshUpdate();

        colors = textMesh.mesh.colors32;
        originalColors = textMesh.mesh.colors32;
    }
}
