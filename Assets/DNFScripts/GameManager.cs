using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //private int playerHealth = 100; // Player's health
    private int playerMana = 100;     // Player's mana
    private int maxMana = 100;
    private int manaRestoreFactor = 10;

    [SerializeField]
    [Tooltip("The event fired when the mana is changed")]
    UnityEvent<float> m_ManaChanged;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(RegenerateMana());
    }

    public void UseMana(int amount)
    {
        playerMana = Mathf.Clamp(playerMana - amount, 0, maxMana);
        float manaRatio = (float)playerMana / maxMana;
        m_ManaChanged?.Invoke(manaRatio);
    }

    void Update()
    {


    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (playerMana < maxMana)
            {
                playerMana = Mathf.Clamp(playerMana + manaRestoreFactor, 0, maxMana);
                float manaRatio = (float)playerMana / maxMana;
                m_ManaChanged?.Invoke(manaRatio);
            }
            yield return new WaitForSeconds(1f);
        }
    }


}

