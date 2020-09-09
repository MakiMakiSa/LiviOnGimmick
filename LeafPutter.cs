using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LeafPutter : MonoBehaviour
{
    [SerializeField]
    VisualEffect LeafVFX;
    // Start is called before the first frame update
    void Start()
    {
        if (!LeafVFX) enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        LeafVFX.SetVector3("PlayerPosition", LeafVFX.transform.InverseTransformPoint(transform.position));
    }
}
