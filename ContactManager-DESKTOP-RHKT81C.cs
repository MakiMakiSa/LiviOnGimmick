using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    PhysicMaterial defaultPhysic;
    [SerializeField]
    PhysicMaterial slipPhysic;


    public bool fitFlag = true;

    Vector3 defaultScale;

    Rigidbody rb;
    SphereCollider col;

    //    float WallingStamina = 10f;

    public Vector3 LastPhysicsVector => lastPhysicsVector; 
    Vector3 lastPhysicsVector;



    List<ContactPoint> contactlList = new List<ContactPoint>();//現在接触中の情報リスト

    public ContactPoint upNormalContact;//現在接触しているコンタクトの中で、法線が一番上を向いているもの

    public Vector3 upNormal_Ex
    {
        get
        {
            Vector3 ofs = Vector3.up;
            if(IsAnyContact)
            {
                ofs = transform.position - upNormalContact.point;
            }

            return ofs;
        }
    }

    public Rigidbody GetUpNormalRB()
    {
        if (IsAnyContact)
        {
            if (upNormalContact.otherCollider)
            {
                return upNormalContact.otherCollider.attachedRigidbody;
            }
        }
        return null;
    }

    public Vector3 UpNormal_Ground => (IsWalling && fitFlag) ? upNormalContact.normal : Vector3.up;
    public Vector3 UpNormal_Simple => upNormal_Ex; //IsAnyContact ? upNormalContact.normal:Vector3.up;
    public Vector3 contactPos => IsAnyContact ? upNormalContact.point : transform.position;

    bool isGround;//現在接触している法線が、地面に値するか否か
    bool isGround_prev;//前回フレームのisGround
    public bool IsGround => isGround;
    public bool IsGround_prev => isGround_prev;

    public bool isNowLanding => IsAnyContact && !isAnyContact_prev;

    //何かに接触しているか否か
    public bool IsAnyContact => ((contactlList.Count > 0) && (fitFlag || isGround) );
    bool isAnyContact_prev;




    float stamina = 2.0f;

    public bool ContactRBFlag
    {
        get
        {
            if (upNormalContact.otherCollider)
            {
                if (upNormalContact.otherCollider.attachedRigidbody)
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
            return flag;
        }
    }

    int contactCounter = 0;



    Vector3 impact;
    public void setImpact(Vector3 v , float p = 1f)
    {
        if(v.magnitude > impact.magnitude)
        {
            impact = Vector3.Lerp(impact, v, Time.deltaTime * p);
            impact = Vector3.ClampMagnitude(impact, 0.25f);
        }
    }

    public void DragImpact()
    {
        impactVector = Vector3.Lerp(impactVector, impact, Time.deltaTime * 30f);
        impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 30f);
    }

    Vector3 impactVector;
    public Vector3 Impact => impactVector;




    bool isPhysicsUpdate = false;


    public void Reset()
    {
        contactlList = new List<ContactPoint>();
        isGround = false;
        contactCounter = 0;
        isPhysicsUpdate = true;
        impact = Vector3.up*5f;
        impactVector = impact;
    }

    private void Start()
    {
        defaultScale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        col = rb.GetComponent<SphereCollider>();



    }

    private void Update()
    {
        impactVector = Vector3.Lerp(impactVector, impact, Time.deltaTime * 7f);
        impact = Vector3.Lerp(impact , Vector3.zero , Time.deltaTime*7f);

        //fitWeight += isGround ? Time.deltaTime*30f : -Time.deltaTime;
        //fitWeight = Mathf.Clamp(fitWeight, 0f, stamina);

        ////壁走りのスタミナ変動
        //if(isGround)
        //{
        //    WallingStamina = 1f;
        //}
        //else
        //{
        //    WallingStamina -= Time.deltaTime;
        //}

        //WallingStamina = Mathf.Clamp(WallingStamina, 0f, 1f);
        //if(WallingStamina <= 0f)
        //{
        //    col.material = slipPhysic;
        //}
        //else
        //{
        //    col.material = defaultPhysic;
        //}

    }
    private void FixedUpdate()
    {
        lastPhysicsVector = rb.velocity;
        isAnyContact_prev = IsAnyContact;

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
            isGround = false;

            //全ての接触情報から一番上向きの法線のものを取得
            if (contactlList.Count > 0)
            {
                Vector3 normal = Vector3.down * float.MaxValue;
                upNormalContact = contactlList.OrderByDescending(b => b.normal.y).ToArray()[0];
                //一番上向きの法線の上に立てるか否か判定
                float d = Vector3.Dot(Vector3.up, upNormalContact.normal);
                isGround = d > tweak.groundUp;
            }
            else
            {
                //何にも接触していなければ、法線は上、設置フラグオフ
                upNormalContact = new ContactPoint();
                isGround = false;
            }

            //if (IsMassOfMin)
            //{
            //    isGround = false;
            //}

            Vector3 lossScale = transform.lossyScale;
            Vector3 localScale = transform.localScale;
            transform.localScale = new Vector3(
                            localScale.x / lossScale.x * defaultScale.x,
                            localScale.y / lossScale.y * defaultScale.y,
                            localScale.z / lossScale.z * defaultScale.z);


            rb.useGravity = true;
            //張り付き処理
            rb.drag = minDrag;
            if (IsAnyContact)
            {
//                rb.useGravity = false;

                rb.drag = 1f;
                //地面が自分より重いか、地面のリジッドボディがキネマティックならば、その地面を自分の親とする
                if (IsMassOfMin || GetUpNormalRB().isKinematic)
                {
                    transform.parent = upNormalContact.otherCollider.transform;
                    Debug.DrawLine(transform.position, transform.parent.position, Color.magenta);
                }
                else
                {
                    transform.parent = null;
                }


                ////現在ベクトルを表面に沿わせる
                //Vector3 v = Vector3.ProjectOnPlane(rb.velocity , upNormalContact.normal);
                //rb.velocity = v;

                float tweakTension = 50f;
                Vector3 surfaceVector = -upNormalContact.normal * tweakTension * Mathaf.deltaClampTime(Physics.gravity.magnitude);

                if (fitFlag)
                {
                    //表面張力（自分張り付き判定用）

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
            else
            {
                transform.parent = null;
            }

        }


    }
    /// <summary>
    //フレーム更新後、最初に呼ばれたコリジョンステイかエンターが、全てを葬る
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
        CheckPhysicsUpdate();

        contactCounter++;

        impact = collision.relativeVelocity * 0.15f;
        impact = Vector3.ClampMagnitude(impact, 1f);
    }

    private void OnCollisionExit(Collision collision)
    {
//        Debug.Log("E!" + contactCounter + "->" + (contactCounter - 1));

        contactCounter--;

        if (contactCounter <= 0)
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
