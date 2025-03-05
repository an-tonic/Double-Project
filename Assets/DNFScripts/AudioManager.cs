using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    private float elapsedTime = 0f;
    public float maxChangeTime = 20f;
    public float maxVolumeValue = 0f; 
    public float minVolumeValue = -80f; 
    public float maxPitchValue = 1f; 
    public float minPitchValue = 0.7f; 
    public float normalLowPassResonance = 1f;
    public float contusionLowPassResonance = 2f;

    void OnEnable()
    {
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / maxChangeTime;
        t = Mathf.Clamp01(t);

        float volume = Mathf.Lerp(maxVolumeValue, minVolumeValue, t);
        float pitch = Mathf.Lerp(maxPitchValue, minPitchValue, t);
        float resonance = Mathf.Lerp(normalLowPassResonance, contusionLowPassResonance, t); ;

        audioMixer.SetFloat("VolumeShift", volume);
        audioMixer.SetFloat("PitchShift", pitch);
        audioMixer.SetFloat("ResonanceShift", resonance);
    }

    void OnDisable()
    {
        audioMixer.SetFloat("VolumeShift", maxVolumeValue);
        audioMixer.SetFloat("PitchShift", maxPitchValue);
        audioMixer.SetFloat("ResonanceShift", normalLowPassResonance);
    }

}
