using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Rigidbody))]
public class SDManager : MonoBehaviour
{
    static public SDManager main;

    [SerializeField]
    Bullet bullet;

    List<Renderer> renderers = new List<Renderer>();

    static int sid_Dissolve = Shader.PropertyToID("Dissolve");
    static int sid_Ball_Weight = Shader.PropertyToID("Ball_Weight");
    static int sid_BaseColor = Shader.PropertyToID("BaseColor");

    Color emitColor = Color.blue;

    bool isWarping;
    float warpWeight;

    public Vector3 velocity => rb.velocity;

    Animator animator;



    [SerializeField]
    float agi = 100f;

    [SerializeField]
    PlayerManager playerBall;

    Vector3 targetPos
    {
        get
        {
            return playerBall.transform.position;
        }
    }
    PitPoint currentPit;
    public PitPoint CurrentPit => currentPit;

    [SerializeField]
    VisualEffect ve;

    [SerializeField]
    GameObject Bom;

    Rigidbody rb;

    float defaultDrag;
    float defaultAngularDrag;

    bool stayFlag;
    Vector3 stayPos;


    //アニメーション系
    int AnmID_Defense;
    int AnmID_isAnimated;



    float defenseDelay;

    //フィジックス系
    bool isLeftSide = false;


    float dilay = 0f;

    float darkColorWeight = 0f;



    public bool EnterBallingFlag => ballingFlag && !ballingFlag_prev;
    public bool ExitBallingFlag => !ballingFlag && ballingFlag_prev;

    bool ballingFlag = true;
    bool ballingFlag_prev = true;

    public bool EnterInPlayerFlag => inPlayerFlag && !inPlayerFlag_prev;
    public bool ExitInPlayerFlag => !inPlayerFlag && inPlayerFlag_prev;
    bool inPlayerFlag = true;
    bool inPlayerFlag_prev = true;


    KeyBuffer kb_L3;
    KeyBuffer kb_B;
    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;

        kb_L3 = KeyBufferManager.CreateKey();
        kb_B = KeyBufferManager.CreateKey();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        AnmID_Defense = Animator.StringToHash("isDefense");
        AnmID_isAnimated = Animator.StringToHash("isParticleAnimated");

        defaultDrag = rb.drag;
        defaultAngularDrag = rb.angularDrag;

        renderers.AddRange(GetComponentsInChildren<Renderer>());



    }


    // Update is called once per frame
    void Update()
    {
        //defense移行の時間制御
        defenseDelay -= Time.deltaTime;
        defenseDelay = Mathf.Clamp(defenseDelay, 0f, 1f);



        if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.L3))
        {
            kb_L3.SetEnter();
        }
        //設置切り替え
        if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.X))
        {
            stayFlag = !stayFlag;
            stayPos = transform.position;
        }
        //defenseの中身からっぽ切り替え
        if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.B))
        {
            kb_B.SetEnter();
        }





        //アニメーターセット
        animator.SetBool(AnmID_Defense, ballingFlag);
        animator.SetBool(AnmID_isAnimated, defenseDelay > 0f);

        //defense移行エフェクトセット
        if (ballingFlag && defenseDelay > 0f)
        {
            ve.Play();
        }
        else
        {
            ve.Stop();
        }



        //ボールウェイト
        float weightSpeed = 15f;
        darkColorWeight += inPlayerFlag ? Time.deltaTime * weightSpeed : -Time.deltaTime * weightSpeed;
        darkColorWeight = Mathf.Clamp(darkColorWeight, -5f, 5f);


        //でぃそるぶとボール闇落ちカラー変更
        warpWeight += isWarping ? Time.deltaTime : -Time.deltaTime;
        warpWeight = Mathf.Clamp(warpWeight, 0f, 1f);


        var mpb = new MaterialPropertyBlock();
        mpb.SetFloat(sid_Dissolve, ((warpWeight - 0.5f) * -2f));
        mpb.SetFloat(sid_Ball_Weight, darkColorWeight);
