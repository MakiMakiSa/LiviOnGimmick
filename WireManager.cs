using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//スクリプト実行順、一番最後に実行
[DefaultExecutionOrder(int.MaxValue)]

public class WireManager : MonoBehaviour
{
    [SerializeField]
    LayerMask mask;
    public LayerMask Mask => mask;

    [SerializeField]float springPow = 10f;

    const float minRange = 1f;
    const float ofsRange = 0.1f;
    const float springClamp = 0.5f;
    class WireStates
    {
        public WireStates(Transform t , Rigidbody r , int n)
        {
            transform = t;
            rigidbody = r;
            num = n;
        }
        public Transform transform;
        public Rigidbody rigidbody;

        public int num;
    }

    //ベロのフックポイントリスト
    List<WireStates> WIres = new List<WireStates>();
    //フック状態か否か
    public bool isFuck => WIres.Count > 0;
    //フックしている、最後のひっかかりポイント
    public Transform FirstFuck => GetHit(0);

    //フックしている、次のひっかかりポイント
    public Transform NextFuck => WIres.Count > 1 ? GetHit(1) : FirstFuck;

    //最初に設置した糸の長さ
    float wireRange;
    //最初に設置した糸の長さがどれだけ引き延ばされているか
    public float currentWireRange
    {
        get
        {
            WireStates prevWs = null;
            float range = 0f;
            foreach(WireStates ws in WIres)
            {
                if(prevWs != null)
                {
                    range += Vector3.Distance(ws.transform.position,prevWs.transform.position);
                }
                prevWs = ws;
            }
            return range;
        }
    }
    Transform GetHit(int num)
    {
        if (num > WIres.Count - 1) num = WIres.Count - 1;
        return WIres[num].transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /// <summary>
    /// 衝突した三角形を描画
    /// </summary>
    void DrawHitTriangles(RaycastHit hit)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0,p1);
        Debug.DrawLine(p1,p2);
        Debug.DrawLine(p2,p0);

    }
    /// <summary>
    /// 既にリンクされた線と線の接触判定
    /// </summary>
    void CheckFuck()
    {

        WireStates prev = null;
        WireStates prevPrev = null;

        List<WireStates> addList = new List<WireStates>();
        List<WireStates> delList = new List<WireStates>();

        int num = 0;

        foreach (WireStates current in WIres)
        {
            if(prev != null)
            {
                if(Vector3.Distance(prev.transform.position , current.transform.position) > minRange)
                {
                    if (Physics.Linecast(prev.transform.position, current.transform.position, out RaycastHit hit, mask))
                    {
                        DrawHitTriangles(hit);

                        Add(hit.transform, hit.rigidbody,hit.normal, num, hit.point, addList ,true, current.transform);
                        num++;
                    }
                }

                if(prevPrev!= null)
                {
                    Vector3 thisPos = prevPrev.transform.position;
                    Vector3 nextPos = prev.transform.position;

                    Vector3 dotPos = thisPos - nextPos;

                    //float dot = Vector3.Dot((current.transform.forward+prev.transform.forward).normalized, dotPos);
                    //if (dot > ofsRange)
                    //{
                        if (!Physics.Linecast(thisPos, current.transform.position, out RaycastHit hit, mask))
                        {
                            delList.Add(prev);
                        }
                    //}
                }
            }

            prevPrev = prev;
            prev = current;
            num++;
        }

        //生成した点を本体のリストに追加
        foreach(WireStates ws in addList)
        {
            WIres.Insert(ws.num,ws);
        }

        //消えるべき点を本体から消す
        foreach(WireStates ws in delList)
        {
            WIres.Remove(ws);
        }
    }

    /// <summary>
    /// 最初の線のリンク
    /// </summary>
    /// <param name="sta">検索開始点</param>
    /// <param name="end">検索終了店</param>
    /// <param name="mask">マスク</param>
    /// <param name="thisRB">開始点の所有者のリジッドボディ（ワイヤーアクションでひっぱるため）</param>
    public void SearchFuck(Vector3 sta , Vector3 end  , Rigidbody thisRB)
    {
        if (Physics.Linecast(sta,end,out RaycastHit hit,mask))
        {
            Add(thisRB.transform,thisRB,-hit.normal,0 ,thisRB.position,WIres,false);
            Add(hit.transform,hit.rigidbody,hit.normal,0,hit.point,WIres,true);

            wireRange = currentWireRange;
            //当たったものにBreakSystemがついてれば跡を残す
            BreakSystem bs = hit.transform.GetComponent<BreakSystem>();
            if(bs)
            {
//                bs.BreakAtRay(hit);
                bs.BreakAtPoint(hit.point, null);
            }
        }
    }

    //リンク点と計算に使うゲームオブジェクトの追加
    void Add(Transform t , Rigidbody r , Vector3 normal ,int num , Vector3 ofsPos , List<WireStates> list , bool normalFlag,Transform prevTrans = null)
    {
        GameObject obj = new GameObject(t.name + ":Wire"+":Num"+WIres.Count);
        obj.transform.parent = t;
        obj.transform.position = ofsPos;

        obj.transform.localRotation = Quaternion.LookRotation(normal + (prevTrans? prevTrans.forward:Vector3.zero));//法線を斜めにする
        if(normalFlag)
        {
            obj.transform.position = obj.transform.position + obj.transform.forward * ofsRange;
        }

        list.Add(new WireStates(obj.transform,r,num));
    }
    /// <summary>
    //リンク点とそれに伴って生成したゲームオブジェクトの破棄
    /// </summary>
    /// <param name="ws">破棄するリストオブジェクト</param>
    void Remove(WireStates ws)
    {
        Destroy(ws.transform.gameObject);
        WIres.Remove(ws);
    }

    /// <summary>
    ///すべてのリンク点とゲームオブジェクトを破棄
    /// </summary>
    public void ResetFuck()
    {
        if (!isFuck) return;

        foreach (var ws in WIres)
        {
            Destroy(ws.transform.gameObject);
        }
        WIres.RemoveAll(b => true);
    }

    // Update is called once per frame
    /// <summary>
    /// 線の表示,線と線を引き合わせる
    /// </summary>
    void Update()
    {

        Vector3 prePos = transform.position;

        WireStates prev = null;
        float stretch = Mathf.Max(1f,currentWireRange - wireRange);

        foreach (var current in WIres)
        {
            Spring(prev, current,stretch);
            prev = current;



            //描画
            Debug.DrawLine(current.transform.position, prePos);
            Debug.DrawLine(current.transform.position, current.transform.position + current.transform.forward);
            prePos = current.transform.position;
        }
    }

    void Spring(WireStates prev , WireStates current , float stretch)
    {
        if (current == null || prev == null) return;

        if(current.rigidbody && prev.rigidbody)
        {
            if (prev.rigidbody == current.rigidbody)return;
        }

        Vector3 vel = Vector3.ClampMagnitude((prev.transform.position - current.transform.position) , springClamp);
        current.rigidbody?.AddForceAtPosition(vel * springPow*stretch,current.transform.position);
        prev.rigidbody?.AddForceAtPosition(vel * -springPow*stretch, prev.transform.position);


        ////20200601追加　フックオブジェクトのターゲットリジッドボディに親がいれば、その親にもアドフォース
        //if (current.rigidbody && current.rigidbody.transform.parent)
        //{
        //    Rigidbody parentRB = current.rigidbody.transform.parent.GetComponent<Rigidbody>();
        //    if (parentRB)
        //    {
        //        parentRB.AddForceAtPosition(vel * springPow * stretch *0.5f, current.transform.position);
        //    }
        //}
        //if (prev.rigidbody && prev.rigidbody.transform.parent)
        //{
        //    Rigidbody parentRB = prev.rigidbody.transform.parent.GetComponent<Rigidbody>();
        //    if (parentRB)
        //    {
        //        parentRB.AddForceAtPosition(vel * -springPow * stretch*0.5f, prev.transform.position);
        //    }
        //}
    }

    private void FixedUpdate()
    {
        if(WIres.Count >= 2)
        {
            WIres[0].transform.rotation = WIres[1].transform.rotation;
        }
        CheckFuck();
    }
}
