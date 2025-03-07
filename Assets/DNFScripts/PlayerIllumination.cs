using UnityEngine;

public class PlayerIllumination : MonoBehaviour
{
    
    public GameObject audioManager;
    public FadeController fadeController;
    public float fadeDuration = 7.0f;
    public float fadeStrength = 0.8f;

    private void OnTriggerEnter(Collider other)
    {
        Log.L("1");
        if (other.CompareTag("LightVolume"))
        {
        Log.L("2");

            audioManager.SetActive(false);
            //fadeController.FadeTo(0, fadeDuration); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Log.L("3");

        if (other.CompareTag("LightVolume"))
        {
            Log.L("4");

            audioManager.SetActive(true);
            //fadeController.FadeTo(fadeStrength, fadeDuration);
        }
    }
}
