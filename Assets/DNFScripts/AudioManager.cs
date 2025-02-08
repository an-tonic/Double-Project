using System.Collections.Generic;

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<GameObject> targetAudioObjects; // Public list of GameObjects to manage
    public float normalVolume = 1f; // Volume for normal state
    public float contusionVolume = 0.5f; // Volume for contusion state

    // Low-pass filter parameters
    public float normalPitch = 1f; // Pitch for normal state
    public float contusionPitch = 0.7f; // Pitch for contusion state
    public float normalLowPassResonance = 1f; // Q factor for normal state
    public float contusionLowPassResonance = 2f; // Q factor for contusion state

    private bool isIlluminated; // Current state
    public float transitionDuration = 1f; // Duration for transition
    private float elapsedTime; // Timer to track transition
    private Dictionary<GameObject, AudioSource> audioSources = new Dictionary<GameObject, AudioSource>(); // Cache audio sources

    private void Awake()
    {
        // Cache AudioSources for all target objects
        foreach (var obj in targetAudioObjects)
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSources[obj] = audioSource; // Store the audio source in the dictionary
            }
            else
            {
                Debug.LogWarning($"No AudioSource found on {obj.name}");
            }
        }
    }

    private void Update()
    {
        // Update audio settings if the state has changed
        if (elapsedTime > 0)
        {
            // Determine transition progress
            float t = elapsedTime / transitionDuration;

            // Gradually adjust audio properties
            foreach (var kvp in audioSources)
            {
                UpdateAudioProperties(kvp.Key, kvp.Value, t);
            }

            // If the transition is complete, reset elapsedTime
            if (elapsedTime <= 0)
            {
                elapsedTime = 0; // Stop the transition
            }
        }
    }

    // Method to update audio for the target GameObjects based on isIlluminated state
    public void UpdateAudio(bool isIlluminated)
    {
        if (this.isIlluminated != isIlluminated) // Only update if state changes
        {
            this.isIlluminated = isIlluminated;
            elapsedTime = transitionDuration; // Reset the timer for transition
        }
        else
        {
            // If the state remains the same, continue updating elapsed time for smooth transition
            elapsedTime = Mathf.Max(0, elapsedTime - (Time.deltaTime * (isIlluminated ? 1 : -1) * (transitionDuration / 2)));
        }
    }

    private void UpdateAudioProperties(GameObject obj, AudioSource audioSource, float t)
    {
        // Check if the GameObject has a LowPassFilter; if not, add one
        AudioLowPassFilter lowPassFilter = obj.GetComponent<AudioLowPassFilter>();
        if (lowPassFilter == null)
        {
            lowPassFilter = obj.AddComponent<AudioLowPassFilter>();
        }

        // Interpolate audio properties based on current state
        audioSource.volume = Mathf.Lerp(isIlluminated ? contusionVolume : normalVolume,
                                        isIlluminated ? normalVolume : contusionVolume,
                                        1 - t);

        audioSource.pitch = Mathf.Lerp(isIlluminated ? contusionPitch : normalPitch,
                                        isIlluminated ? normalPitch : contusionPitch,
                                        1 - t);

        lowPassFilter.lowpassResonanceQ = Mathf.Lerp(isIlluminated ? contusionLowPassResonance : normalLowPassResonance,
                                                       isIlluminated ? normalLowPassResonance : contusionLowPassResonance,
                                                       1 - t);
    }
}
