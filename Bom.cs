using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bom : MonoBehaviour
{
    VisualEffect ve;

    [SerializeField]
    Material material;

    float timer = 0f;
    float waiter = 0f;
    float alpha;
    // Start is called before the first frame update
    void Start()
    {
        ve = GetComponent<VisualEffect>();

//        material = GetComponent<Renderer>().material;
    }

    private void OnEnable()
    {
        if(ve)ve.Play();
        waiter = 1f;
        transform.localScale = Vector3.zero;

        alpha = 1f;
        SetAlpha(alpha);
    }

    void SetAlpha(float a)
    {
        material.SetFloat("Alpha", a);
    }
    void SetTime(float t)
    {
        material.SetFloat("Time", t);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime*alpha;
        SetTime(timer);

        waiter -= Time.deltaTime*3f;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 5f, Time.deltaTime * 30f);

        SetAlpha(alpha);

        if (waiter <= 0f)
        {
            alpha -= Time.deltaTime*0.5f;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            if(alpha <= 0f)
            {
                if (ve.aliveParticleCount <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }

    }
}
