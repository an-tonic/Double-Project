using UnityEngine;

public class PlayerIllumination : MonoBehaviour
{

    public GameObject audioManager;
    public FadeController fadeController;
    public float fadeDuration = 7.0f;
    public float fadeStrength = 0.8f;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("LightVolume"))
        {


            audioManager.SetActive(false);
            //fadeController.FadeTo(0, fadeDuration); 
        }
    }

    private void OnTriggerExit(Collider other)
    {


        if (other.CompareTag("LightVolume"))
        {

            audioManager.SetActive(true);
            //fadeController.FadeTo(fadeStrength, fadeDuration);
        }
    }
}
