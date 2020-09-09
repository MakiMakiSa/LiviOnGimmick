using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BurstAtoB : MonoBehaviour
{
    [SerializeField]
    GameObject A;
    [SerializeField]
    GameObject B;

    [SerializeField]
    VisualEffect ve;

    float detail = 1f;

    public float lerp;


    bool flag = false;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        Burst();
        ve.SendEvent("Burst");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            flag = !flag;
            timer += 3f;
        }

        lerp += flag ? Time.deltaTime : -Time.deltaTime;
        lerp = Mathf.Clamp(lerp, 0f, 1f);

        Burst();

    }

    void Burst()
    {
        MapManager.MeshToColorMap_Mix(A , B , out Texture2D posBuffA,  detail , lerp);
        ve.SetTexture("PositionTextureA", posBuffA);
        ve.SetInt("count", posBuffA.width * posBuffA.height);

        ve.SetFloat("AtoB", lerp);
    }
}