//        mpb.SetColor(sid_BaseColor, emitColor);
        foreach (Renderer r in renderers)
        {
            r.SetPropertyBlock(mpb);
        }



    }

    private void FixedUpdate()
    {

        float TimeSpeed = Time.deltaTime * 10f;

        //defenseモード切替
        ballingFlag_prev = ballingFlag;
        inPlayerFlag_prev = inPlayerFlag;

        if (kb_L3.EnterCheck(defenseDelay <= 0f))
        {

            ballingFlag = !ballingFlag;
            inPlayerFlag = ballingFlag;
            defenseDelay = 0.3f;

            if(ballingFlag)
            {
                currentPit = PitPoint.search(transform);
            }
            else
            {
                currentPit = null;
                rb.isKinematic = false;
                transform.parent = null;
            }
        }
        //inPlayerFlag切り替え
        if (kb_B.EnterCheck(ballingFlag && defenseDelay <= 0f))
        {
            inPlayerFlag = false;
        }


        //プレイヤー制御
        if (EnterInPlayerFlag)
        {
            playerBall.gameObject.SetActive(false);
        }
        if (ExitInPlayerFlag)
        {
            playerBall.Warp();
            playerBall.SetVelocity(rb.velocity);
            playerBall.gameObject.SetActive(true);
            playerBall.transform.position = rb.transform.position;
        }


        //defense状態と通常状態の分岐

        rb.freezeRotation = false;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 15f);
        if (ballingFlag)
        {
            isWarping = false;

            rb.drag = Mathf.Lerp(rb.drag, 0f, TimeSpeed);
            rb.angularDrag = Mathf.Lerp(rb.angularDrag, 0.3f, TimeSpeed);


            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.5f, TimeSpeed);
            gameObject.layer = LayerMask.NameToLayer("SD");

            if (defenseDelay > 0f)
            {
                //コックピットと共鳴中だったら、玉になる瞬間に引き寄せられる
                if(currentPit)
                {
                    transform.SetPositionAndRotation(Vector3.Lerp(transform.position, currentPit.transform.position+(Vector3.up*0.5f), Time.deltaTime * 7f), transform.rotation);

                }

                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                Look(false);

                dilay = 0.5f;
            }
            else
            {

                rb.useGravity = true;

                if (dilay > 0f)
                {
                    dilay -= Time.deltaTime;
                    dilay = Mathf.Clamp(dilay, 0f, 1f);
                }
                else
                {
                    DefenseMode();
                }
            }
        }
        else
        {
            //ボールから解除される時,爆発する
            //if (ballingFlag_prev)
            //{
            //    Bom.transform.position = transform.position;
            //    Bom.SetActive(true);
            //    Debug.Log("B!!!");
            //    //中身がいれば、垂直にジャンプさせる
            //    if(inPlayerFlag_prev)
            //    {
            //        Vector3 vel = rb.velocity;
            //        vel.y = 15f;
            //        playerBall.ballRB.velocity = vel;
            //    }
            //}

            rb.drag = Mathf.Lerp(rb.drag, defaultDrag, TimeSpeed);
            rb.angularDrag = Mathf.Lerp(rb.angularDrag, defaultAngularDrag, TimeSpeed);

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.5f, TimeSpeed);
            rb.useGravity = false;
            DefaultMode();
        }


        prevCollisionFlag = false;

        PitPoint.UD();
    }

    bool prevBrake = false;
    void DefenseMode()
    {
        //コックピットに完全にハマったら、制御系統を変更
        rb.isKinematic = false;
        if (currentPit)
        {
            if (currentPit.isRideOn)
            {
                if(!rb.isKinematic)
                {
                    transform.parent = currentPit.transform;
                    rb.isKinematic = true;
                }
                transform.SetPositionAndRotation(
                    currentPit.transform.position,
                    currentPit.transform.rotation
                    );

                return;

            }
        }
        else
        {
            transform.parent = null;
        }

        if (KeyManagerHub.main.GetKey(KeyMode.Stay, Key_Hub.R2))
        {
            emitColor = Color.yellow;
            rb.freezeRotation = true;
        }
        else if (inPlayerFlag)
        {
            Vector3 vel = KeyManagerHub.main.GetStickVectorFromCamera(true);

            if (vel.sqrMagnitude <= 0f)
            {
                emitColor = Color.black;
            }
            else
            {
                emitColor = Color.red;
            }

            vel = new Vector3(vel.z, vel.y, -vel.x) * 300f;

            vel = Vector3.Lerp(Vector3.zero, vel, Time.deltaTime);

            rb.AddTorque(vel, ForceMode.VelocityChange);

        }




    }

    void DefaultMode()
    {
        Vector3 upNormal = playerBall.Contacts.UpNormal_Ground;

        Vector3 tPos = targetPos;

        //float addSpeed = 0f;
        //rb.drag = Mathf.Lerp(rb.drag, defaultDrag, Time.deltaTime*10f);
        ////ジャンプ中は自分の足元にＳＤがくる
        //if (playerBall.isAerial && !stayFlag)
        //{
        //    tPos = targetPos - (Vector3.up*0.3f);
        //    addSpeed = 75f;
        //    rb.drag = 10f;
        //}
        //else
        //{
        if (stayFlag)
        {
            tPos = stayPos;
            emitColor = Color.green;
        }
        else
        {
            //坂道判定で左右offset
            emitColor = Color.blue;
            tPos = Vector3.ClampMagnitude(transform.position - tPos, 1f) + tPos;
            float dot = Vector3.Dot(Camera.main.transform.right, upNormal);
            if (dot > 0.3f) isLeftSide = true;
            else if (dot < -0.3f) isLeftSide = false;
            else
            {
                //画面ポス判定で左右offset
                Vector3 mySPos = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 parSPos = Camera.main.WorldToScreenPoint(targetPos);
                isLeftSide = (mySPos.x > parSPos.x);
            }
            tPos += Camera.main.transform.right * (isLeftSide ? 0.5f : -0.5f);
            tPos += (upNormal * 0.5f);
        }
        //}

        //プレイヤーと重ならないように補正
        //        tPos.y += Mathf.Clamp((2f - Vector3.Distance(transform.position, parent.transform.position)), 0f, 1f) * 1f;

        //射撃
        //if(KeyManagerHub.main.GetKey(KeyMode.Enter , Key_Hub.R1))
        //{
        //    Instantiate(bullet, transform.position, transform.rotation).Init();
        //    rb.AddForce(-transform.forward*3f, ForceMode.Impulse);
        //}

        //距離が離れすぎたらワープ
        float tDist = Vector3.Distance(tPos, transform.position);
        isWarping = tDist > 3f;


        //プレイヤーとカメラの間に入らないように補正
        Vector3 playerToCameraPos =  Mathaf.NearPosOnLine(transform.position, Camera.main.transform.position, targetPos);

        Vector3 ptpVel =  transform.position- playerToCameraPos;

        rb.AddForce(ptpVel * Mathf.Clamp(2f - ptpVel.magnitude, 0f, 1f)*Time.deltaTime*60f, ForceMode.VelocityChange);


        if (warpWeight >= 1f)
        {
            transform.position = tPos;
        }






        //加速移動
        Vector3 vel = tPos - transform.position;
        vel = Vector3.ClampMagnitude(vel, 2f);
        vel = Vector3.Lerp(Vector3.zero, vel, Time.deltaTime);
        rb.AddForce(vel * (agi/*+addSpeed*/), ForceMode.VelocityChange);





        //        Vector3 radVel = Quaternion.LookRotation(Camera.main.transform.forward).eulerAngles - transform.eulerAngles;

        Look(true);

    }

    void Look(bool accelFlag)
    {
//        Quaternion toRad = Quaternion.LookRotation(/*targetPos - transform.position*/Camera.main.transform.forward);
        Quaternion toRad = Quaternion.LookRotation(CameraManager_Cinema.main.TargetPos - transform.position);
        if (accelFlag)
        {

            Vector3 radVel = toRad.eulerAngles - transform.eulerAngles;
            radVel = Mathaf.NormalizeRads(radVel);
            radVel = Vector3.Lerp(Vector3.zero, radVel, Time.deltaTime);
            rb.AddRelativeTorque(radVel, ForceMode.VelocityChange);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Lerp(transform.rotation, toRad, Time.deltaTime * 10f);


            //ランダムに震える
            float r = 5f;
            rb.AddRelativeTorque(new Vector3(UnityEngine.Random.Range(-r, r), UnityEngine.Random.Range(-r, r), UnityEngine.Random.Range(-r, r)), ForceMode.VelocityChange);


        }
    }


    bool prevCollisionFlag = false;

    public void Hop()
    {
        if (prevCollisionFlag)
        {

            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }
    }


    //private void OnCollisionStay(Collision collision)
    //{
    //    prevCollisionFlag = true;
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        //            rb.AddForce(parent.ballRB.velocity*Time.deltaTime , ForceMode.VelocityChange);
    //        //            rb.velocity = parent.ballRB.velocity * 1.5f;
    //        //            rb.velocity = collision.contacts[0].normal*30f;

    //    }
    //}
}
