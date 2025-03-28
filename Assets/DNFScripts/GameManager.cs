using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //private List<char> knownSigns = new List<char>();
    public List<char> knownSigns = new List<char> {'f', 'd'};
    private int playerHealth = 100; 
    private int playerMana = 100;   
    private int maxMana = 100;
    private int maxHealth = 100;
    private int manaRestoreFactor = 5;


    private int spellsCast = 0;
    private float distanceTraveled = 0;
    private int enemiesKilled = 0;
    private int manaUsed = 0;
    private int score = 0;

    [SerializeField]
    [Tooltip("The event fired when the mana is changed")]
    UnityEvent<float> m_ManaChanged;

    [SerializeField]
    [Tooltip("The event fired when the health is changed")]
    UnityEvent<float> m_HealthChanged;


    [SerializeField]
    [Tooltip("The event fired when a new sign is learnt")]
    UnityEvent<char> m_NewSignLearnt;


    [SerializeField]
    [Tooltip("The event fired when user stats are updated")]
    UnityEvent<string> m_StatsUpdated;

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
        UpdateStats();
        StartCoroutine(RegenerateMana());
    }

    public void KillEnemy()
    {
        enemiesKilled++;
        UpdateStats();
    }

    public bool UseMana(int amount)
    {
        if (amount > playerMana) return false;

        playerMana = Mathf.Clamp(playerMana - amount, 0, maxMana);
        float manaRatio = (float)playerMana / maxMana;
        m_ManaChanged?.Invoke(manaRatio);
        
        manaUsed += amount;
        spellsCast++;
        UpdateStats();
        return true;
    }

    public void TakeDamage(int damage)
    {

        playerHealth -= damage;
        float healthRatio = (float)playerHealth / maxHealth;
        Log.L(playerHealth);
        m_HealthChanged?.Invoke(healthRatio);

    }

    public void AddNewSign(char sign)
    {
        char newSign = char.ToLower(sign);
        Log.L("invokink: " + newSign);

        if (!knownSigns.Contains(newSign) && "abcdefghijklmnopqrstuvwxyz".IndexOf(newSign) >= 0)
        {
            Log.L("invokink: " + newSign);
            knownSigns.Add(newSign);
            m_NewSignLearnt?.Invoke(newSign);
            UpdateStats();
        }
    }


    public void TravelDistance(float distance)
    {
        if (distance < 0) return;
        distanceTraveled += distance;
    }

    public bool IsSignLearned(char sign)
    {
        return knownSigns.Contains(char.ToLower(sign));
    }

    private void UpdateStats()
    {
        string stats = $"{spellsCast}\n" +
                       $"{distanceTraveled:F1}m\n" +
                       $"{enemiesKilled}\n" +
                       $"{knownSigns.Count}\n" +
                       $"{manaUsed}\n" +
                       $"{score}";
        m_StatsUpdated?.Invoke(stats);
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

