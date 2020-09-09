using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakSystem : MonoBehaviour
{

    [SerializeField]
    static float resolutionScale = 64;


    Texture2D texture_A_XY = null;
    Texture2D texture_B_YZ = null;
    Texture2D texture_C_XZ = null;


    Vector3 offset;

    [SerializeField]
    Texture2D defaultStamp;

    Renderer thisRenderer;

    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<Renderer>();
        Material material = thisRenderer.material;

        offset = thisRenderer.bounds.min - thisRenderer.transform.position;

        Vector3 s =   thisRenderer.bounds.size;
        float maxS = Mathf.Max(s.x, s.y,s.z);



        int reso = (int)(resolutionScale * maxS);


        //テクスチャのコピーを作成（みんなで一つのテクスチャを共有するとだめ）
        texture_A_XY = Mathaf.textureCreateInit(material, "BreakMask_XY", reso, reso , TextureWrapMode.Clamp);
        texture_B_YZ = Mathaf.textureCreateInit(material, "BreakMask_YZ", reso, reso, TextureWrapMode.Clamp);
        texture_C_XZ = Mathaf.textureCreateInit(material, "BreakMask_XZ", reso, reso, TextureWrapMode.Clamp);


        //        wSize = thisRenderer.material.GetVector("Scale");




        material.SetTexture("BreakMask_XY", texture_A_XY);
        material.SetTexture("BreakMask_YZ", texture_B_YZ);
        material.SetTexture("BreakMask_XZ", texture_C_XZ);

        material.SetFloat("ResolutionScale", resolutionScale);





    }

    // Update is called once per frame
    void Update()
    {
        float l = thisRenderer.bounds.center.x - thisRenderer.bounds.size.x;
        float r = thisRenderer.bounds.center.x + thisRenderer.bounds.size.x;
        float u = thisRenderer.bounds.center.y - thisRenderer.bounds.size.y;
        float d = thisRenderer.bounds.center.y + thisRenderer.bounds.size.y;
        float f = thisRenderer.bounds.center.z - thisRenderer.bounds.size.z;
        float b = thisRenderer.bounds.center.z + thisRenderer.bounds.size.z;

        Vector3[] vs = {
            new Vector3(l, u, f),
            new Vector3(l, u,b),
            new Vector3(r, u,f),
            new Vector3(r, u,b),
            new Vector3(l, d, f),
            new Vector3(l, d,b),
            new Vector3(r, d,f),
            new Vector3(r, d,b)
        };


        Debug.DrawRay(transform.position, Vector3.up, Color.blue);
        foreach(Vector3 vA in vs)
        {
            foreach (Vector3 vB in vs)
            {
                Debug.DrawLine(vA,vB, Color.red);
            }
        }

    }

    public void BreakAtPoint(Vector3 pos, Texture2D stampTexture)
    {
//        pos -= offset;
        if (!stampTexture)
        {
            stampTexture = defaultStamp;
        }
        pos = transform.InverseTransformPoint(pos);

        float ws = 1f / (texture_A_XY.width/resolutionScale);
        
//        pos -= transform.position;

        

        pos *= ws;


        pos.x %= 1f;
        pos.y %= 1f;
        pos.z %= 1f;


        Vector2 xy = new Vector2(pos.x, pos.y);
        Vector2 yz = new Vector2(pos.y, pos.z);
        Vector2 xz = new Vector2(pos.x, pos.z);



        Mathaf.DrawTexture(xy, texture_A_XY, stampTexture);
        Mathaf.DrawTexture(yz, texture_B_YZ, stampTexture);
        Mathaf.DrawTexture(xz, texture_C_XZ, stampTexture);

    }


    public void BreakAtRay(RaycastHit hit)
    {
        Mathaf.DrawTexture_Ray(texture_A_XY, hit, defaultStamp);
    }
}
