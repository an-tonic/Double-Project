using UnityEngine;
using System.Collections;

public class FallManager : MonoBehaviour
{
    public Transform startPosition;
    public GameObject xrOrigin;
    public FadeController fadeController;

    public float fallThreshold = -5f;
    public float fadeDuration = 7.0f;
    public float fadeStrength = 0.8f;
    

    private Rigidbody rb;
    private bool isFalling = false;

    void Start()
    {
        rb = xrOrigin.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (xrOrigin.transform.position.y < fallThreshold && !isFalling)
        {
            StartCoroutine(FallSequence());
        }
    }

    IEnumerator FallSequence()
    {
        isFalling = true;

        yield return fadeController.FadeTo(fadeStrength, fadeDuration);

        rb.velocity = Vector3.zero;
        xrOrigin.transform.position = startPosition.position;

        yield return fadeController.FadeTo(0, fadeDuration);

        isFalling = false;
    }

}
