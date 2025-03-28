using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GenericTagTrigger : MonoBehaviour
{
    public string targetTag = "PlayerHand";


    [SerializeField]
    [Tooltip("Event is fired immediatly when this trigger is activated.")]
    public UnityEvent onTriggerEntered;

    [Range(0f, 10000f)]
    public int WaitForMilliseconds = 0;

    [SerializeField]
    [Tooltip("Event is fired when this trigger is activated after delay.")]
    public UnityEvent onTriggerEnteredDelayed;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(targetTag))
        {
            if (WaitForMilliseconds > 0)
                StartCoroutine(DelayedTrigger());
            
            onTriggerEntered?.Invoke();
        }
    }

    private IEnumerator DelayedTrigger()
    {
        yield return new WaitForSeconds(WaitForMilliseconds / 1000f);
        onTriggerEnteredDelayed?.Invoke();
    }

}
