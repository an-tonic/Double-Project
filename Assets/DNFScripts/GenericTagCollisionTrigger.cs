using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GenericTagCollisionTrigger : MonoBehaviour
{
    public string targetTag = "DamagingObject";


    [SerializeField]
    [Tooltip("Event is fired immediatly when this trigger is activated.")]
    public UnityEvent onCollisionEntered;

    [Range(0f, 10000f)]
    public int WaitForMilliseconds = 0;

    [SerializeField]
    [Tooltip("Event is fired when this trigger is activated after delay.")]
    public UnityEvent onCollisionEnteredDelayed;

    private void OnCollisionEnter(Collision collision)
    {
 
        if (collision.gameObject.CompareTag(targetTag))
        {
  
            if (WaitForMilliseconds > 0)
                StartCoroutine(DelayedTrigger());

            onCollisionEntered?.Invoke();
        }
    }

    private IEnumerator DelayedTrigger()
    {
        yield return new WaitForSeconds(WaitForMilliseconds / 1000f);
        onCollisionEnteredDelayed?.Invoke();
    }

}
