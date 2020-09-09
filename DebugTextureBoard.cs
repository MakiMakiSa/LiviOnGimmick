using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100000)]
public class DebugTextureBoard : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetTexture("MainTexture", Bullet_VFX.Texture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
