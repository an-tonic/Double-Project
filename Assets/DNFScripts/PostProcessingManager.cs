using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;

    private float elapsedTime = 0f; 
    public float maxChangeTime = 20f; 
    public float maxSaturationValue = -100f; 
    public float maxContrastValue = -50f;

    void Awake()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out colorGrading);
        }
    }

    void OnEnable()
    {
        elapsedTime = 0f;
    }


    void Update()
    {

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / maxChangeTime;
        t = Mathf.Clamp01(t);
        colorGrading.saturation.value = Mathf.Lerp(0, maxSaturationValue, t);
        colorGrading.contrast.value = Mathf.Lerp(0, maxContrastValue, t);
    }


}
