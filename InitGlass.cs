using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[DefaultExecutionOrder(-10000)]
public class InitGlass : MonoBehaviour
{
    [SerializeField]
    Terrain terrain;
    [SerializeField]
    VisualEffect ve;

    [SerializeField]
    VisualEffect ve_GrassDust;

    [SerializeField]
    Material meshMaterial;


    Texture2D texture;

    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        if (!terrain)
        {
            enabled = false;
            return;
        }

        Created_2D();
    }


    void Created_2D()
    {
        ve.ResetOverride("PositionBuffer");
        ve.ResetOverride("GrassCount");


        int buffSize = terrain.terrainData.treeInstanceCount;

        float r = Mathf.Sqrt(buffSize);
        int w = (int)Mathf.Ceil(r);
        int h = w;

        Debug.Log("W" + w + "H" + h + "Buff" + buffSize);

        Color[] posBuffer = new Color[w * h];
        Vector3[] vertexs = new Vector3[buffSize];

        for (int i = 0; i < buffSize; i++)
        {
            TreeInstance ti = terrain.terrainData.GetTreeInstance(i);

            //var t = ti.GetType();
            //t.Name
            Vector3 pos = Vector3.Scale(ti.position, terrain.terrainData.size) + Terrain.activeTerrain.transform.position;

            posBuffer[i] = new Color(pos.x, pos.y-0.3f, pos.z);
            vertexs[i] = pos + (Vector3.up * 0.0001f);

        }

        texture = new Texture2D(w, h, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(posBuffer);
        texture.Apply();

        

        //地面メッシュ生成
        GrassDust gd = MeshCreate.Create(vertexs, false, null).AddComponent<GrassDust>();
        gd.effect = ve_GrassDust;
        gd.gameObject.AddComponent<Rigidbody>().isKinematic = true;
        gd.gameObject.layer = LayerMask.NameToLayer("FootEvent");
        gd.gameObject.tag = "Hide";
        //草エフェクト開始
        ve.SetTexture("PositionBuffer", texture);
        ve.SetFloat("GrassCount", posBuffer.Length * 2f);
        terrain.drawTreesAndFoliage = false;
        //        ve.Play();
        //        ve.gameObject.SetActive(true);


        ve.SendEvent("Emit");
        //ve.gameObject.SetActive(true);
        //ve.enabled = true;
//        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
