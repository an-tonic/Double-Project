using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;

    private float elapsedTime = 0f; // Track time in dark state
    public float maxChangeTime = 20f; // Time to reach max saturation change
    public float maxSaturationValue = -100f; // Minimum saturation value for effect
    public float maxContrastValue = -50f;

    private void Awake()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out colorGrading);
        }
    }

    public void UpdateSaturation(bool isIlluminated)
    {
        // Update the time based on whether the scene is illuminated or not
        if (isIlluminated)
        {
            elapsedTime = Mathf.Max(0, elapsedTime - Time.deltaTime * 10); // Decrement time in dark
        }
        else
        {
            elapsedTime += Time.deltaTime; // Increment time in dark
        }

        // Calculate the interpolation factor (0 to 1)
        float t = elapsedTime / maxChangeTime;
        t = Mathf.Clamp01(t); // Clamp to ensure it's between 0 and 1

        // Calculate saturation and contrast changes
        float saturationChange = Mathf.Lerp(0, maxSaturationValue, t);
        float contrastChange = Mathf.Lerp(0, maxContrastValue, t);

        // Apply the calculated saturation and contrast values
        SetSaturationAndContrast(saturationChange, contrastChange);
    }

    public void SetSaturationAndContrast(float saturationValue, float contrastValue)
    {
        if (colorGrading != null)
        {
            colorGrading.saturation.value = saturationValue;
            colorGrading.contrast.value = contrastValue;
        }
    }
}
