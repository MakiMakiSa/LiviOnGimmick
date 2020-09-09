using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SunLightManager : MonoBehaviour
{
    [SerializeField]
    Volume volume;
    [SerializeField]
    Light sunLight;

    HDRISky sky;

    float defaultDesiredLuxValue;
    float defaultSunValue;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out HDRISky s);
        if(s)
        {
            sky = s;
            defaultDesiredLuxValue = sky.desiredLuxValue.value;
        }

        defaultSunValue = sunLight.intensity;

    }

    // Update is called once per frame
    void Update()
    {
        if (sky)
        {
            float d = (Mathf.Clamp(Camera.main.transform.position.y, 10f, 20f)-10f) / 10f;

            sky.desiredLuxValue.value = Mathf.Lerp(0f, defaultDesiredLuxValue , d);
            sunLight.intensity = Mathf.Lerp(0f, defaultSunValue, d);
        }
    }
}
