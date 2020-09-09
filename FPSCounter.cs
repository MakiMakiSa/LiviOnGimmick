using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    int frameCount;
    float prevTime;

    [SerializeField]
    Text text;


    void Start()
    {
        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update()
    {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            text.text =  (frameCount / time).ToString() + "fps";

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
