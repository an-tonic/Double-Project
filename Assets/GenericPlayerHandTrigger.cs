using UnityEngine;
using UnityEngine.Events;

public class GenericPlayerHandTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Event fired when this trigger is activated.")]
    private UnityEvent onTriggerEntered;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("PlayerHand"))
        {
            onTriggerEntered?.Invoke();
        }
    }
}
