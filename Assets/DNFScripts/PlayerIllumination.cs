using UnityEngine;


public class PlayerIllumination : MonoBehaviour
{
    public static bool IsIlluminated = false; // Static variable, globally accessible


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("LightVolume"))
        {

            IsIlluminated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LightVolume"))
        {
            IsIlluminated = false;
        }
    }
}

