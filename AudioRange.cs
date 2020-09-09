using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioRange : MonoBehaviour
{

    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        distance -= audio.minDistance;
        distance = Mathf.Clamp(distance, 0f, audio.maxDistance);

        audio.volume = 1f - (distance / audio.maxDistance);

        
    }
}
