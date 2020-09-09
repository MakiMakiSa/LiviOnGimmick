using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapManager
{
    static float a_b;
    static float ab_c;

    static public void MeshToColorMap(GameObject obj , out Texture2D posBuff ,/* out Texture2D normalBuff ,out Texture2D uvBuff,*/ bool lightFlag , float density)
    {
        

        var smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();


        List<Color> buff_pos = new List<Color>();
        //List<Color> buff_nor = new List<Color>();
        //List<Color> buff_uv = new List<Color>();


        //可変メッシュ
        foreach (var smr in smrs)
        {
            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);

            if(lightFlag)
            {
                //座標
                buff_pos.AddRange(mesh.vertices.Select(b => new Color(b.x, b.y, b.z)).ToList());
                ////ノーマル
                //buff_nor.AddRange(mesh.normals.Select(b => new Color(b.x, b.y, b.z)).ToList());
                ////UV
                //buff_nor.AddRange(mesh.uv.Select(b => new Color(b.x, b.y, 0f)).ToList());

            }
            else
            {
                MeshBuffToSurfacePoints(mesh, out Vector3[] poss);

                //座標
                buff_pos.AddRange(poss.Select(b => new Color(b.x, b.y, b.z)).ToList());
                ////ノーマル
                //buff_nor.AddRange(nors.Select(b => new Color(b.x, b.y, b.z)).ToList());
                ////UV
                //buff_uv.AddRange(uvs.Select(b => new Color(b.x, b.y, b.z)).ToList());
            }
        }

        Debug.Log("MeshCount:"+smrs.Count);
        ////不変メッシュ
        //foreach (var mf in mfs)
        //{
        //    Mesh mesh = mf.sharedMesh;
        //    //座標
        //    buff_pos.AddRange(mesh.vertices.Select(b => new Color(b.x, b.y, b.z)).ToList());
        //    //ノーマル
        //    buff_nor.AddRange(mesh.normals.Select(b => new Color(b.x, b.y, b.z)).ToList());
        //}



        //テクスチャ作成
        posBuff = CreateMap(buff_pos , out Color[] buf);
        ////テクスチャ作成
        //normalBuff = CreateMap(buff_nor);
        ////テクスチャ作成
        //uvBuff = CreateMap(buff_uv);



    }



    static public void MeshToColorMap_Mix(GameObject objA, GameObject objB, out Texture2D posBuff, float density , float lerp)
    {


        var smrsA = objA.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        var smrsB = objB.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();


        List<Color> buff_posA = new List<Color>();
        List<Color> buff_posB = new List<Color>();
        //List<Color> buff_nor = new List<Color>();
        //List<Color> buff_uv = new List<Color>();


        //可変メッシュ
        foreach (var smr in smrsA)
        {
            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);

            MeshBuffToSurfacePoints(mesh, out Vector3[] poss);
            //座標
            buff_posA.AddRange(poss.Select(b => new Color(b.x, b.y, b.z)).ToList());
        }
        foreach (var smr in smrsB)
        {
            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);


            MeshBuffToSurfacePoints(mesh, out Vector3[] poss);
            //座標
            buff_posB.AddRange(poss.Select(b => new Color(b.x, b.y, b.z)).ToList());
        }

        int m = Mathf.Max(buff_posA.Count, buff_posB.Count);

        List<Color> buff_pos = new List<Color>(m);

        for (int i = 0; i < m; i++)
        {
            int a = i % buff_posA.Count;
            int b = i % buff_posB.Count;

            buff_pos.Add(Color.Lerp(buff_posA[a], buff_posB[b], lerp));
        }


        //テクスチャ作成
        posBuff = CreateMap(buff_pos, out Color[] buf);



    }



    /// <summary>
    /// 軽量版
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="_poss"></param>
    static public void MeshBuffToSurfacePoints(Mesh mesh, out Vector3[] _poss)
    {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        var buffSize = (triangles.Length / 3);

        _poss = new Vector3[(int)buffSize];

        var buffCount = 0;



        Debug.Log("Tri" + mesh.triangles.Length);

        for (var i = 0; i < triangles.Length; i += 3)
        {
            var v0 = triangles[i];
            var v1 = triangles[i + 1];
            var v2 = triangles[i + 2];

            var pos_a = vertices[v0];
            var pos_b = vertices[v1];
            var pos_c = vertices[v2];


            //a_b += Time.deltaTime;
            //ab_c += Time.deltaTime*Time.deltaTime;
            //a_b %= 1;
            //ab_c %= 1;

            Vector3 res = Vector3.Lerp(pos_a, pos_b, (i*0.1f)%1);
            res = Vector3.Lerp(res, pos_c, (i*0.78f)%1);
            _poss[buffCount] = res;

            buffCount++;
        }

    }

    /// <summary>
    /// 高精度版
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="_poss"></param>
    /// <param name="density"></param>
    static public void MeshBuffToSurfacePoints(Mesh mesh , out List<Vector3> _poss /*, out List<Vector3> _nors, out List<Vector3> _uvs */, float density = 10f)
    {
        _poss = new List<Vector3>();
        //_nors = new List<Vector3>();
        //_uvs = new List<Vector3>();

        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        //var normals = mesh.normals;
        //var uvs = mesh.uv;





        

        Debug.Log("Tri"+mesh.triangles.Length);

        for (var i = 0; i < triangles.Length; i += 3)
        {
            var v0 = triangles[i];
            var v1 = triangles[i + 1];
            var v2 = triangles[i + 2];

            var pos_a = vertices[v0];
            var pos_b = vertices[v1];
            var pos_c = vertices[v2];

            //var nor_a = normals[v0];
            //var nor_b = normals[v1];
            //var nor_c = normals[v2];

            //var uv_a = uvs[v0];
            //var uv_b = uvs[v1];
            //var uv_c = uvs[v2];



            var area = Mathaf.TriangleArea(pos_a, pos_b, pos_c);
            
            var areaDensity = Mathf.CeilToInt(area * density);

            for (var l = 0; l < areaDensity; l++)
            {
                var a_b = Random.Range(0f, 1f);
                var ab_c = Random.Range(0f, 1f);
                _poss.Add(SurfacePoint(pos_a, pos_b, pos_c, a_b, ab_c));
                //_nors.Add(SurfacePoint(nor_a, nor_b, nor_c, a_b, ab_c));
                //_uvs.Add(SurfacePoint(uv_a, uv_b, uv_c, a_b, ab_c));
            }
        }

    }
    static public Vector3 SurfacePoint(Vector3 a , Vector3 b , Vector3 c , float a_b , float ab_c)
    {
        Vector3 res = Vector3.Lerp(a, b, a_b);
        res = Vector3.Lerp(res, c, ab_c);
        return res;
    }

    /// <summary>
    /// 頂点データのＵＶ値からテクスチャの色を取得する
    /// 頂点データの数やテクスチャ情報の変更が無い事を前提として、初期化時に１度だけ呼び使用する事を推奨する
    /// UV値を保存してシェーダー側でテクスチャ色を拾わせる方法もあるけれど、若干こっちのが軽い、若干
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    static public List<Color> MeshToMap_Color_Init(Mesh mesh, Texture2D texture)
    {

        Vector2[] uvs = mesh.uv;

        float r = Mathf.Sqrt(uvs.Length);
        int width = (int)Mathf.Ceil(r);
        int height = width;

        List<Color> resuluts = uvs.Select(vtx =>

        texture.GetPixel((int)(vtx.x * texture.width) , (int)(vtx.y * texture.height))

        ).ToList();

        return resuluts;
    }

    /// <summary>
    /// バッファから２Ｄテクスチャを作成
    /// サイズはバッファ長から自動計算で正方形（端数部分は黒塗り）
    /// </summary>
    /// <param name="buff"></param>
    /// <returns></returns>
    static public  Texture2D CreateMap(IEnumerable<Color> buff , out Color[] buf)
    {
        float r = Mathf.Sqrt(buff.Count());
        int width = (int)Mathf.Ceil(r);
        int height = width;


        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;

        buf = new Color[width * height];

        int idx = 0;
        foreach (Color color in buff)
        {
            buf[idx] = color;
            idx++;
        }

        tex.SetPixels(buf);
        tex.Apply();

        return tex;
    }

}
