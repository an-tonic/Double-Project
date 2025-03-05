using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class PostProcessingManager : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;

    public float maxChangeTime = 20f;

    void Awake()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out colorGrading);
        }
    }

    public IEnumerator AdjustPostProcessing(float targetSaturation, float targetContrast)
    {
        float elapsedTime = 0f;
        float startSaturation = colorGrading.saturation.value;
        float startContrast = colorGrading.contrast.value;

        while (elapsedTime < maxChangeTime)
        {
            float t = elapsedTime / maxChangeTime;
            colorGrading.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, t);
            colorGrading.contrast.value = Mathf.Lerp(startContrast, targetContrast, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        colorGrading.saturation.value = targetSaturation;
        colorGrading.contrast.value = targetContrast;
    }
}
