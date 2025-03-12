using UnityEngine;

public class RingController : MonoBehaviour
{

    private Material ringMaterial;
    private Color fullValueColor;

    public Color emptyColor = Color.black;

    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            ringMaterial = renderer.material;
            fullValueColor = ringMaterial.GetColor("_EmissionColor");
        }
    }

    public void UpdateColorRing(float valueRatio)
    {

        Color newColor = Color.Lerp(emptyColor, fullValueColor, valueRatio);
        ringMaterial.SetColor("_EmissionColor", newColor * valueRatio);
    }
}
