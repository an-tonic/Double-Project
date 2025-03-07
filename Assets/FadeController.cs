using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Renderer fadePlane;

    private Material fadeMaterial;

    void Start()
    {
        fadeMaterial = fadePlane.material;
    }

    public Coroutine FadeTo(float targetAlpha, float fadeDuration)
    {
        StopAllCoroutines();
        return StartCoroutine(FadeProcess(targetAlpha, fadeDuration));
    }

    private IEnumerator FadeProcess(float targetAlpha, float fadeDuration)
    {
        Log.L("starting fade process");
        float startAlpha = fadeMaterial.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            Color color = fadeMaterial.color;
            color.a = alpha;
            fadeMaterial.color = color;
            yield return null;
        }
    }
}
