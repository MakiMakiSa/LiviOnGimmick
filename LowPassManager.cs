using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioReverbZone))]
public class LowPassManager : MonoBehaviour
{
    AudioReverbZone ARZ;

    // Start is called before the first frame update
    void Start()
    {
        ARZ =  GetComponent<AudioReverbZone>();
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
