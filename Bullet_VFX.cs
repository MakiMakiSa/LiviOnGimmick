using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bullet_VFX : MonoBehaviour
{

    static int idCounter;
    int id;


    //弾丸たちの座標バッファ、使用されているバッファのwには１が、そうでなければ０が入る（RGB = XYZ)
    static Color[] buff = new Color[65536/8];
    static Texture2D texture;

    static public Texture2D Texture => texture;

    static public VisualEffect ve;


    static public bool InitFlag = false;
    static public void Init()
    {
        texture = MapManager.CreateMap(buff , out Color[] buf);

        buff = buf;
    }

    static public void UpdateMap()
    {
        texture.SetPixels(buff);
        texture.Apply();

        Debug.Log("Tex" + ve.aliveParticleCount);
        ve.SetTexture("PositionMap", texture);
    }

    // Start is called before the first frame update
    void Start()
    {
        id = idCounter;
        idCounter++;
        idCounter %= buff.Length;

        UpdateBuff(true);
        ve.SetInt("initialID", id);
        ve.SendEvent("Emit");


    }

    // Update is called once per frame
    void Update()
    {
        UpdateBuff(true);
    }

    private void OnDestroy()
    {
        UpdateBuff(false);
    }

    void UpdateBuff(bool flag)
    {
        buff[id] = new Color(transform.position.x, transform.position.y, transform.position.z, flag ? 1f:0f);
    }
}
