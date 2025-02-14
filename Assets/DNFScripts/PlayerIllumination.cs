

using UnityEngine;

public class PlayerIllumination : MonoBehaviour
{

    public GameObject postProcessingManager;
    public GameObject audioManager;


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("LightVolume"))
        {
            postProcessingManager.SetActive(false);
            audioManager.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("LightVolume"))
        {
            
            postProcessingManager.SetActive(true);
            audioManager.SetActive(true);
        }
    }
}
