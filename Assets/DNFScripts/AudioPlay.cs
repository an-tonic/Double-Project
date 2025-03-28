using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public AudioSource targetAudioSource;

    public void PlaySound()
    {
        if (targetAudioSource != null)
        {
            targetAudioSource.Play();
        }
    }
}
