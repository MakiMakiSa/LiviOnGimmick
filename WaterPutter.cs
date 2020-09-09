using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPutter : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Material waterMaterial;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waterMaterial.SetVector("UserPosition", transform.position);
        
    }
}
