using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignSpriteController : MonoBehaviour
{
    public GameObject[] signSprites;


    public void ToggleSprite(char letter)
    {
        Log.L("Toggle: " + letter);
        int index = "abcdefghijklmnopqrstuvwxyz".IndexOf(char.ToLower(letter));
        if (index < 0) return;

        signSprites[index].SetActive(true);
    }

}
