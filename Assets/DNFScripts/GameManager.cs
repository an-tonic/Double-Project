using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    //private int playerHealth = 100; // Player's health
    //private int playerMana = 100;     // Player's mana
    //private float nextHealthDecreaseTime = 0f;

    public AudioManager audioManager;
    //public int healthDecreaseRate = 1;
    //public float healthDecreaseInterval = 1f;
    


    private void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this; // Set this instance as the singleton
            DontDestroyOnLoad(gameObject); // Keep this GameObject across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }



    void Update()
    {

        DecreaseHealth();
        
        UpdateAudio();
        
    }

    private void UpdateAudio()
    {

        if (audioManager != null)
        {
            //audioManager.UpdateAudio(playerIllumination.IsIlluminated);

        }
    }

    private void DecreaseHealth()
    {
        //if (!playerIllumination.IsIlluminated && Time.time >= nextHealthDecreaseTime)
        //{
        //    playerHealth -= healthDecreaseRate;
        //    nextHealthDecreaseTime = Time.time + healthDecreaseInterval; 
        //}

    }

}

