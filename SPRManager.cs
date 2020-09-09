using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[DefaultExecutionOrder(-10000)]
public class SPRManager : MonoBehaviour
{
    List<SPR> sprs = new List<SPR>();

    // Start is called before the first frame update
    void Start()
    {
        InitSPR(transform , 0);
        sprs = sprs.OrderBy(b => b.count).ToList();
    }

    void InitSPR(Transform p , int count)
    {
        if (p.childCount <= 0) return;

        if (p.name.Contains("_SPR_"))//ジョイントを付けたいターゲットなのでつける
        {
            count++;

            Rigidbody parRB = p.GetComponent<Rigidbody>();//リジッドボディを取得（最親だった場合付いてない）
            var joint = p.gameObject.AddComponent<CharacterJoint>();//キャラジョイントを付けると自動でリジッドボディがつく
            if(!parRB)
            {
                p.GetComponent<Rigidbody>().isKinematic = true;
            }

            foreach (Transform t in p)
            {
                //リジッドボディを取得、なければ付ける
                Rigidbody rb = t.gameObject.GetComponent<Rigidbody>();
                if(!rb)rb = t.gameObject.AddComponent<Rigidbody>();

                //子を切り離す
                t.parent = null;

                //親のジョイントに自分を指定
                joint.connectedBody = rb;

                rb.drag = 0.1f;

                //スプリング生成
                SPR spr = new SPR(rb, p, count);

                //生成したスプリングをリストに追加
                sprs.Add(spr);


                //再帰
                InitSPR(t , count);
            }
        }
        else//無関係なトランスフォームなので子を探す
        {
            foreach (Transform t in p)
            {
                //再帰
                InitSPR(t , count);
            }
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //foreach(SPR s in sprs)
        //{
        //    s.Move();
        //}
    }
}



public class SPR
{
    Rigidbody parRB;
    Transform parent;
    Rigidbody rb;
    Vector3 defaultLocalPos;
    Quaternion defaultLocalRad;
    public int count;

    Transform transform => rb.transform;

    public SPR(Rigidbody r , Transform p , int c)
    {
        count = c;
        parent = p;
        rb = r;

        parRB = parent.GetComponent<Rigidbody>();

        defaultLocalPos = r.transform.localPosition;
        defaultLocalRad = r.transform.localRotation;
    }

    public void Move()
    {

//        Quaternion toRad = Quaternion.LookRotation(transform.position - parent.position , parent.up);
        
        if(parRB)
        {
//           parent.LookAt(transform.position);
        }
///        transform.position = parent.TransformPoint(defaultLocalPos);
//        Vector3 toRad = parent.TransformVector(defaultLocalRad.eulerAngles);

        //回転移動
//        Vector3 radVel = toRad - rb.transform.rotation.eulerAngles;

//        radVel = Mathaf.NormalizeRads(radVel);
//        radVel = Vector3.Lerp(Vector3.zero, radVel, Time.deltaTime);

 //       rb.AddRelativeTorque(radVel, ForceMode.VelocityChange);


        //加速移動
        //        Vector3 vel = toPos - rb.transform.position;
        //        vel = Vector3.ClampMagnitude(vel, 2f);
        //        vel = Vector3.Lerp(Vector3.zero, vel, Time.deltaTime);
        //        rb.AddForce(vel * 60f, ForceMode.VelocityChange);
        //        transform.position = toPos;

    }

}

