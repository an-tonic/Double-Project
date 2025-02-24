using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ConsentScreen : MonoBehaviour
{
    public GameObject[] infoTexts;
    private int currentTextIndex = 0;

    public void OnProceedPressed()
    {
        if (currentTextIndex < infoTexts.Length - 1)
        {
            infoTexts[currentTextIndex].gameObject.SetActive(false);
            currentTextIndex++;
            infoTexts[currentTextIndex].gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
