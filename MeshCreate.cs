using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

static public class MeshCreate
{
    struct VList
    {
        public Vector3 pos;
        public int num;
    }

    //ゲームオブジェクトからメッシュコライダを作成
    static public List<GameObject> CreateFromMeshCollider(GameObject obj , LayerMask layer)
    {
        List<GameObject> results = new List<GameObject>();

        var smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

        //可変メッシュ
        foreach (var smr in smrs)
        {
            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);

            GameObject meshObj = new GameObject("CreatedMeshColliderObj");
            meshObj.layer = layer;
            meshObj.transform.position = smr.transform.position;
            meshObj.transform.rotation = smr.transform.rotation;

           

            MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshCollider meshCollider = meshObj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            {
                MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterials = smr.materials;
            }


            results.Add(meshObj);

        }

        return results;
    }

    static public GameObject Create(Vector3[] vertexs ,bool isTrigger ,Material material , float scale = 3f , int cutDistance = 10)
    {
        //扱いやすいよう整列する
        List<Vector3> V = new List<Vector3>();
        for(int i= 0; i< vertexs.Length; i++)
        {
            Vector3 p = vertexs[i]*scale;
            p.x = (int)p.x;
            p.z = (int)p.z;
            V.Add(p);
        }
        V = V.Distinct().ToList();
        V = V.OrderBy(b => b.x).ToList();


        //VListリストを作成
        List<VList> vs = new List<VList>();
        for (int i = 0; i < V.Count; i++)
        {
            VList v;
            v.pos = V[i];
            v.num = i;

            vs.Add(v);

        }

        //２次元配列のサイズを計算
        float minX = V.OrderBy(d => d.x).ToArray()[0].x;
        float maxX = V.OrderByDescending(d => d.x).ToArray()[0].x;
        float minZ = V.OrderBy(d => d.z).ToArray()[0].z;
        float maxZ = V.OrderByDescending(d => d.z).ToArray()[0].z;

        int xSize = (int)(maxX - minX);
        int zSize = (int)(maxZ - minZ);

        Debug.Log("x"+xSize + "z" + zSize);

        VList[,] list2D = new VList[xSize,zSize];
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                Vector3 c = new Vector3(x+minX, 0, z+minZ);
                VList vl = vs.Find(b => ((b.pos.x == c.x) && (b.pos.z == c.z)));

                list2D[x, z].pos = vl.pos;
                list2D[x, z].num = vl.num;


            }
        }



        //三角形つくる
        List<int> triangles = new List<int>();

        for (int x = 0; x < xSize-1; x++)
        {
            for (int z = 0; z < zSize-1; z++)
            {
                VList vl1 = list2D[x, z];
                VList vl2 = list2D[x, z+1];
                VList vl3 = list2D[x+1, z];
                VList vlE = list2D[x+1, z+1];

                if (vl1.num != 0 && vl2.num != 0 && vl3.num != 0 && vlE.num != 0)
                {
                    triangles.Add(vl1.num);
                    triangles.Add(vl2.num);
                    triangles.Add(vl3.num);

                    triangles.Add(vl2.num);
                    triangles.Add(vlE.num);
                    triangles.Add(vl3.num);
                }
            }
        }

        //拡大したぶんのサイズを元に戻す
        V = V.Select(b => b /= scale).ToList();




        Mesh mesh = new Mesh();
        mesh.vertices = V.ToArray();
        mesh.triangles =triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject meshObj = new GameObject("CreatedMeshObj");
//        meshObj.transform.position = centerPos;


        MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshCollider meshCollider = meshObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        if(isTrigger)
        {
            meshCollider.convex = true;
            meshCollider.isTrigger = isTrigger;
        }

        if (material)
        {
            MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
        }




        return meshObj;



    }

}
