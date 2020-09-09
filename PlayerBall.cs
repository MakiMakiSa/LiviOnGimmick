using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class MinMaxRange
{
    public float min;
    public float max;
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBall : MonoBehaviour
{

    //移動速度
    [SerializeField, Range(1,50000)]
    float moveSpeed = 50000f;
    //ステップスピード
    [SerializeField, Range(1,10)]
    float stepSpeed = 10;
    //ジャンプ力
    [SerializeField, Range(1, 10000)]
    float jumpSpeed = 10000f;

    //ジャンプ間隔
    const float jumpRecastTime = 0.3f;
    const float stepRecastTime = 0.5f;
    float groundRecastTimer;


    const float magForce = 500f;

    Vector3 currentVector;
    public Vector3 CurrentVector => currentVector;
    Vector3 LastVector;
    public Vector3 Forward
    {
        get
        {
            return LastVector;
        }
    }


    Vector3 aimPos;
    public Vector3 AimPos => aimPos;
    
     

    //キーマネージャー、これがついていたらプレイヤー操作をうけつける
    [SerializeField]
    KeyManagerHub keyManagerHub;
    [SerializeField]
    CameraManager cameraManager;
    [SerializeField]
    WireManager wireManager;

    //各種フィジックスマテリアル
    [SerializeField]
    PhysicMaterial BallingPhysic;
    [SerializeField]
    PhysicMaterial IcePhysic;
    PhysicMaterial HumanPhysic;

    //現在何かに接触しているフラグ
    bool isCollision;
    public bool IsCollision => isCollision;

    //地面のノーマルが上下反転状態のとき、操作を反転させる計算をするか否かのフラグ
    bool reverseFlag = false;

    //現在接触しているすべての衝突情報
    List<ContactPoint> currentCollisionStates = new List<ContactPoint>();
    //現在接触しているすべての面の法線の平均
    public Vector3 CurrentCollisionNormal
    {
        get
        {
            if (currentCollisionStates.Count <= 0) return Vector3.up;
            Vector3 ave = Vector3.zero;
            ave.x = currentCollisionStates.Average(b => b.normal.x);
            ave.y = currentCollisionStates.Average(b => b.normal.y);
            ave.z = currentCollisionStates.Average(b => b.normal.z);
            return ave;
        }
    }
    //現在接触している面の法線(すべての法線の中で、一番上をむいているものを抜粋）
    public Vector3 CurrentCollisionNormal_Up => IsCollision ? LastCollisionNormal_Up: Vector3.up;

    //最後に接触していた面の法線
    public Vector3 LastCollisionNormal_Up
    {
        get
        {
            if (currentCollisionStates.Count <= 0) return Vector3.up;
            return currentCollisionStates.OrderByDescending(b => b.normal.y).ToList()[0].normal;
        }
    }

    //直立できる地面の角度
    [SerializeField]
    float groundRad = 30f;
    //何かに接触していて、それが直立できる地面か否か
    public bool isGround => IsCollision && (Vector3.Angle(CurrentCollisionNormal_Up, Vector3.up) < groundRad);

    [SerializeField]
    MinMaxRange DragState;
    [SerializeField]
    MinMaxRange DragState_Angler;
    

    //ダッシュフラグ
    bool isDash;
    //しゃがみフラグ
    bool isSquat;
    float squatWeight;
    public float SquatWeight => squatWeight;


    //自分のリジッドボディ
    Rigidbody rb;
    [SerializeField]
    public Rigidbody RB => rb;
    //自分のコライダ
    SphereCollider col;
    public SphereCollider Col => col;

    //自分の座標
    public Vector3 Position => rb.position;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = float.MaxValue;

        col = GetComponent<SphereCollider>();

        HumanPhysic = col.material;

    }
    /// <summary>
    /// 操作系の処理はこっち
    /// </summary>
    void Update()
    {
    }
    /// <summary>
    /// プレイヤー操作をうけた移動
    /// </summary>
    void PCMove(KeyManagerHub Key)
    {
        Vector3 stickVel = -Key.GetStickVectorFromNormal(true , CurrentCollisionNormal_Up , reverseFlag);
        float stickLen = stickVel.magnitude;


        bool key_Balling = Key.GetKey(KeyMode.Stay , Key_Hub.L2);
        bool key_Jump = Key.GetKey(KeyMode.Enter   , Key_Hub.R2);
        bool key_JumpStay = Key.GetKey(KeyMode.Stay, Key_Hub.R2);

        bool key_Aim = Key.GetKey(KeyMode.Stay     , Key_Hub.L1);
        bool key_Attack = Key.GetKey(KeyMode.Stay  , Key_Hub.R1);

        bool key_Dash = Key.GetKey(KeyMode.Stay    , Key_Hub.L3);

        bool key_Squat = Key.GetKey(KeyMode.Enter, Key_Hub.R3);

        if(key_Squat)
        {
            isSquat = !isSquat;
        }
        squatWeight += Time.deltaTime * (isSquat ? 2f : -2f);
        squatWeight = Mathf.Clamp(squatWeight, 0f, 1f);

        bool notDashFlag = key_Aim;
        if (notDashFlag) isDash = false;
        if(isDash)
        {
            isSquat = false;
        }



        //さかさま判定
        if (stickLen <= 0f)
        {
            reverseFlag = CurrentCollisionNormal_Up.y < 0f ? true:false;
        }
        if (CurrentCollisionNormal_Up.y > 0f) reverseFlag = false;

        rb.freezeRotation = false;
        rb.angularDrag = DragState_Angler.min;
        rb.useGravity = true;

        if (key_Balling)//ボール状態
        {
            wireManager.ResetFuck();

            if (key_JumpStay)
            {
                rb.freezeRotation = true;
            }

            rb.drag = DragState.min;

            col.material = BallingPhysic;
        }
        else //通常状態
        {

            JumpAction(key_Jump);




            if(wireManager.isFuck)
            {
                isDash = true;
                rb.useGravity = false;
            }
            if (IsCollision)
            {
                rb.useGravity = false;
                if (!isGround)
                {
                    isDash = false;
                }
                else if (stickLen <= Mathf.Epsilon)
                {
                    stickVel = Vector3.zero;
                    rb.angularDrag = DragState_Angler.max;
                }

                if(stickLen < 0.9f) isDash = false;


                rb.drag = DragState.max;

                if (key_Dash && !notDashFlag) isDash = true;

                col.material = HumanPhysic;
            }
            else
            {
                col.material = IcePhysic;
                rb.drag = DragState.min;
                stickVel *= 0.15f;
                isDash = false;
            }

            currentVector = stickVel;
            if(stickVel.magnitude > 0f)
            {
                LastVector = stickVel;
            }
            rb.AddForce(stickVel * Time.deltaTime * moveSpeed * (isDash ? 1f : 0.5f));

        }



        Color debCol = isGround ? Color.red : Color.green;


        Debug.DrawLine(rb.position,rb.position + (CurrentCollisionNormal_Up * 3f),debCol);
        Debug.DrawLine(rb.position,rb.position + (LastCollisionNormal_Up * 3f),debCol);



        if (groundRecastTimer > 0f) groundRecastTimer -= Time.deltaTime;

    }

    void JumpAction(bool enter)
    {
        //ジャンプ系動作の発動計算
        if (IsCollision && groundRecastTimer <= 0f)
        {
            if (enter)
            {
                //ジャンプ
                Jump();
            }
        }
    }

    //ホバリング
    bool Hover()
    {

        if (rb.velocity.y < 0f)
        {
            rb.AddForce(Vector3.up * 220f);
            rb.velocity *= 0.95f;
            return true;
        }

        return false;
    }
    //武器、ベロ、ショット管理
    void ShotManage(RaycastHit aim,bool aimFlag , bool attackFlag)
    {
        if (!attackFlag && wireManager.isFuck) wireManager.ResetFuck();

        if(aimFlag)
        {

        }
        else
        {
            if(attackFlag)
            {
                FuckShot(aim);
            }
        }
    }
    //ベロショット
    void FuckShot(RaycastHit aim)
    {
        if(!wireManager.isFuck)
        {
            wireManager.SearchFuck(transform.position , aim.point ,  rb);
        }
    }

    //ジャンプ
    void Jump()
    {
        if (groundRecastTimer > 0f) return;
        groundRecastTimer = jumpRecastTime;
        rb.velocity *= 0.9f;

        Vector3 jumpVel = CurrentCollisionNormal_Up;

        if(!wireManager.isFuck)jumpVel.y = jumpVel.y >= -0.1f? 1f:-1f;

        rb.AddForce(jumpVel* jumpSpeed);
    }

    /// <summary>
    /// 物理演算系のしょりはこっち
    /// </summary>
    private void FixedUpdate()
    {
        if (keyManagerHub)
        {
            PCMove(keyManagerHub);
        }
        //何かに接触したか否かのフラグをリセット
        isCollision = false;
//        transform.parent = null;
    }

    /// <summary>
    /// 何かに接触した場合、isCollisionを立て、接触した面の法線をcurrentCullisionNormalに入れる
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
 
//        transform.parent = collision.transform;

        if(groundRecastTimer <= 0f)
        {
            if (!IsCollision)
            {
                isCollision = true;
                currentCollisionStates = new List<ContactPoint>(collision.contacts);
            }
            else
            {
                currentCollisionStates.AddRange(collision.contacts);
            }
        }

        foreach(ContactPoint cp in collision)
        {
            //自分を壁や地面に押し当てる
            bool flag = true;
            if(collision.rigidbody)
            {
                if (rb.mass > collision.rigidbody.mass) flag = false;
            }
            if(flag)
            {
                rb.AddForce(-cp.normal * magForce);
                collision.rigidbody?.AddForce(cp.normal * magForce);
            }

        }
    }
}
