using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[DefaultExecutionOrder(99)]

[RequireComponent(typeof(ContactManager))]
public class PlayerManager : MonoBehaviour
{
    static public PlayerManager main;

    [System.Serializable]
    class States
    {
        public float agi;//移動速度
        public int restStepCount;
    }

    [System.Serializable]
    class TweakParam
    {
        public float avaterRotationSpeed = 10f;//表示アバターの方向転換速度
        public float minStickMagnitued = 0.02f;//スティック入力の最小値
        public float jumpingPower = 4f;         //ジャンプ力
        public float specialIdleTime = 10f;            //操作をしない事による特殊アイドルへの移行
    }

    //各種ステータス
    [SerializeField]
    States states;
    [SerializeField]
    TweakParam tweakParam;

    [SerializeField]
    LayerMask handMask;



    //アバターについてるコンポ(アニメーションよりのフィールド）
    [SerializeField]
    Animator animator;
    PlayerIKManager ikManager;
    BreathingManager breathingManager;

    int animID_speed = Animator.StringToHash("speed");
    int animID_speed_X = Animator.StringToHash("speed_X");
    int animID_speed_Z = Animator.StringToHash("speed_Z");
    int animID_speed_Y = Animator.StringToHash("speed_Y");
    int animID_isAerial = Animator.StringToHash("isAerial");
    int animID_isLeft = Animator.StringToHash("isLeft");
    int animID_isWalling = Animator.StringToHash("isWalling");

    int animID_stepTrigger = Animator.StringToHash("stepTrigger");
    int animID_AttackTrigger = Animator.StringToHash("attackTrigger");

    int animLayer_impact;

    float nonOperatingTime = 0f;


    public bool isAerial => aerialTimer <= 0f;//!myContacts.IsAnyContact;
    float aerialTimer = 0f;

    bool isGround => myContacts.IsGround;




    //ボールについてるコンポ（Physicsよりのフィールド）
    public Rigidbody ballRB;
    SphereCollider ballCollider;
    ContactManager myContacts;
    public ContactManager Contacts => myContacts;
    WireManager wireManager;

    float timeScale = 1f;

    float freezWeight = 0f;

    //IKの為のトランスフォーム
    Transform lookForward;
    Transform lookRotation;


    //キーマネージメント

    KeyBuffer key_B;
    KeyBuffer key_R1;
    KeyBuffer key_L2;

    int restJumpCount = 0;
    int restStepCount = 0;
    bool dashFlag = false;


    public Vector3 currentInputVel;
    Vector3 lastInputVel;
    Vector3 toVector;
    Transform tAvatar => animator.transform;


    public Vector3 upNormal => myContacts.UpNormal_Ground;

    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;

        //アバターコンポ
     

        animLayer_impact = animator.GetLayerIndex("impact Layer");

        ikManager = animator.GetComponent<PlayerIKManager>();
        breathingManager = animator.GetComponent<BreathingManager>();


        //ボールコンポ
        ballRB = GetComponent<Rigidbody>();
        ballCollider = GetComponent<SphereCollider>();
        myContacts = GetComponent<ContactManager>();
        wireManager = GetComponent<WireManager>();



        //IK
        lookRotation = new GameObject("lookRotation").transform;
        lookRotation.parent = tAvatar;
        lookRotation.localPosition = Vector3.zero;
        lookForward = new GameObject("lookForward").transform;
        lookForward.parent = lookRotation;
        lookForward.localPosition = Vector3.forward;


