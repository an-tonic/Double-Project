using UnityEngine;

public class PlankTrigger : MonoBehaviour
{
    public Animator plankAnimator;

    private void OnTriggerEnter(Collider other)
    {
        
        plankAnimator.SetTrigger("PlankFall");
        Destroy(gameObject);
        
    }
}
