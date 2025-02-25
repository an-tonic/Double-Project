using System.Collections;
using TMPro;
using UnityEngine;

public class HighlightLetter : MonoBehaviour
{
    public TextMeshPro textMesh;
    public Color32 highlightColor;
    public float transitionDuration = 1.0f;
    private Color32[] colors;
    private Color32[] originalColors;

    void Start()
    {
        if (textMesh == null) return;

        textMesh.ForceMeshUpdate();
        colors = textMesh.mesh.colors32; 
        originalColors = new Color32[colors.Length]; 
        colors.CopyTo(originalColors, 0);
    }

    public void glowLetter(int letterIndex)
    {
        if (textMesh == null) return;

        TMP_TextInfo textInfo = textMesh.textInfo;
        textMesh.ForceMeshUpdate();

        if (letterIndex < 0 || letterIndex >= textInfo.characterCount) return;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[letterIndex];

        originalColors = textMesh.mesh.colors32.Clone() as Color32[];

        StartCoroutine(ChangeLetterColor(letterIndex, transitionDuration, charInfo));
    }

    private IEnumerator ChangeLetterColor(int letterIndex, float duration, TMP_CharacterInfo charInfo)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float lerpFactor = elapsedTime / duration;

            for (int i = charInfo.vertexIndex; i < charInfo.vertexIndex + 4; i++)
            {
                colors[i] = Color32.Lerp(originalColors[i], highlightColor, lerpFactor);
            }

            textMesh.mesh.colors32 = colors;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void setText(string text)
    {
        if (text == null) return;
        if (textMesh == null) return;

        textMesh.text = text;
    }
}
