using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class ContactManager : MonoBehaviour
{

    [System.Serializable]
    class TweakParam
    {
        public float groundUp = 0.85f;  //立てる地平の法線方向からの誤差
    }
    [SerializeField]
    TweakParam tweak;//調整用パラメーター

    [SerializeField]
    float minDrag = 0.3f;


    public bool fitFlag = true;

    Vector3 defaultScale;

    Rigidbody rb;
    SphereCollider col;



    List<ContactPoint> contactlList = new List<ContactPoint>();//現在接触中の情報リスト

    ContactPoint upNormalContact;//現在接触しているコンタクトの中で、法線が一番上を向いているもの

    public Rigidbody GetUpNormalRB()
    {
        if(IsAnyContact)
        {
            if(upNormalContact.otherCollider)
            {
                return upNormalContact.otherCollider.attachedRigidbody;
            }
        }
        return null;
    }

    public Vector3 UpNormal => (IsWalling && fitFlag )? upNormalContact.normal : Vector3.up;
    public Vector3 contactPos => IsAnyContact ? upNormalContact.point : transform.position;

    bool isGround;//現在接触している法線が、地面に値するか否か
    bool isGround_prev;//前回フレームのisGround
    public bool IsGround => isGround;
    public bool IsGround_prev => isGround_prev;

    //何かに接触しているか否か
    public bool IsAnyContact => (contactlList.Count > 0);

    public bool ContactRBFlag
    {
        get
        {
            if(upNormalContact.otherCollider)
            {
                if(upNormalContact.otherCollider.attachedRigidbody)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool IsMassOfMin
    {
        get
        {
            bool flag = true;
            Rigidbody b = GetUpNormalRB();
            if (b && b.mass <= rb.mass * 2f) flag = false;

            return flag;
        }

    }
    public bool IsWalling
    {
        get
        {
            bool flag = IsAnyContact && !isGround && fitFlag;
            if (!IsMassOfMin) flag = false;
            return  flag;
        }
    }

    int contactCounter = 0;


    Vector3 impact;
    public void setImpact(Vector3 v)
    {
        impact += (v - impact) * Time.deltaTime;
        impact = Vector3.ClampMagnitude(impact, 0.25f);
    }

    Vector3 impactVector;
    public Vector3 Impact => impactVector;




    bool isPhysicsUpdate = false;

    private void Start()
    {
        defaultScale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        col = rb.GetComponent<SphereCollider>();

    }

    private void Update()
    {
        impactVector = Vector3.Lerp(impactVector, impact, Time.deltaTime*10f);
        impact -= impact * 7f * Time.deltaTime;

    }
    private void FixedUpdate()
    {
        isPhysicsUpdate = false;


        //判定が行われた後に呼ばれる
        StartCoroutine(fixFunction());
    }

    IEnumerator fixFunction()
    {
        yield return new WaitForFixedUpdate();




        //もしここに至るまでに接触判定の更新がなされていた場合、isPhysicsUpdateがtrueになっているはずだから、そしたらこっちの接触情報もアップデートする
        if (isPhysicsUpdate)
        {
            isGround_prev = isGround;

            //全ての接触情報から一番上向きの法線のものを取得
            if (contactlList.Count > 0)
            {
                Vector3 normal = Vector3.down*float.MaxValue;

                foreach (ContactPoint c in contactlList)
                {
                    if (c.normal.y > normal.y)
                    {
                        normal = c.normal;
                        upNormalContact = c;
                    }
                }



                //一番上向きの法線の上に立てるか否か判定
                Vector3 un = UpNormal;
                float d = Vector3.Dot(Vector3.up, un);
                isGround = d > tweak.groundUp;

            }
            else
            {
                //何にも接触していなければ、法線は上、設置フラグオフ
                upNormalContact = new ContactPoint();
                isGround = false;
            }

            if(IsMassOfMin)
            {
                isGround = false;
            }


            transform.parent = null;
            transform.localScale = defaultScale;
            rb.useGravity = true;
            //張り付き処理
            float tweakTension = 50f;
            Vector3 surfaceVector = -UpNormal * tweakTension * Mathaf.deltaClampTime(Physics.gravity.magnitude);

            rb.drag = minDrag;
            if (IsAnyContact)
            {
                rb.drag = 1f;
                //if(upNormalContact.otherCollider && upNormalContact.otherCollider.attachedRigidbody)
                //{

                //}
                //else
                //{
                //}
                if(IsMassOfMin)
                {
                    transform.parent = upNormalContact.otherCollider.transform;
                    Debug.DrawLine(transform.position, transform.parent.position, Color.magenta);
                }


                ////現在ベクトルを表面に沿わせる
                //Vector3 v = Vector3.ProjectOnPlane(rb.velocity , UpNormal);
                //transform.position = upNormalContact.point + (UpNormal * col.radius);

                if (fitFlag)
                {
                    //表面張力（自分張り付き判定用）
                    rb.useGravity = false;

                    rb.AddForceAtPosition(surfaceVector, contactPos);
                }
                if (ContactRBFlag)
                {
                    //表面張力（親からの反発判定用）
                    upNormalContact.otherCollider.attachedRigidbody.AddForceAtPosition(-surfaceVector, contactPos);
                    //重みを加える
                    upNormalContact.otherCollider.attachedRigidbody.AddForceAtPosition(Vector3.down * tweakTension * Mathaf.deltaClampTime(Physics.gravity.magnitude), contactPos);

                }
                else
                {
                }

            }

        }

    }
    /// <summary>
    //フレーム更新後、最初に呼ばれたコリジョンステイが、全てを葬る
    /// </summary>
    void CheckPhysicsUpdate()
    {
        if (!isPhysicsUpdate)
        {
            contactlList.RemoveAll(b => true);
            isPhysicsUpdate = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {

        CheckPhysicsUpdate();
        //if(!collision.rigidbody)
        //{
        //}
        contactlList.AddRange(collision.contacts);
    }

    private void OnCollisionEnter(Collision collision)
    {

        contactCounter++;

        impact += collision.relativeVelocity * 0.1f;
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("E!" + contactCounter + "->" +(contactCounter-1));

        contactCounter--;

        if(contactCounter <= 0)
        {
            isPhysicsUpdate = false;
        }
        
        CheckPhysicsUpdate();

        //foreach (ContactPoint c in collision.contacts)
        //{
        //    contactlList.Remove(c);
        //}

    }


    public void Jump(Vector3 jumpingVel)
    {
        Debug.Log("j!");

        rb.drag = minDrag;
        //ジャンプする瞬間、足元にあるオブジェクトをけってアドフォースする
        if (IsAnyContact && upNormalContact.otherCollider)
        {
            if (upNormalContact.otherCollider.attachedRigidbody)
            {
                upNormalContact.otherCollider.attachedRigidbody.AddForceAtPosition(-jumpingVel, upNormalContact.point, ForceMode.Impulse);

            }
        }

    }
}
