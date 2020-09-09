using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartFade : MonoBehaviour
{
    Image image;

    [SerializeField]
    float waitTime;
    [SerializeField]
    float fadeTime;

    float defaultFadeTime;


    Color aColor;
    Color bColor;
    void Start()
    {
        image = GetComponent<Image>();

        image.enabled = true;

        defaultFadeTime = fadeTime;

        aColor = image.color;
        bColor = image.color;
        bColor.a = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if(waitTime <= 0f)
        {
            fadeTime -= Time.deltaTime;

            float a = fadeTime / defaultFadeTime;
            image.color = Color.Lerp(bColor, aColor, Mathf.Clamp(a , 0f,1f));

            if(a < 0f)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
