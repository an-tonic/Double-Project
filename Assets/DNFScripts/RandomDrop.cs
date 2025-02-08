using UnityEngine;

public class WaterDropSound : MonoBehaviour
{
    public AudioSource waterDropAudio;
    public float minDelay = 2f;
    public float maxDelay = 10f;
    
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    void Start()
    {
        StartCoroutine(PlayWaterDropSound());
    }

    System.Collections.IEnumerator PlayWaterDropSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            waterDropAudio.pitch = Random.Range(minPitch, maxPitch);
            waterDropAudio.Play();
        }
    }
}
