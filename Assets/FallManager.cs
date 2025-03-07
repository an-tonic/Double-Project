using System.Collections;
using UnityEngine;

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
            Log.L("Yes, falling");
            StartCoroutine(RespawnSequence());
        }
    }

    IEnumerator RespawnSequence()
    {
        isFalling = true;
        Log.L("fading in");
        yield return fadeController.FadeTo(fadeStrength, fadeDuration);
        Log.L("moving");
        rb.velocity = Vector3.zero;
        xrOrigin.transform.position = startPosition.position;
        Log.L("fade out");
        yield return fadeController.FadeTo(0, fadeDuration);
        Log.L("not falling");
        isFalling = false;
    }

}
