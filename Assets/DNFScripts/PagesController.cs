using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class PagesController : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    public Animator animator3;

    [SerializeField]
    [Tooltip("The event fired when the last Page is turned.")]
    UnityEvent<bool> m_LastPageTurned;

    [SerializeField]
    [Tooltip("The event fired when the first Page is turned.")]
    UnityEvent<bool> m_FirstPageTurned;

    private int _currentPage = 0;

    public int currentPage
    {
        get => _currentPage;
        set => _currentPage = value;
    }

    public void TriggerPage(string triggerName)
    {
        string lowerTrigger = triggerName.ToLower();
        
        if (lowerTrigger == "triggerleft")
        {
            TurnPageBackward();
        }
        else if (lowerTrigger == "triggerright")
        {
            TurnPageForward();
        }
        if(currentPage == 1)
        {
            //enable left coolider
            m_FirstPageTurned?.Invoke(true);
            m_LastPageTurned?.Invoke(true);
        }
        if (currentPage == 0)
        {
            //disable left coolider
            m_FirstPageTurned?.Invoke(false);
        }
        if (currentPage == 3)
        {
            m_LastPageTurned?.Invoke(false);
        }

    }

    private void TurnPageForward()
    {
        if (currentPage <= 2) 
        {
            GetAnimator(currentPage).SetTrigger("Turn Page");
            currentPage++;
        }
    }

    private void TurnPageBackward()
    {
        if (currentPage > 0)
        {
            GetAnimator(currentPage-1).SetTrigger("Turn Page");  
            currentPage--;
        }
    }

    private Animator GetAnimator(int index)
    {
        return index switch
        {
            0 => animator3,
            1 => animator2,
            2 => animator1,
            _ => null
        };
    }
}
