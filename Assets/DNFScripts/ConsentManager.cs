using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ConsentScreen : MonoBehaviour
{
    public GameObject[] infoTexts;
    public GameObject[] objectsToDisable;
    public Text buttonText;

    private int currentTextIndex = 0;


    public void OnProceedPressed()
    {
        if(currentTextIndex == infoTexts.Length-1)
        {
            buttonText.text = "Enter the game";
        }
        if (currentTextIndex < infoTexts.Length - 1)
        {
            infoTexts[currentTextIndex].gameObject.SetActive(false);
            currentTextIndex++;
            infoTexts[currentTextIndex].gameObject.SetActive(true);
        }
        else
        {
            foreach(GameObject obj in objectsToDisable){
                obj.SetActive(false);
            }
            SceneManager.LoadSceneAsync("IntroScene");
        }
    }

    public void OnBackPressed()
    {
        if (currentTextIndex > 0)
        {
            infoTexts[currentTextIndex].gameObject.SetActive(false);
            currentTextIndex--;
            infoTexts[currentTextIndex].gameObject.SetActive(true);
        }
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
