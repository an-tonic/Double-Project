using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugMenu : MonoBehaviour
{

    public TextMeshPro textMesh;
    int fps = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
   
        fps = (int)(1f / Time.unscaledDeltaTime);
        textMesh.text = fps.ToString();
    }
}