        //キーバッファ
        key_B = KeyBufferManager.CreateKey();
        key_R1 = KeyBufferManager.CreateKey();
        key_L2 = KeyBufferManager.CreateKey();





    }
    private void Update()
    {
        bool isAnyContact = myContacts.IsAnyContact;
        bool isWalling = myContacts.IsWalling;

        stepCT -= Time.deltaTime;
        jumpCT -= Time.deltaTime;
        stepCT = Mathf.Clamp(stepCT, 0f, 1f);
        jumpCT = Mathf.Clamp(jumpCT, 0f, 1f);

        //左スティックキー入力
        currentInputVel = -KeyManagerHub.main.GetStickVectorFromNormal(true, myContacts.UpNormal_Simple, false/*Vector3.Dot(upNormal, Camera.main.transform.up) < 0f*/);
        if (!isAerial)
        {
//            currentInputVel *= myContacts.FitWeight;
        }
        if (currentInputVel.magnitude <= 0f && isGround)
        {
            freezWeight += Time.deltaTime * 5f;
        }
        else
        {
            freezWeight -= Time.deltaTime * 5f;
        }
        freezWeight = Mathf.Clamp(freezWeight, 0f, 1f);
        if(stepCT <= 0f)
        {
            if (freezWeight >= 1f)
            {
                ballRB.freezeRotation = true;
            }
            else
            {
                ballRB.freezeRotation = false;
                ballRB.angularVelocity = Vector3.Lerp(ballRB.angularVelocity, Vector3.zero, Time.deltaTime * freezWeight * 30f);
            }
        }
        //        InputVel = Vector3.Lerp(InputVel ,  Vector3.zero , myContacts.Impact.magnitude*3f);

        if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.B)) key_B.SetEnter();

        if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.L2)) key_L2.SetEnter();
        dashFlag = KeyManagerHub.main.GetKey(KeyMode.Stay, Key_Hub.L2);

        if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.R1)) key_R1.SetEnter();
        if (KeyManagerHub.main.GetKey(KeyMode.Exit, Key_Hub.R1)) key_R1.SetExit();


        //float timeChangeSpeed = Time.unscaledDeltaTime * 3f;
        //timeScale += KeyManagerHub.main.GetKey(KeyMode.Stay, Key_Hub.R2) ? -timeChangeSpeed : timeChangeSpeed;
        //timeScale = Mathf.Clamp(timeScale, 0f, 1f);
        //Time.timeScale = Mathf.Lerp(0.3f, 1f, timeScale);//(currentInputVel.magnitude*0.5f);
        if (KeyManagerHub.main.GetKey(KeyMode.Stay, Key_Hub.R2)) Time.timeScale = 0.5f;
        else Time.timeScale = 1f;






        //回転方向と現在方向の差
        Vector3 uN = myContacts.upNormal_Ex;
        //        float angle = Vector3.Angle(uN, animator.transform.up) * 0.05f;

        //人体座標をボール座標と重ねる
        if (!tAvatar.gameObject.activeSelf) return;
        Vector3 uNor = Vector3.Lerp(tAvatar.up, upNormal, Time.deltaTime).normalized;

        float ofsH = 0.05f;

        tAvatar.position = transform.position - (uNor * (ballCollider.radius + ofsH));
        ikManager.currentPos = transform.position - (uNor * (ballCollider.radius + ofsH));
        //人体座標を、衝撃ぶんずらす
        Vector3 impVel = myContacts.Impact;
        impVel = Vector3.ClampMagnitude(impVel, 0.75f);
        //横軸強すぎると顔面壁にぶつかる
        float downDot = Vector3.Dot(upNormal, impVel);
        //ステップ中じゃなければ、空中じゃなければ足のIK補正でしゃがむ、衝撃でしゃがむ
        if (!isAerial && !isSteping)
        {
            downDot = Mathf.Max(downDot, ikManager.HOfs);
        }
        impVel += upNormal * downDot;
        impVel *= 0.5f;
        tAvatar.position -= impVel;

        //アバターの方向転換
        tAvatar.parent = transform.parent;

        Vector3 liv = lastInputVel;


        if (uN != Vector3.up)
        {
            Vector3 axis = Vector3.Cross(Vector3.up, uN);

            //坂道に立っている時の姿勢補正
            float tiltDot = Mathf.Abs(Vector3.Dot(Vector3.up, upNormal)) - 1f;
            if (!isSteping)
            {
                myContacts.setImpact(upNormal * Mathf.Clamp(tiltDot * -1.5f, 0f, 1.5f));
                Quaternion wallQ = Quaternion.AngleAxis(-45f * -tiltDot, axis);
                uN = wallQ * uN;
            }
        }
        Debug.DrawRay(transform.position, uN, Color.red);

        tAvatar.rotation = Quaternion.Lerp(tAvatar.rotation, Quaternion.LookRotation(liv, uN), (tweakParam.avaterRotationSpeed/*+ angle*/) * Time.deltaTime);

        //ステップ中はしゃがまない
        if (isSteping)
        {
            myContacts.DragImpact();
        }

        //IKマネージャーの壁登りフラグを立てる
        //        ikManager.climeFlag = isClime;


        //回転する時は体を丸める
        Vector3 upOfs = upNormal - tAvatar.up;
        //アニメーション更新
        animator.SetLayerWeight(animLayer_impact, (downDot * 4f) + (upOfs.magnitude));


        //非操作時間とそれによるアイドル変化
        if (/*ballRB.velocity.sqrMagnitude*/currentInputVel.sqrMagnitude < 0.01f)
        {
            nonOperatingTime += Time.deltaTime;

            lookRotation.rotation = Quaternion.Lerp(lookRotation.rotation, Camera.main.transform.rotation, Time.deltaTime * 5f);


            if (nonOperatingTime > tweakParam.specialIdleTime)
            {
                ikManager.nextLookTarget = Camera.main.transform;
            }
        }
        else
        {
            //if (isGround)
            //{
            if (toVector.sqrMagnitude <= 0f) toVector = tAvatar.forward;

            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            lookRotation.position = head.position;

            float upVelDot = Vector3.Dot(animator.transform.up, ballRB.velocity) * 0.5f;
            Vector3 lookUpOfs = animator.transform.up * upVelDot;
            Quaternion toRad = Quaternion.LookRotation(toVector + lookUpOfs, Vector3.up); ;
            Quaternion fromRad = lookRotation.rotation;


            lookRotation.rotation = Quaternion.Lerp(fromRad, toRad, Time.deltaTime * 5f);




            Debug.DrawRay(transform.position, lookRotation.forward, Color.yellow);

            ikManager.nextLookTarget = lookForward;
            //}
            //else
            //{
            //    ikManager.nextLookTarget = null;
            //}

            nonOperatingTime = 0f;
        }


        float animationSpeed = Time.deltaTime * 30f;


        //空中判定
        if (!myContacts.IsAnyContact)
        {
            aerialTimer -= Time.deltaTime;
            aerialTimer = Mathf.Clamp(aerialTimer, 0f, 1f);
        }
        else aerialTimer = 0.1f;

        animator.SetBool(animID_isAerial, isAerial);
        //空中ベクトル変化は、空中時のみ変化（チャクチした瞬間等、変化が激しすぎてアニメーションが一瞬で切り替わる）
        ikManager.isAerial = isAerial;
        if (isAerial)
        {

            //空中だった場合ステップアニメはCancel
            animator.ResetTrigger(animID_stepTrigger);
            //空中でステップ中だった場合重力なし,アニメーション重力も上昇に固定
            float zone = 5f;
            float yVel = Mathf.Clamp((animator.transform.InverseTransformVector(myContacts.LastPhysicsVector).y + zone) / (zone * 2f), 0f, 1f);
            yVel = Mathf.Lerp(1f, 0f, yVel);
            yVel = Mathf.Lerp(animator.GetFloat(animID_speed_Y), yVel, Time.deltaTime * 60f);

            ballRB.useGravity = true;


            animator.SetFloat(animID_speed_Y, Mathf.Lerp(animator.GetFloat(animID_speed_Y), yVel, animationSpeed));


            //移動アニメーション空中
            Vector3 animatedVector = ballRB.velocity;
            animatedVector = tAvatar.InverseTransformVector(animatedVector);
            animatedVector.y = 0f;
            float speed_Z = animatedVector.z;
            float speed_X = animatedVector.x;


            animator.SetFloat(animID_speed_X, Mathf.Lerp(animator.GetFloat(animID_speed_X), speed_X, animationSpeed));
            animator.SetFloat(animID_speed_Z, Mathf.Lerp(animator.GetFloat(animID_speed_Z), speed_Z, animationSpeed));
            animator.SetFloat(animID_speed, 1f);
        }
        else
        {
            //移動アニメーション地上
            Vector3 animatedVector = ballRB.angularVelocity * 0.5f;
            animatedVector = tAvatar.InverseTransformVector(animatedVector);
            animatedVector.y = 0f;
            float speed_Z = animatedVector.x;
            float speed_X = -animatedVector.z;


            animator.SetFloat(animID_speed_X, Mathf.Lerp(animator.GetFloat(animID_speed_X), speed_X, animationSpeed));
            animator.SetFloat(animID_speed_Z, Mathf.Lerp(animator.GetFloat(animID_speed_Z), speed_Z, animationSpeed));
            animator.SetFloat(animID_speed, Mathf.Lerp(animator.GetFloat(animID_speed), animatedVector.magnitude * 0.5f, animationSpeed));
        }

        //        playerAnimator.SetBool(animID_isWalling, isClime);

        //息遣い
        if (isAerial)
        {
            breathingManager.Fatigue(ballRB.velocity.magnitude);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //各種判定を確認
        bool isAnyContact = myContacts.IsAnyContact;
        bool isClime = myContacts.IsWalling;


        ikManager.handFlag_L = false;
        ikManager.handFlag_R = false;

        ikManager.handLookParent_L = tAvatar;
        ikManager.handLookParent_R = tAvatar;

        ikManager.targetHandsWeight -= Time.deltaTime;


        breathingManager.muteFlag = false;





        Transform hip = animator.GetBoneTransform(HumanBodyBones.Hips);


        //各種判定の補正
        //if(isGround)
        //{
        //    upNormal = Vector3.up;
        //}



        //左スティックキー入力
        //        Vector3 InputVel = -KeyManagerHub.main.GetStickVectorFromNormal(true, upNormal, Vector3.Dot(upNormal, Camera.main.transform.up) < 0f);

        Vector3 offsetVector = Vector3.zero;
        //ダッシュ中は早い
        toVector = Vector3.ProjectOnPlane(currentInputVel * states.agi * (dashFlag ? 1.5f : 1f), myContacts.UpNormal_Simple);

        //入力が弱い時は壁走りできない
        //        myContacts.fitFlag = toVector.magnitude > 0.8f;

        //ショット
        Vector3 aimPoint = transform.position + (toVector * 100f);
        Debug.DrawLine(transform.position, aimPoint, Color.blue);
        if (key_R1.EnterCheck(!wireManager.isFuck))
        {
            animator.SetTrigger(animID_AttackTrigger);
            //            wireManager.SearchFuck(transform.position, aimPoint, ballRB);
        }

        if (key_R1.Exit)
        {
            //            wireManager.ResetFuck();
        }

        //隠れる状態の時はしゃがむ
        if (transform.parent)
        {
            if (transform.parent.tag == "Hide" && !isSteping)
            {
                myContacts.setImpact(upNormal * 2.0f);
            }
        }



        //頭が天井にぶつからないように補正
        Ray headRay = new Ray(transform.position, upNormal * 0.5f);
        Debug.DrawRay(headRay.origin, headRay.direction, Color.yellow);

        if (Physics.SphereCast(headRay, ballCollider.radius, out RaycastHit hit, 0.5f, handMask))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.cyan);

            float dDot = Vector3.Dot(upNormal, hit.normal);
            if (dDot < 0f && !isSteping)
            {
                Vector3 down = upNormal * dDot;
                myContacts.setImpact(down * -2.0f);

            }

        }




        //物に触る
        Vector3 handVector = toVector;
        Vector3 lsPos = animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position;
        Vector3 rsPos = animator.GetBoneTransform(HumanBodyBones.RightShoulder).position;
        Ray lsRay = new Ray(((lsPos - (tAvatar.right * 0.15f)) + (upNormal * 0.1f)) - (tAvatar.forward * 0.1f), tAvatar.forward * 0.5f);
        Ray rsRay = new Ray(((rsPos + (tAvatar.right * 0.15f)) + (upNormal * 0.1f)) - (tAvatar.forward * 0.1f), tAvatar.forward * 0.5f);

        //                    float forwardVel = Vector3.Dot(tAvatar.forward, handVector);
        float rayRange = 1.5f;
        float rayRadius = 0.3f;


        ikManager.handFlag_L = Physics.SphereCast(lsRay, rayRadius, out RaycastHit lsHit, rayRange, handMask);
        ikManager.handFlag_R = Physics.SphereCast(rsRay, rayRadius, out RaycastHit rsHit, rayRange, handMask);

        //体の逆側へ手は伸ばさない

        Vector3 crossVel_L = Vector3.Cross(handVector, lsHit.point - rsRay.origin);
        Vector3 crossVel_R = Vector3.Cross(handVector, rsHit.point - lsRay.origin);
        Debug.DrawRay(lsRay.origin, crossVel_L);
        Debug.DrawRay(rsRay.origin, crossVel_R);

        //if (invLSPos.x > 0.1f) ikManager.handFlag_L = false;
        //if (invRSPos.x < -0.1f) ikManager.handFlag_R = false;
        if (crossVel_L.y > 1f) ikManager.handFlag_L = false;
        if (crossVel_R.y < -1f) ikManager.handFlag_R = false;

        //
        if (ikManager.handFlag_L)
        {
            ikManager.handLookParent_L = lsHit.transform.transform;

            ikManager.handHit_L = lsHit;


            float tLhandW = Vector3.Dot(-lsHit.normal, handVector);

            ikManager.SetHandTransform(true, lsHit);

            ikManager.targetHandsWeight = tLhandW;
        }
        if (ikManager.handFlag_R)
        {
            ikManager.handLookParent_R = rsHit.transform.transform;

            ikManager.handHit_R = rsHit;

            float tRhandW = Vector3.Dot(-rsHit.normal, handVector);

            ikManager.SetHandTransform(false, rsHit);

            if (ikManager.targetHandsWeight < tRhandW)
            {
                ikManager.targetHandsWeight = tRhandW;
            }
        }

        //if (ikManager.handFlag_L)
        //{
        //    ikManager.handLookParent_L = lsHit.transform.transform;

        //    ikManager.handHit_L = lsHit;
        //    ikManager.SetHandTransform(true, lsHit);


        //    ikManager.targetHandsWeight = 1f;
        //}
        //if (ikManager.handFlag_R)
        //{
        //    ikManager.handLookParent_R = rsHit.transform.transform;
        //    ikManager.handHit_R = rsHit;
        //    ikManager.SetHandTransform(false, rsHit);


        //    ikManager.targetHandsWeight = 1f;
        //}

        ikManager.targetHandsWeight = Mathf.Clamp(ikManager.targetHandsWeight, 0f, 1f);

        //手で触ったものの法線が下だった場合、もしかしたらくぐるかもしれないからちょっとしゃがむ
        Vector3 downVel = lsHit.normal.y < rsHit.normal.y ? lsHit.normal : rsHit.normal;
        if (downVel.y < 0f && !isSteping)
        {
            Vector3 down = Vector3.up * Vector3.Dot(Vector3.down, downVel);
            myContacts.setImpact(down * 0.5f);
        }

        //if (ikManager.handFlag_L || ikManager.handFlag_R)
        //{
        //    toVector -= tAvatar.right * (0.2f * (Vector3.Dot(tAvatar.right, toVector)));//物を押している最中は速度半減
        //}

        //壁登り中は、押し込みモーションを取らない
        //        ikManager.puthWeight = Mathf.Lerp(ikManager.puthWeight, isClime ? 0f : 1f, Time.deltaTime * 3f);

        if (isAnyContact)
        {

            //設地したら、空中ダッシュを回復
            if(isGround)
            {
                restStepCount = states.restStepCount;
                restJumpCount = states.restStepCount;
            }


            //移動
            if(stepCT <= 0f)
            {
                Vector3 fromVector = ballRB.velocity;

                //                if(vel.magnitude > tweakParam.minStickMagnitued)
                //                {
                //                }
                //                else
                //                {
                ////                    offsetVector = Vector3.ProjectOnPlane(-ballRB.velocity , upNormal) * Time.deltaTime*3f;
                //                }
                offsetVector = toVector - fromVector;
                offsetVector *= offsetVector.magnitude;
                offsetVector = Vector3.ClampMagnitude(offsetVector, states.agi);
                offsetVector = Vector3.Lerp(Vector3.zero, offsetVector, Time.deltaTime * currentInputVel.magnitude);

                //ballRB.AddForce(offsetVector, ForceMode.VelocityChange);

                ballRB.AddForceAtPosition(offsetVector, transform.position + upNormal, ForceMode.VelocityChange);
                ballRB.AddForceAtPosition(-offsetVector, transform.position - upNormal, ForceMode.VelocityChange);

            }
        }
        else
        {

            //空中移動
            Vector3 fromVector = ballRB.velocity;

            //                if(vel.magnitude > tweakParam.minStickMagnitued)
            //                {
            //                }
            //                else
            //                {
            ////                    offsetVector = Vector3.ProjectOnPlane(-ballRB.velocity , upNormal) * Time.deltaTime*3f;
            //                }
            offsetVector = toVector - fromVector;
            //                offsetVector *= offsetVector.magnitude;
            offsetVector = Vector3.Lerp(Vector3.zero, offsetVector, Time.deltaTime);
            offsetVector.y = 0f;
            ballRB.AddForce(offsetVector, ForceMode.VelocityChange);

        }

        //チャクチした瞬間は反動でジャンプできない
        if (myContacts.isNowLanding)
        {
            jumpCT = 0.2f;
        }
        //ジャンプ
        if (key_B.EnterCheck(jumpCT <= 0f && restJumpCount > 0))
        {
            Jump(upNormal, toVector);
        }
        //ステップ
        if (key_L2.EnterCheck(stepCT <= 0f && restStepCount > 0))
        {
            Step(myContacts.UpNormal_Simple, toVector);
        }





        //最後に入力をした際のベクトルを取得

        if (wireManager.isFuck)
        {
            lastInputVel = Vector3.ProjectOnPlane(wireManager.NextFuck.position - transform.position, upNormal);
        }
        //else if (KeyManagerHub.main.GetKey(KeyMode.Stay, Key_Hub.L2) /*CameraManager.main.lookTarget*/)//エイム
        //{
        //    lastInputVel = Vector3.ProjectOnPlane(Camera.main.transform.forward, upNormal);
        //}
        else
        {
            if (isGround || isClime)
            {
                if (currentInputVel.magnitude > tweakParam.minStickMagnitued)//入力があった場合
                {
                    if (ikManager.targetHandsWeight > 0f && !isClime)//物に触れていた場合
                    {
                        Vector3 normal = Vector3.zero;
                        if (ikManager.handFlag_L && !ikManager.handFlag_R)
                        {
                            normal = ikManager.handHit_L.normal;
                        }
                        else if (ikManager.handFlag_R && !ikManager.handFlag_L)
                        {
                            normal = ikManager.handHit_R.normal;
                        }
                        else
                        {
                            normal = (ikManager.handHit_L.normal + ikManager.handHit_R.normal) * 0.5f;
                        }
                        lastInputVel = Vector3.ProjectOnPlane(Vector3.Lerp(currentInputVel, -normal, ikManager.targetHandsWeight), upNormal);

                    }
                    else//物に触れていなかった場合
                    {
                        //          lastInputVel = ballRB.velocity;
                        lastInputVel = Vector3.ProjectOnPlane(currentInputVel, upNormal);
                    }
                }
                else//なかった場合
                {
                    lastInputVel = Vector3.ProjectOnPlane(tAvatar.forward, upNormal);
                }
            }
            else
            {
                lastInputVel = Vector3.ProjectOnPlane(ballRB.velocity, upNormal);
            }
        }


    }



    public void Warp()
    {
        myContacts.Reset();
        ballRB.useGravity = true;
        ballRB.velocity = Vector3.zero;

        jumpCT = 0f;
        stepCT = 0f;
        restJumpCount = states.restStepCount;
        restStepCount = states.restStepCount;
    }

    public void SetVelocity(Vector3 vel)
    {
        ballRB.velocity = vel;
    }

    float jumpCT;
    void Jump(Vector3 uN, Vector3 vel)
    {
        //無理やり空中判定に
        aerialTimer = 0f;
        //音
        FootSEManager.PlayClip(0, tAvatar.position, tAvatar.rotation, 1f);
        //ジャンプした直後はステップできない
        stepCT = stepCT_Max / 2f;



        //
        restJumpCount--;
        jumpCT = 0.3f;


        //
        Vector3 jumpingVel = uN * tweakParam.jumpingPower;
        ballRB.velocity = (vel + jumpingVel);


        myContacts.Jump(jumpingVel);
        //ジャンプするたび、軸足は逆に
        animator.SetBool(animID_isLeft, !animator.GetBool(animID_isLeft));

        //(ジャンプする際、回転していると設置判定がバグるから回転を止める）
        ballRB.angularVelocity = Vector3.zero;

        if (myContacts.IsAnyContact)
        {
            //ジャンプする瞬間、アニメーション用のベクトルを更新
            animator.SetFloat(animID_speed_Y, 0f);
        }
    }


    float stepCT = 0f;
    float stepCT_Max = 0.65f;
    bool isSteping => stepCT > 0f;

    void Step(Vector3 uN, Vector3 vel)
    {
        FootSEManager.PlayClip(0, tAvatar.position, tAvatar.rotation, 1f);

        restStepCount--;
        stepCT = stepCT_Max;


        if (vel.sqrMagnitude <= 0f) vel = -animator.transform.forward;

        ballRB.freezeRotation = false;

        Vector3 aerialVel = Vector3.zero;

        if (!myContacts.IsAnyContact)
        {
            aerialVel = Vector3.up * 0.4f;
            ballRB.velocity = (Vector3.ProjectOnPlane(vel, uN).normalized + aerialVel) * states.agi*2f;
        }
        else
        {
            aerialVel = -uN * 0.5f;
            ballRB.velocity = (Vector3.ProjectOnPlane(vel, uN).normalized + aerialVel) * states.agi * 3f;
        }




        //        stepVel += vel*3f;
        //                ballRB.velocity = jumpingVel;
        //ballRB.AddForce(stepVel, ForceMode.VelocityChange);
        //myContacts.Jump(stepVel);


        //ジャンプするたび、軸足は逆に
        animator.SetBool(animID_isLeft, !animator.GetBool(animID_isLeft));

        //(ジャンプする際、回転していると設置判定がバグるから回転を止める）
        ballRB.angularVelocity = Vector3.zero;

        //ジャンプする瞬間、アニメーション用のベクトルを更新
        if (myContacts.IsAnyContact)
        {
            animator.SetFloat(animID_speed_Y, 0f);
        }



        //アニメーション速度変化
        Vector3 animatedVector = ballRB.velocity;
        animatedVector = tAvatar.InverseTransformVector(animatedVector);
        animatedVector.y = 0f;
        float speed_Z = animatedVector.z;
        float speed_X = animatedVector.x;


        animator.SetFloat(animID_speed_X, speed_X);
        animator.SetFloat(animID_speed_Z, speed_Z);
        animator.SetFloat(animID_speed, 1f);

        //ステップトリガー

        animator.SetTrigger(animID_stepTrigger);

    }

}
